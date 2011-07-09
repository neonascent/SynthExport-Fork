using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using NetServ.Net.Json;
using System.Diagnostics;




namespace json_parse
{

    class Program
    {

        static void Main(string[] args)
        {

            JsonObject jsonOb;
            JsonObject Joimage;

            JsonObject Jor;

            JsonObject Jogroup;

            string json = File.ReadAllText(@"0.json");

            using (JsonParser parser = new JsonParser(new StringReader(json), true))

                jsonOb = parser.ParseObject();

            JsonObject Jo1 = (JsonObject)jsonOb["l"];

            JsonObject Jo2 = (JsonObject)Jo1.First().Value;

            JsonObject Jox = (JsonObject)Jo2["x"];

            string CSVoutput = "group,image,p0,p1,p2,j0,j1,j2,j3,j4,j5,j6,j7,j8,eulerX,eulerY,eulerZ\n";

            foreach (string groupKey in Jox.Keys)
            {

                Jogroup = (JsonObject)Jox[groupKey];

                Jor = (JsonObject)Jogroup["r"];

                foreach (string ObKey in Jor.Keys)
                {

                    CSVoutput += groupKey + "," + ObKey;

                    Joimage = (JsonObject)Jor[ObKey];

                    foreach (JsonNumber coord in (JsonArray)Joimage["p"])

                        CSVoutput += "," + coord.Value.ToString();



                    // start simeon's changes 

                    //int jCount = ((JsonArray)Joimage["j"]).Count;
                    //JsonNumber num;
                    //for (int i = 0; i <= 3; i++) {
                    //    num = (JsonNumber)((JsonArray)Joimage["j"])[i];
                    //    CSVoutput += "," + num.Value.ToString();
                    //}



                    //for (int i = 7; i <= 8; i++) {
                    //    num = (JsonNumber)((JsonArray)Joimage["j"])[i];
                    //    CSVoutput += "," + num.Value.ToString();
                    //}
                    //
                    // end simeon's changes 

                    foreach (JsonNumber coord in (JsonArray)Joimage["j"])

                        CSVoutput += "," + coord.Value.ToString();

                    // start simeon's euler values
                    // use the quat.exe <float> <float> <float>
                    JsonNumber qNum;
                    qNum = (JsonNumber)((JsonArray)Joimage["j"])[4];
                    double qx = qNum.Value;
                    qNum = (JsonNumber)((JsonArray)Joimage["j"])[5];
                    double qy = qNum.Value;
                    qNum = (JsonNumber)((JsonArray)Joimage["j"])[6];
                    double qz = qNum.Value;

                    Process p = new Process();
                    p.StartInfo.UseShellExecute = false;
                    p.StartInfo.RedirectStandardOutput = true;
                    p.StartInfo.WorkingDirectory = "./";
                    p.StartInfo.FileName = "quat.exe";
                    p.StartInfo.Arguments = qx + " " + qy + " " + qz;
                    p.Start();
                    p.WaitForExit();
                    string eulerRot = p.StandardOutput.ReadToEnd();
                    CSVoutput += "," + eulerRot;
                    // end simeon's euler values

                    CSVoutput += "\n";

                }

            }

            File.WriteAllText(@"json.csv", CSVoutput);

        }



        static double[] unitQuaternionToXYZEuler(double qx, double qy, double qz)
        {

            double qw = Math.Sqrt(qx * qx + qy * qy + qz * qz);



            double[] q = { qx, qy, qz, qw };

            double xRot = Degrees(Math.Atan(

                (2 * (q[0] * q[3] + q[1] * q[2])) / (1 - (2 * (Math.Pow(q[2], 2) + Math.Pow(q[3], 2))))));

            double yRot = Degrees(Math.Asin(

                2 * (q[0] * q[2] - q[3] * q[1])));

            double zRot = Degrees(Math.Atan(

                (2 * (q[0] * q[1] + q[2] * q[3])) / (1 - (2 * (Math.Pow(q[1], 2) + Math.Pow(q[2], 2))))));



            double[] rot = { xRot, yRot, zRot };

            return rot;

        }



        static double Degrees(double rad)
        {

            return rad * (180 / Math.PI);

        }

    }
}