using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace SynthExport
{
    public struct CameraPosition
    {
        public double X;
        public double Y;
        public double Z;
    }
/*
    public struct CameraRotation
    {
        public double Rx;
        public double Ry;
        public double Rz;

        public static CameraRotation FromNormalizedQuaternion(double x0, double x1, double x2)
        {
            if (x0 == 0 && x1 == 0 && x2 == 0)
                return new CameraRotation();

            double a = x0 * x0 + x1 * x1 + x2 * x2;

            double s = Math.Sqrt(a);

            double x3 = Math.Sqrt(1 - a);

            double l = Math.Atan2(s, x3);

            double k = l * 2.0 / s;

            return new CameraRotation()
            {
                Rx = x0 * k,
                Ry = x1 * k,
                Rz = x2 * k
            };

        }
    }
    */
    public struct CameraRotation
    {
        public double[,] Matrix;

        public static CameraRotation FromNormalizedQuaternion(double x, double y, double z)
        {
            if (x == 0 && y == 0 && z == 0)
                return new CameraRotation() { Matrix = new double[3, 3] };

            double w = Math.Sqrt(1 - x * x - y * y - z * z);
            //if (x ^ 2 + y ^ 2 + z ^ 2 >= 1.0) w = 0.0;

            double[,] matrix = new double[3, 3];

            matrix[0, 0] = 1 - 2 * y * y - 2 * z * z;
            matrix[0, 1] = 2 * x * y + 2 * z * w;
            matrix[0, 2] = 2 * x * z - 2 * y * w;
            matrix[1, 0] = 2 * x * y - 2 * z * w;
            matrix[1, 1] = 1 - 2 * x * x - 2 * z * z;
            matrix[1, 2] = 2 * y * z + 2 * x * w;
            matrix[2, 0] = 2 * x * z + 2 * y * w;
            matrix[2, 1] = 2 * y * z - 2 * x * w;
            matrix[2, 2] = 1 - 2 * x * x - 2 * y * y;

            return new CameraRotation() { Matrix = matrix };
        }
    }

  

    public struct CameraDistortion
    {
        public double K1;
        public double K2;
    }

    public class CameraParameters
    {
        public int ImageId { get; private set; }
        public CameraPosition Position { get; private set; }
        public CameraRotation Rotation { get; private set; }
        public double AspectRatio { get; private set; }
        public double FocalLength { get; private set; }
        public CameraDistortion RadialDistortion { get; private set; }

        public CameraParameters(int imageId, CameraPosition position, CameraRotation rotation, double aspectRatio, double focalLength, CameraDistortion radialDistortion)
        {
            ImageId = imageId;
            Position = position;
            Rotation = rotation;
            AspectRatio = aspectRatio;
            FocalLength = focalLength;
            RadialDistortion = radialDistortion;
        }
    }

    public class CameraParameterList
    {
        List<CameraParameters> cameraParameterList;

        public CameraParameterList(IEnumerable<CameraParameters> cameraParameters)
        {
            cameraParameterList = new List<CameraParameters>(cameraParameters);
        }

        public int NumberOfImages
        {
            get
            {
                return cameraParameterList.Count;
            }
        }

        public void ExportAsCsv(string path)
        {
            using (StreamWriter streamWriter = new StreamWriter(path))
            {
                streamWriter.WriteLine("ImageId, AspectRatio, FocalLength, RadialDistortionK1, RadialDistortionK2");
                //streamWriter.WriteLine("ImageId, PositionX, PositionY, PositionZ, RotationMatrix1, RotationMatrix2, RotationMatrix3, AspectRatio, FocalLength, RadialDistortionK1, RadialDistortionK2");

                foreach (CameraParameters parameters in cameraParameterList)
                {

                    streamWriter.Write(parameters.ImageId.ToString(CultureInfo.InvariantCulture)+", ");
                    streamWriter.Write(parameters.AspectRatio.ToString(CultureInfo.InvariantCulture) + ", ");
                    streamWriter.Write(parameters.FocalLength.ToString(CultureInfo.InvariantCulture) + ", ");
                    streamWriter.Write(parameters.RadialDistortion.K1.ToString(CultureInfo.InvariantCulture) + ", ");
                    streamWriter.WriteLine(parameters.RadialDistortion.K2.ToString(CultureInfo.InvariantCulture));
                    
                
                }
            }
        }

        public void ExportAsMaxScriptJustCameras(string path, string setName)
        {


            using (StreamWriter streamWriter = new StreamWriter(path))
            {

                int totalcameras = cameraParameterList.Count;

                streamWriter.WriteLine("/* 3DS Max Camera Exporter by Josh Harle (http://tacticalspace.org) */");
                streamWriter.WriteLine("if queryBox \"Before running, make sure your mesh object is named 'object_"+setName+"'. Have you done this?\" beep:true then (");
                streamWriter.WriteLine();
                //streamWriter.WriteLine("/*  Enable initial camera states below; 1 = enabled, 0 = disabled */");
                streamWriter.WriteLine();
                //streamWriter.WriteLine("messageBox \"3DS Max Camera and Projection Map Exporter by Josh Harle (http://tacticalspace.org)\"");
                streamWriter.WriteLine();

                //foreach (CameraParameters parameters in cameraParameterList)
                //{
                //    streamWriter.WriteLine("Enable_Camera_" + parameters.ImageId.ToString(CultureInfo.InvariantCulture) + " = 1");
                //}
                streamWriter.WriteLine();
                streamWriter.WriteLine("progressstart \"Adding Cameras and Camera Maps\"");
                streamWriter.WriteLine();
                streamWriter.WriteLine("startCamera = matrix3 [1,0,0] [0,-1,0] [0,0,-1] [0,0,0]");
                streamWriter.WriteLine("sideCamera = matrix3 [0,1,0] [1,0,0] [0,0,-1] [0,0,0]");
                streamWriter.WriteLine("t = matrix3 [1,0,0] [0,1,0] [0,0,1] [0,0,0]");
                streamWriter.WriteLine("R = matrix3 [1,0,0] [0,1,0] [0,0,1] [0,0,0]");
                streamWriter.WriteLine();

                foreach (CameraParameters parameters in cameraParameterList)
                {


                    streamWriter.WriteLine("progressupdate (100.0*" + parameters.ImageId.ToString(CultureInfo.InvariantCulture) + "/" + totalcameras.ToString() + ")");
                    // matrix
                    //http://photosynth.net/view.aspx?cid=84daebb1-557f-48f6-b88d-a8566849e88c

                    streamWriter.Write("Camera");
                    streamWriter.Write(parameters.ImageId.ToString(CultureInfo.InvariantCulture));
                    //streamWriter.WriteLine(" = freecamera()");
                    streamWriter.Write(" = ");
                    streamWriter.WriteLine("freecamera name: \"" + parameters.ImageId.ToString(CultureInfo.InvariantCulture) + "_" + setName +"\"");

                    streamWriter.WriteLine("Camera" + parameters.ImageId.ToString(CultureInfo.InvariantCulture) + ".fov = cameraFOV.MMtoFOV " + (35 * parameters.FocalLength).ToString(CultureInfo.InvariantCulture));

                    streamWriter.Write("R.row1 = [");
                    streamWriter.Write((parameters.Rotation.Matrix[0, 0]).ToString(CultureInfo.InvariantCulture));
                    streamWriter.Write(", ");
                    streamWriter.Write((parameters.Rotation.Matrix[0, 1]).ToString(CultureInfo.InvariantCulture));
                    streamWriter.Write(", ");
                    streamWriter.Write((parameters.Rotation.Matrix[0, 2]).ToString(CultureInfo.InvariantCulture));
                    streamWriter.WriteLine("]");

                    //myTransform.row2 = [10.0,20.0,30.0] 
                    streamWriter.Write("R.row2 = [");
                    streamWriter.Write((parameters.Rotation.Matrix[1, 0]).ToString(CultureInfo.InvariantCulture));
                    streamWriter.Write(", ");
                    streamWriter.Write((parameters.Rotation.Matrix[1, 1]).ToString(CultureInfo.InvariantCulture));
                    streamWriter.Write(", ");
                    streamWriter.Write((parameters.Rotation.Matrix[1, 2]).ToString(CultureInfo.InvariantCulture));
                    streamWriter.WriteLine("]");

                    //myTransform.row3 = [10.0,20.0,30.0] 
                    streamWriter.Write("R.row3 = [");
                    streamWriter.Write((parameters.Rotation.Matrix[2, 0]).ToString(CultureInfo.InvariantCulture));
                    streamWriter.Write(", ");
                    streamWriter.Write((parameters.Rotation.Matrix[2, 1]).ToString(CultureInfo.InvariantCulture));
                    streamWriter.Write(", ");
                    streamWriter.Write((parameters.Rotation.Matrix[2, 2]).ToString(CultureInfo.InvariantCulture));
                    streamWriter.WriteLine("]");


                    //myTransform.row4 = [10.0,20.0,30.0] 
                    streamWriter.Write("t.row4 = [");
                    streamWriter.Write(parameters.Position.X.ToString(CultureInfo.InvariantCulture));
                    streamWriter.Write(", ");
                    streamWriter.Write(parameters.Position.Y.ToString(CultureInfo.InvariantCulture));
                    streamWriter.Write(", ");
                    streamWriter.Write(parameters.Position.Z.ToString(CultureInfo.InvariantCulture));
                    streamWriter.WriteLine("] ");

                    //$Teapot01.transform = myTransform
                    string cameratype = "startCamera";
                    string wAngle = "0";
                    if (parameters.AspectRatio < 1)
                    {
                        cameratype = "sideCamera";
                        wAngle = "-90";
                    }

                    streamWriter.WriteLine("Camera" + parameters.ImageId.ToString(CultureInfo.InvariantCulture) + ".transform = " + cameratype + " * R * t");
                    streamWriter.WriteLine("");


                }

                               
                streamWriter.WriteLine("");
                streamWriter.WriteLine("progressend()");
                streamWriter.WriteLine("");
                streamWriter.WriteLine("selectionSets[\"" + setName + "\"] = $*_" + setName + " --all the current objects named \"*_" + setName + "\"");
                streamWriter.WriteLine("");
                streamWriter.WriteLine("");
                streamWriter.WriteLine("");

                streamWriter.WriteLine("messageBox \"Cameras added!\"");
                streamWriter.WriteLine(")");
            }
        }

        public void ExportAsMaxScript(string path)
        {

                       
            using (StreamWriter streamWriter = new StreamWriter(path))
            {

                int totalcameras = cameraParameterList.Count;
                
                streamWriter.WriteLine("/* 3DS Max Camera and Projection Map Exporter by Josh Harle (http://tacticalspace.org) */");
                streamWriter.WriteLine("/*  Enable initial camera states below; 1 = enabled, 0 = disabled */");              
                streamWriter.WriteLine();
                //streamWriter.WriteLine("messageBox \"3DS Max Camera and Projection Map Exporter by Josh Harle (http://tacticalspace.org)\"");
                streamWriter.WriteLine();
                streamWriter.WriteLine("if queryBox \"Before starting, remember to added the folder containing your images (e.g. /pmvs/visualize) to 3DS Max path list using Customize -> Configure-User-Paths -> External Files -> Add.  Have you done this?\" beep:true then (");
                streamWriter.WriteLine();
                streamWriter.WriteLine("Default_Mix_Type = 2 /*  0 = Additive, 1 = Subtractive, 2 = Mix */");
                streamWriter.WriteLine("Default_Mix_Amount = 100");
                streamWriter.WriteLine("Default_Angle_Threshold = 90");
                                
                foreach (CameraParameters parameters in cameraParameterList)
                {
                    streamWriter.WriteLine("Enable_Camera_" + parameters.ImageId.ToString(CultureInfo.InvariantCulture)  + " = 1");
                }
                streamWriter.WriteLine();
                streamWriter.WriteLine("progressstart \"Adding Cameras and Camera Maps\"");
                streamWriter.WriteLine();
                streamWriter.WriteLine("startCamera = matrix3 [1,0,0] [0,-1,0] [0,0,-1] [0,0,0]");
                streamWriter.WriteLine("sideCamera = matrix3 [0,1,0] [1,0,0] [0,0,-1] [0,0,0]");
                streamWriter.WriteLine("t = matrix3 [1,0,0] [0,1,0] [0,0,1] [0,0,0]");
                streamWriter.WriteLine("R = matrix3 [1,0,0] [0,1,0] [0,0,1] [0,0,0]");
                streamWriter.WriteLine();

                int materialNum = 1, compositeNum = 1;
                string compositeMaterial = "";

                foreach (CameraParameters parameters in cameraParameterList)
                {


                    streamWriter.WriteLine("progressupdate (100.0*" + parameters.ImageId.ToString(CultureInfo.InvariantCulture) + "/" + totalcameras.ToString()+")");
                    // matrix
                    //http://photosynth.net/view.aspx?cid=84daebb1-557f-48f6-b88d-a8566849e88c

                    streamWriter.Write("Camera");
                    streamWriter.Write(parameters.ImageId.ToString(CultureInfo.InvariantCulture));
                    //streamWriter.WriteLine(" = freecamera()");
                    streamWriter.Write(" = ");
                    streamWriter.WriteLine("freecamera name: \"" + parameters.ImageId.ToString(CultureInfo.InvariantCulture) + "\"");
                    
                    streamWriter.WriteLine("Camera"+parameters.ImageId.ToString(CultureInfo.InvariantCulture)+".fov = cameraFOV.MMtoFOV "+(35 * parameters.FocalLength).ToString(CultureInfo.InvariantCulture));

                    streamWriter.Write("R.row1 = [");
                    streamWriter.Write((parameters.Rotation.Matrix[0, 0]).ToString(CultureInfo.InvariantCulture));
                    streamWriter.Write(", ");
                    streamWriter.Write((parameters.Rotation.Matrix[0, 1]).ToString(CultureInfo.InvariantCulture));
                    streamWriter.Write(", ");
                    streamWriter.Write((parameters.Rotation.Matrix[0, 2]).ToString(CultureInfo.InvariantCulture));
                    streamWriter.WriteLine("]");

                    //myTransform.row2 = [10.0,20.0,30.0] 
                    streamWriter.Write("R.row2 = [");
                    streamWriter.Write((parameters.Rotation.Matrix[1, 0]).ToString(CultureInfo.InvariantCulture));
                    streamWriter.Write(", ");
                    streamWriter.Write((parameters.Rotation.Matrix[1, 1]).ToString(CultureInfo.InvariantCulture));
                    streamWriter.Write(", ");
                    streamWriter.Write((parameters.Rotation.Matrix[1, 2]).ToString(CultureInfo.InvariantCulture));
                    streamWriter.WriteLine("]");

                    //myTransform.row3 = [10.0,20.0,30.0] 
                    streamWriter.Write("R.row3 = [");
                    streamWriter.Write((parameters.Rotation.Matrix[2, 0]).ToString(CultureInfo.InvariantCulture));
                    streamWriter.Write(", ");
                    streamWriter.Write((parameters.Rotation.Matrix[2, 1]).ToString(CultureInfo.InvariantCulture));
                    streamWriter.Write(", ");
                    streamWriter.Write((parameters.Rotation.Matrix[2, 2]).ToString(CultureInfo.InvariantCulture));
                    streamWriter.WriteLine("]");

       
                    //myTransform.row4 = [10.0,20.0,30.0] 
                    streamWriter.Write("t.row4 = [");
                    streamWriter.Write(parameters.Position.X.ToString(CultureInfo.InvariantCulture));
                    streamWriter.Write(", ");
                    streamWriter.Write(parameters.Position.Y.ToString(CultureInfo.InvariantCulture));
                    streamWriter.Write(", ");
                    streamWriter.Write(parameters.Position.Z.ToString(CultureInfo.InvariantCulture));
                    streamWriter.WriteLine("] ");

                    //$Teapot01.transform = myTransform
                    string cameratype = "startCamera";
                    string wAngle = "0";
                    if (parameters.AspectRatio < 1)
                    {
                        cameratype = "sideCamera";
                        wAngle = "-90";
                    }

                    streamWriter.WriteLine("Camera"+parameters.ImageId.ToString(CultureInfo.InvariantCulture)+".transform = " + cameratype + " * R * t");

                    streamWriter.WriteLine("");

                    // create camera mapping for this camera

                    streamWriter.WriteLine("/* Create Diffuse */");
                    streamWriter.WriteLine("bm" + parameters.ImageId.ToString(CultureInfo.InvariantCulture) + " = BitmapTexture name: \"bitmap_" + parameters.ImageId.ToString("00000000") + "\" filename: \"" + parameters.ImageId.ToString("00000000") + ".jpg\"");
                    streamWriter.WriteLine("bm" + parameters.ImageId.ToString(CultureInfo.InvariantCulture) + ".coords.V_Tile = false");
                    streamWriter.WriteLine("bm" + parameters.ImageId.ToString(CultureInfo.InvariantCulture) + ".coords.U_Tile = false");
                    streamWriter.WriteLine("bm" + parameters.ImageId.ToString(CultureInfo.InvariantCulture) + ".coords.W_Angle = "+ wAngle);
                    streamWriter.WriteLine("/* create camera  map per pixel */");
                    streamWriter.WriteLine("cm_dif_" + parameters.ImageId.ToString(CultureInfo.InvariantCulture) + " = Camera_Map_Per_Pixel name: \"cameramap_" + parameters.ImageId.ToString("00000000") + "\" camera: Camera" + parameters.ImageId.ToString(CultureInfo.InvariantCulture) + " backFace: true angleThreshold: Default_Angle_Threshold texture: bm" + parameters.ImageId.ToString(CultureInfo.InvariantCulture));
                    
                    streamWriter.WriteLine("/* Create Opacity */");
                    streamWriter.WriteLine("bmo" + parameters.ImageId.ToString(CultureInfo.InvariantCulture) + " = BitmapTexture name: \"bitmap_" + parameters.ImageId.ToString("00000000") + "\" filename: \"" + parameters.ImageId.ToString("00000000") + ".jpg\"");
                    streamWriter.WriteLine("bmo" + parameters.ImageId.ToString(CultureInfo.InvariantCulture) + ".coords.V_Tile = false");
                    streamWriter.WriteLine("bmo" + parameters.ImageId.ToString(CultureInfo.InvariantCulture) + ".coords.U_Tile = false");
                    streamWriter.WriteLine("bmo" + parameters.ImageId.ToString(CultureInfo.InvariantCulture) + ".coords.W_Angle = " + wAngle);
                    streamWriter.WriteLine("bmo" + parameters.ImageId.ToString(CultureInfo.InvariantCulture) + ".output.Output_Amount = 100");
                    streamWriter.WriteLine("cmo_" + parameters.ImageId.ToString(CultureInfo.InvariantCulture) + " = Camera_Map_Per_Pixel name: \"cameramap_o_" + parameters.ImageId.ToString("00000000") + "\" camera: Camera" + parameters.ImageId.ToString(CultureInfo.InvariantCulture) + " backFace: true angleThreshold: 90 texture: bmo" + parameters.ImageId.ToString(CultureInfo.InvariantCulture));
                    
                    streamWriter.WriteLine("");
                    streamWriter.WriteLine("/* create standard map */");

                    streamWriter.WriteLine("m" + parameters.ImageId.ToString(CultureInfo.InvariantCulture) + " = standardMaterial name: \"material_" + parameters.ImageId.ToString("00000000") + "\" diffuseMap: cm_dif_" + parameters.ImageId.ToString(CultureInfo.InvariantCulture) + " opacityMap: cmo_" + parameters.ImageId.ToString(CultureInfo.InvariantCulture) + " showInViewport: true");
                    



                    // create or add to composite material
                    if (materialNum == 10) {
                        materialNum = 1;
                        compositeNum++;
                    }
                    compositeMaterial = "cpm_"+compositeNum.ToString();
                    string lastCompositeMaterial = "cpm_" + (compositeNum - 1).ToString();

                    // create new composite material
                    if (materialNum == 1) {

                        streamWriter.WriteLine(compositeMaterial + " = compositeMaterial()");
                        
                        // if first one 
                        if (compositeNum == 1)
                        {
                            streamWriter.WriteLine(compositeMaterial + ".materialList[1] = standardMaterial name: \"Base Composite Material\" opacity: 100");
                        }
                        else
                        {
                            streamWriter.WriteLine(compositeMaterial + ".materialList[1] = " + lastCompositeMaterial);
                        }

                    }

                    streamWriter.WriteLine(compositeMaterial+".materialList["+(materialNum+1).ToString()+"] = m"+parameters.ImageId.ToString(CultureInfo.InvariantCulture)+"");
                    streamWriter.WriteLine(compositeMaterial + ".amount[" + (materialNum).ToString() + "] = Default_Mix_Amount");
                    streamWriter.WriteLine(compositeMaterial + ".mixType[" + (materialNum).ToString() + "] = Default_Mix_Type");

                    streamWriter.WriteLine("if Enable_Camera_" + parameters.ImageId.ToString(CultureInfo.InvariantCulture)+" != 1 do ( "+compositeMaterial+".mapEnables["+(materialNum+1).ToString()+"] = false )");
                    streamWriter.WriteLine("");
	                materialNum++;
	 

                }


                streamWriter.WriteLine("for node in rootNode.children do(");
                streamWriter.WriteLine("node.material = " + compositeMaterial);
                streamWriter.WriteLine(")");

                streamWriter.WriteLine("");
                streamWriter.WriteLine("progressend()");
                streamWriter.WriteLine("");
                streamWriter.WriteLine("messageBox \"Cameras and materials added!  Make sure you have rotated your model 90° to fit the camera coordinate system.  You can change the mix of materials to be projected by opening the material editor (press M), Material -> Get All Scene Materials, and then changing the setting in each combination material.\"");
                streamWriter.WriteLine(")");



            }
        }


        public void ExportAsBundle(string path)
        {
            using (StreamWriter streamWriter = new StreamWriter(path))
            {
                
                streamWriter.WriteLine("# bundle output file");
                streamWriter.WriteLine("# written by SynthExport");
                streamWriter.WriteLine(cameraParameterList.Count.ToString(CultureInfo.InvariantCulture) + " 0");

                foreach (CameraParameters parameters in cameraParameterList)
                {
                    // sneaky constant
                    int imageResolution = 2264;

                    //  <f> <k1> <k2>   [the focal length, followed by two radial distortion coeffs]
                    //document.form1.bundle.value += Number(res / FocalLength).toExponential(10) + " " + Number(RadialDistortionK1).toExponential(10) + " " + Number(RadialDistortionK2).toExponential(10) + "\n";

                    streamWriter.Write((imageResolution / parameters.FocalLength).ToString(CultureInfo.InvariantCulture));
                    streamWriter.Write(" ");
                    streamWriter.Write(parameters.RadialDistortion.K1.ToString(CultureInfo.InvariantCulture));
                    streamWriter.Write(" ");
                    streamWriter.WriteLine(parameters.RadialDistortion.K2.ToString(CultureInfo.InvariantCulture));

                    //  <R>             [a 3x3 matrix representing the camera rotation]

                    streamWriter.Write((-parameters.Rotation.Matrix[0, 0]).ToString(CultureInfo.InvariantCulture));
                    streamWriter.Write(" ");
                    streamWriter.Write((-parameters.Rotation.Matrix[0, 1]).ToString(CultureInfo.InvariantCulture));
                    streamWriter.Write(" ");
                    streamWriter.WriteLine((-parameters.Rotation.Matrix[0, 2]).ToString(CultureInfo.InvariantCulture));

                    streamWriter.Write(parameters.Rotation.Matrix[1, 0].ToString(CultureInfo.InvariantCulture));
                    streamWriter.Write(" ");
                    streamWriter.Write(parameters.Rotation.Matrix[1, 1].ToString(CultureInfo.InvariantCulture));
                    streamWriter.Write(" ");
                    streamWriter.WriteLine(parameters.Rotation.Matrix[1, 2].ToString(CultureInfo.InvariantCulture));

                    streamWriter.Write((-parameters.Rotation.Matrix[2, 0]).ToString(CultureInfo.InvariantCulture));
                    streamWriter.Write(" ");
                    streamWriter.Write((-parameters.Rotation.Matrix[2, 1]).ToString(CultureInfo.InvariantCulture));
                    streamWriter.Write(" ");
                    streamWriter.WriteLine((-parameters.Rotation.Matrix[2, 2]).ToString(CultureInfo.InvariantCulture));

                    //  <t>             [a 3-vector describing the camera translation]
                    streamWriter.Write(parameters.Position.X.ToString(CultureInfo.InvariantCulture));
                    streamWriter.Write(" ");
                    streamWriter.Write(parameters.Position.Y.ToString(CultureInfo.InvariantCulture));
                    streamWriter.Write(" ");
                    streamWriter.WriteLine(parameters.Position.Z.ToString(CultureInfo.InvariantCulture));


                    // matrix
                    //http://photosynth.net/view.aspx?cid=84daebb1-557f-48f6-b88d-a8566849e88c

  
                }
            }
        }

    }
}
