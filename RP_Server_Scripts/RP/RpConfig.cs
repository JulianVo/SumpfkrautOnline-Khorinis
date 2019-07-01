using System;
using GUC.Types;

namespace RP_Server_Scripts.RP
{
    public sealed class RpConfig
    {
        private readonly object _Lock = new object();

        private string _DefaultSpawnWorld = "NEWWORLD/NEWWORLD.ZEN";
        private Vec3f _DefaultSpawnPoint = new Vec3f(0, 0, 50);
        private Angles _DefaultSpawnRotation= new Angles(0,0,0);

        public Vec3f DefaultSpawnPoint
        {
            get
            {
                lock (_Lock)
                {
                    return _DefaultSpawnPoint;
                }
            }
            set
            {
                lock (_Lock)
                {
                    _DefaultSpawnPoint = value;
                }
            }
        }

        public string DefaultSpawnWorld
        {
            get
            {
                lock (_Lock)
                {
                    return _DefaultSpawnWorld;
                }
            }
            set
            {
                lock (_Lock)
                {
                    if (string.IsNullOrWhiteSpace(value))
                    {
                        throw new ArgumentException(@"The default spawn world name must not be null or white space", nameof(value));
                    }
                    _DefaultSpawnWorld = value;
                }
            }
        }

        public Angles DefaultSpawnRotation
        {
            get
            {
                lock (_Lock)
                {
                    return _DefaultSpawnRotation;
                }
            }
            set
            {
                lock (_Lock)
                {
                    _DefaultSpawnRotation = value;
                }
            }
        }

        public int MaxCharacterPerAccount => 5;

        public static RpConfig Default => new RpConfig();
    }
}
