namespace SynthExport
{
    public class CoordinateSystem
    {
        public int ID { get; private set; }
        public bool ShouldBeExported { get; set; }
        public PointCloud PointCloud { get; set; }
        public CameraParameterList CameraParameterList { get; set; }

        public CoordinateSystem(int id)
        {
            ID = id;
        }
    }
}
