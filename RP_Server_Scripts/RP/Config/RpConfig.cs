namespace GUC.Scripts.RP.Config
{
    class RpConfig
    {
        public Point3F DefaultSpawnPoint { get; set; }

        public static RpConfig Default => new RpConfig
        {
            DefaultSpawnPoint = new Point3F(0,0,50)
        };
    }
}
