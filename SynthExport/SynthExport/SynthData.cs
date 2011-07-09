using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Serialization;
using System.ServiceModel;
using Ionic.Zip;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SynthExport.PhotosynthService;

namespace SynthExport
{
    [Serializable]
    public class SynthDataLoadException : ApplicationException
    {
        public SynthDataLoadException() { }
        public SynthDataLoadException(string message) : base(message) { }
        public SynthDataLoadException(string message, Exception inner) : base(message, inner) { }
        protected SynthDataLoadException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }

    public class SynthData
    {
        public string CollectionID { get; private set; }
        public string CollectionRoot { get; private set; }
        public CoordinateSystem[] CoordinateSystems { get; private set; }

        private SynthData()
        {
        }

        private void LoadJsonData(TextReader jsonReader)
        {
            try
            {
                JObject jsonData = JObject.Load(new JsonTextReader(jsonReader));

                JToken collections = jsonData["l"] ?? jsonData["collections"];

                JToken collection = collections.Values().First();

                int numCoordSystems = collection["_num_coord_systems"].Value<int>();

                CoordinateSystems = new CoordinateSystem[numCoordSystems];

                JToken coordSystems = collection["x"] ?? collection["coord_systems"];

                for (int id = 0; id < numCoordSystems; id++)
                {
                    CoordinateSystems[id] = new CoordinateSystem(id);

                    JToken coordSystem = coordSystems[Convert.ToString(id)];

                    JToken pointcloud = coordSystem["k"] ?? coordSystem["pointcloud"];

                    if (pointcloud != null)
                    {
                        string s = pointcloud[0].Value<string>();

                        if (!string.IsNullOrEmpty(s))
                        {
                            JToken binFileCount = pointcloud[1];

                            CoordinateSystems[id].PointCloud = new PointCloud(id, binFileCount.Value<int>());
                        }
                    }

                    JToken projectors = coordSystem["r"] ?? coordSystem["projectors"];

                    List<CameraParameters> cameraParameterList = new List<CameraParameters>();
                  
                    foreach (JToken projector in projectors.Values())
                    {
                        string imageName = ((JProperty)projector.Parent).Name;
                        
                        JToken projectorParameters = projector["j"] ?? projector["image_position_rotation_aspect_focallength"];

                        //int imageId = projectorParameters[0].Value<int>(); // this is out of all the PhotoSynth images, i.e. the thumbs or distort directory
                        int imageId = int.Parse(imageName); // this is out of the working coordinate system, i.e. /pmvs/visualize

                        //string sImageId = projectorParameters[0].Value<string>() + "_or_" + int.Parse(imageName); 

                        CameraPosition position = new CameraPosition()
                        {
                            X = projectorParameters[1].Value<double>(),
                            Y = projectorParameters[2].Value<double>(),
                            Z = projectorParameters[3].Value<double>()
                        };

                        CameraRotation rotation = CameraRotation.FromNormalizedQuaternion(
                            projectorParameters[4].Value<double>(),
                            projectorParameters[5].Value<double>(),
                            projectorParameters[6].Value<double>());

                        double aspectRatio = projectorParameters[7].Value<double>();

                        double focalLength = projectorParameters[8].Value<double>();

                        JToken radialDistortion = projector["f"] ?? projector["radial_distortion"];

                        CameraDistortion distortion;

                        if (radialDistortion != null)
                            distortion = new CameraDistortion()
                            {
                                K1 = radialDistortion[0].Value<double>(),
                                K2 = radialDistortion[1].Value<double>()
                            };
                        else
                            distortion = new CameraDistortion();

                        cameraParameterList.Add(new CameraParameters(imageId, position, rotation, aspectRatio, focalLength, distortion));
                    }

                    cameraParameterList.Sort(new Comparison<CameraParameters>((cp1, cp2) => cp1.ImageId.CompareTo(cp2.ImageId)));

                    CoordinateSystems[id].CameraParameterList = new CameraParameterList(cameraParameterList);
                }
            }
            catch (Exception e)
            {
                if (e is JsonReaderException)
                    throw new SynthDataLoadException("An error occurred while processing synth data.", e);

                throw;
            }
        }

        public static SynthData Load(string collectionID)
        {
            SynthData synthData = new SynthData();

            synthData.CollectionID = collectionID;

            string jsonUrl;

            try
            {
                using (PhotosynthServiceSoapClient soapClient = new PhotosynthServiceSoapClient("PhotosynthServiceSoap"))
                {
                    CollectionResult collectionResult = soapClient.GetCollectionData(new Guid(collectionID), false);

                    if (collectionResult.Result != Result.OK)
                        throw new SynthDataLoadException(string.Format("The Photosynth web service returned an error (Error code: {0}). Make sure the URL you entered is correct and the synth exists.", collectionResult.Result));

                    if (collectionResult.CollectionType != CollectionType.Synth)
                        throw new SynthDataLoadException("The URL you have entered belongs to a panorama hosted on photosynth.net. SynthExport is compatible with photosynths only.");

                    synthData.CollectionRoot = collectionResult.CollectionRoot;
                    jsonUrl = collectionResult.JsonUrl;
                }
            }
            catch (CommunicationException e)
            {
                throw new SynthDataLoadException("An error occurred while trying to contact the Photosynth web service.", e);
            }

            string jsonString;

            try
            {
                using (WebClient webClient = new WebClient())
                    jsonString = webClient.DownloadString(jsonUrl);
            }
            catch (WebException e)
            {
                throw new SynthDataLoadException("An error occured while downloading synth data.", e);
            }

            synthData.LoadJsonData(new StringReader(jsonString));

            return synthData;
        }

        public static SynthData LoadFromZipFile(string path)
        {
            SynthData synthData = new SynthData();

            try
            {
                using (ZipFile zipFile = new ZipFile(path))
                {
                    using (MemoryStream stream = new MemoryStream())
                    {
                        zipFile["0.json"].Extract(stream);

                        stream.Seek(0, SeekOrigin.Begin);

                        synthData.LoadJsonData(new StreamReader(stream));
                    }

                    foreach (ZipEntry zipEntry in zipFile)
                    {
                        string name = zipEntry.FileName.ToLower();

                        if (!name.StartsWith("points_") || !name.EndsWith(".bin"))
                            continue;

                        int i = name.IndexOf('_', 7);

                        if (i < 0)
                            continue;

                        int coordSystem;

                        if (!int.TryParse(name.Substring(7, i - 7), out coordSystem))
                            continue;

                        int index;

                        if (!int.TryParse(name.Substring(i + 1, name.Length - i - 5), out index))
                            continue;

                        using (MemoryStream stream = new MemoryStream())
                        {
                            zipEntry.Extract(stream);

                            stream.Seek(0, SeekOrigin.Begin);

                            synthData.CoordinateSystems[coordSystem].PointCloud.LoadBinFile(stream);
                        }
                    }
                }
            }
            catch (ZipException e)
            {
                throw new SynthDataLoadException("Could not extract synth data and point clouds from zip archive. The file seems to be corrupted.", e);
            }

            return synthData;
        }
    }
}
