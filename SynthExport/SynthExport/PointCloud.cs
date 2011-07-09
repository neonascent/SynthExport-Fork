using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Serialization;
using System.Text;
using System.Xml;

namespace SynthExport
{
    public struct Point
    {
        public float X;
        public float Y;
        public float Z;
        public byte R;
        public byte G;
        public byte B;
    }

    [Serializable]
    public class PointCloudDownloadException : ApplicationException
    {
        public PointCloudDownloadException() { }
        public PointCloudDownloadException(string message) : base(message) { }
        public PointCloudDownloadException(string message, Exception inner) : base(message, inner) { }
        protected PointCloudDownloadException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }

    public class PointCloudDownloader
    {
        public void DownloadPointClouds(SynthData synthData)
        {
            var coordSystemsWithPointCloud = synthData.CoordinateSystems.Where(cs => cs.PointCloud != null);

            int filesDownloaded = 0;
            int totalFileCount = coordSystemsWithPointCloud.Sum(cs => cs.PointCloud.BinFileCount);

            using (WebClient webClient = new WebClient())
                foreach (CoordinateSystem coordSystem in coordSystemsWithPointCloud)
                    for (int i = 0; i < coordSystem.PointCloud.BinFileCount; i++)
                    {
                        string downloadUrl = string.Format("{0}points_{1}_{2}.bin", synthData.CollectionRoot, coordSystem.ID, i);

                        byte[] binFile;

                        try
                        {
                            binFile = webClient.DownloadData(downloadUrl);
                        }
                        catch (WebException e)
                        {
                            throw new PointCloudDownloadException("An error occurred during download of point cloud data files.", e);
                        }

                        using (MemoryStream memoryStream = new MemoryStream(binFile))
                            coordSystem.PointCloud.LoadBinFile(memoryStream);

                        filesDownloaded++;

                        if (CallbackEvent != null)
                            CallbackEvent(this, new CallbackEventArgs(filesDownloaded, totalFileCount));
                    }
        }

        public event EventHandler<CallbackEventArgs> CallbackEvent;

        public class CallbackEventArgs : EventArgs
        {
            public int FilesDownloaded { get; private set; }
            public int TotalFileCount { get; private set; }

            public CallbackEventArgs(int filesDownloaded, int totalFileCount)
            {
                FilesDownloaded = filesDownloaded;
                TotalFileCount = totalFileCount;
            }
        }
    }

    public enum ExportSource
    {
        Website,
        ZipFile
    }

    public class ExportSettings
    {
        public ExportSource Source { get; private set; }
        public string SourcePath { get; private set; }
        public bool ExportPointClouds { get; private set; }
        public bool ExportCameraParameters { get; private set; }
        public bool ExportMaxScript { get; private set; }

        public ExportSettings(ExportSource source, string sourcePath, bool exportPointClouds, bool exportCameraParameters, bool exportMaxScript)
        {
            Source = source;
            SourcePath = sourcePath;
            ExportPointClouds = exportPointClouds;
            ExportCameraParameters = exportCameraParameters;
            ExportMaxScript = exportMaxScript;
        }
    }

    [Serializable]
    public class BinFileParseException : ApplicationException
    {
        public BinFileParseException() { }
        public BinFileParseException(string message) : base(message) { }
        public BinFileParseException(string message, Exception inner) : base(message, inner) { }
        protected BinFileParseException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }

    public class PointCloud
    {
        List<Point> points;

        public int CoordSystem { get; private set; }
        public int BinFileCount { get; private set; }

        public int NumberOfPoints
        {
            get
            {
                return points.Count;
            }
        }

        public PointCloud(int coordSystem)
        {
            points = new List<Point>();

            CoordSystem = coordSystem;
        }

        public PointCloud(int coordSystem, int binFileCount)
        {
            points = new List<Point>();

            CoordSystem = coordSystem;
            BinFileCount = binFileCount;
        }

        Stopwatch stopwatch = new Stopwatch();

        public void LoadBinFile(Stream stream)
        {
            using (BinaryReader binaryReader = new BinaryReader(stream))
                try
                {
                    ushort versionMajor = binaryReader.ReadBigEndianUInt16();
                    ushort versionMinor = binaryReader.ReadBigEndianUInt16();

                    if (versionMajor != 1 || versionMinor != 0)
                        throw new BinFileParseException("The point cloud is stored in an incompatible format and cannot be loaded.");

                    int n = binaryReader.ReadCompressedInt();

                    for (int i = 0; i < n; i++)
                    {
                        int m = binaryReader.ReadCompressedInt();

                        for (int j = 0; j < m; j++)
                        {
                            binaryReader.ReadCompressedInt();
                            binaryReader.ReadCompressedInt();
                        }
                    }

                    int nPoints = binaryReader.ReadCompressedInt();

                    for (int i = 0; i < nPoints; i++)
                    {
                        Point point;

                        point.X = binaryReader.ReadBigEndianSingle();
                        point.Y = binaryReader.ReadBigEndianSingle();
                        point.Z = binaryReader.ReadBigEndianSingle();

                        ushort color = binaryReader.ReadBigEndianUInt16();

                        point.R = (byte)(((color >> 11) * 255) / 31);
                        point.G = (byte)((((color >> 5) & 63) * 255) / 63);
                        point.B = (byte)(((color & 31) * 255) / 31);

                        points.Add(point);
                    }
                }
                catch (EndOfStreamException e)
                {
                    throw new BinFileParseException("The point cloud data seems to be corrupted and cannot be loaded.", e);
                }
        }

        public void ExportAsObj(string path)
        {
            using (StreamWriter streamWriter = new StreamWriter(path))
                foreach (Point point in points)
                {
                    streamWriter.Write("v ");
                    streamWriter.Write(point.X.ToString(CultureInfo.InvariantCulture));
                    streamWriter.Write(' ');
                    streamWriter.Write(point.Y.ToString(CultureInfo.InvariantCulture));
                    streamWriter.Write(' ');
                    streamWriter.Write(point.Z.ToString(CultureInfo.InvariantCulture));
                    streamWriter.Write(' ');
                    streamWriter.Write(point.R.ToString(CultureInfo.InvariantCulture));
                    streamWriter.Write(' ');
                    streamWriter.Write(point.G.ToString(CultureInfo.InvariantCulture));
                    streamWriter.Write(' ');
                    streamWriter.WriteLine(point.B.ToString(CultureInfo.InvariantCulture));
                }
        }

        public void ExportAsPlyAscii(string path)
        {
            using (StreamWriter streamWriter = new StreamWriter(path))
            {
                streamWriter.WriteLine("ply");
                streamWriter.WriteLine("format ascii 1.0");
                streamWriter.WriteLine("element vertex {0}", points.Count);
                streamWriter.WriteLine("property float x");
                streamWriter.WriteLine("property float y");
                streamWriter.WriteLine("property float z");
                streamWriter.WriteLine("property uchar red");
                streamWriter.WriteLine("property uchar green");
                streamWriter.WriteLine("property uchar blue");
                streamWriter.WriteLine("end_header");

                foreach (Point point in points)
                {
                    streamWriter.Write(point.X.ToString(CultureInfo.InvariantCulture));
                    streamWriter.Write(' ');
                    streamWriter.Write(point.Y.ToString(CultureInfo.InvariantCulture));
                    streamWriter.Write(' ');
                    streamWriter.Write(point.Z.ToString(CultureInfo.InvariantCulture));
                    streamWriter.Write(' ');
                    streamWriter.Write(point.R.ToString(CultureInfo.InvariantCulture));
                    streamWriter.Write(' ');
                    streamWriter.Write(point.G.ToString(CultureInfo.InvariantCulture));
                    streamWriter.Write(' ');
                    streamWriter.WriteLine(point.B.ToString(CultureInfo.InvariantCulture));
                }
            }
        }

        public void ExportAsPlyBinary(string path)
        {
            using (FileStream fileStream = File.Create(path))
            {
                StreamWriter streamWriter = new StreamWriter(fileStream);

                streamWriter.WriteLine("ply");
                streamWriter.WriteLine("format binary_little_endian 1.0");
                streamWriter.WriteLine("element vertex {0}", points.Count);
                streamWriter.WriteLine("property float x");
                streamWriter.WriteLine("property float y");
                streamWriter.WriteLine("property float z");
                streamWriter.WriteLine("property uchar red");
                streamWriter.WriteLine("property uchar green");
                streamWriter.WriteLine("property uchar blue");
                streamWriter.WriteLine("end_header");

                streamWriter.Flush();

                BinaryWriter binaryWriter = new BinaryWriter(fileStream);

                foreach (Point point in points)
                {
                    binaryWriter.Write(point.X);
                    binaryWriter.Write(point.Y);
                    binaryWriter.Write(point.Z);
                    binaryWriter.Write(point.R);
                    binaryWriter.Write(point.G);
                    binaryWriter.Write(point.B);
                }
            }
        }

        public void ExportAsVrml(string path)
        {
            using (StreamWriter streamWriter = new StreamWriter(path))
            {
                streamWriter.WriteLine("#VRML V2.0 utf8");
                streamWriter.WriteLine("Group { children [");
                streamWriter.WriteLine("  Shape {");
                streamWriter.WriteLine("    geometry PointSet {");
                streamWriter.WriteLine("      coord Coordinate {");
                streamWriter.Write("        point [ ");

                for (int i = 0; i < points.Count; i++)
                {
                    streamWriter.Write(points[i].X.ToString(CultureInfo.InvariantCulture));
                    streamWriter.Write(' ');
                    streamWriter.Write(points[i].Y.ToString(CultureInfo.InvariantCulture));
                    streamWriter.Write(' ');
                    streamWriter.Write(points[i].Z.ToString(CultureInfo.InvariantCulture));

                    if (i < points.Count - 1)
                        streamWriter.Write(", ");
                }

                streamWriter.WriteLine(" ]");
                streamWriter.WriteLine("      }");
                streamWriter.WriteLine("      color Color {");
                streamWriter.WriteLine("        color [ ");

                for (int i = 0; i < points.Count; i++)
                {
                    streamWriter.Write((points[i].R / 255.0).ToString(CultureInfo.InvariantCulture));
                    streamWriter.Write(' ');
                    streamWriter.Write((points[i].G / 255.0).ToString(CultureInfo.InvariantCulture));
                    streamWriter.Write(' ');
                    streamWriter.Write((points[i].B / 255.0).ToString(CultureInfo.InvariantCulture));

                    if (i < points.Count - 1)
                        streamWriter.Write(", ");
                }

                streamWriter.WriteLine(" ]");
                streamWriter.WriteLine("      }");
                streamWriter.WriteLine("    }");
                streamWriter.WriteLine("  }");
                streamWriter.WriteLine("]}");
            }
        }

        public void ExportAsX3d(string path)
        {
            StringBuilder point = new StringBuilder();
            StringBuilder color = new StringBuilder();

            for (int i = 0; i < points.Count; i++)
            {
                point.Append(points[i].X.ToString(CultureInfo.InvariantCulture));
                point.Append(' ');
                point.Append(points[i].Y.ToString(CultureInfo.InvariantCulture));
                point.Append(' ');
                point.Append(points[i].Z.ToString(CultureInfo.InvariantCulture));

                color.Append((points[i].R / 255.0).ToString(CultureInfo.InvariantCulture));
                color.Append(' ');
                color.Append((points[i].G / 255.0).ToString(CultureInfo.InvariantCulture));
                color.Append(' ');
                color.Append((points[i].B / 255.0).ToString(CultureInfo.InvariantCulture));
                color.Append(" 1");

                if (i < points.Count - 1)
                {
                    point.Append(' ');
                    color.Append(' ');
                }
            }

            using (XmlWriter xmlWriter = XmlWriter.Create(path))
            {
                xmlWriter.WriteDocType("X3D", "ISO//Web3D//DTD X3D 3.1//EN", "http://www.web3d.org/specifications/x3d-3.1.dtd", null);

                xmlWriter.WriteStartElement("X3D");
                xmlWriter.WriteAttributeString("profile", "Immersive");
                xmlWriter.WriteAttributeString("version", "3.1");
                xmlWriter.WriteStartElement("Scene");
                xmlWriter.WriteStartElement("Shape");
                xmlWriter.WriteStartElement("PointSet");
                xmlWriter.WriteStartElement("Coordinate");
                xmlWriter.WriteAttributeString("point", point.ToString());
                xmlWriter.WriteEndElement();
                xmlWriter.WriteStartElement("ColorRGBA");
                xmlWriter.WriteAttributeString("color", color.ToString());
                xmlWriter.WriteEndElement();
                xmlWriter.WriteEndElement();
                xmlWriter.WriteEndElement();
                xmlWriter.WriteEndElement();
                xmlWriter.WriteEndElement();
            }
        }
    }

    public static class BigEndianExtensions
    {
        public static int ReadCompressedInt(this BinaryReader binaryReader)
        {
            int i = 0;
            byte b;

            do
            {
                b = binaryReader.ReadByte();
                i = (i << 7) | (b & 127);
            } while (b < 128);

            return i;
        }

        public static float ReadBigEndianSingle(this BinaryReader binaryReader)
        {
            byte[] b = binaryReader.ReadBytes(4);

            return BitConverter.ToSingle(new byte[] { b[3], b[2], b[1], b[0] }, 0);
        }

        public static ushort ReadBigEndianUInt16(this BinaryReader binaryReader)
        {
            byte b1 = binaryReader.ReadByte();
            byte b2 = binaryReader.ReadByte();

            return (ushort)(b2 | (b1 << 8));
        }
    }
}
