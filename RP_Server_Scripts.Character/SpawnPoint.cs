using System;
using GUC.Types;
using RP_Server_Scripts.WorldSystem;

namespace RP_Server_Scripts.Character
{
    public sealed class SpawnPoint
    {
        public SpawnPoint( WorldInst world,Vec3f point, Angles rotation)
        {
            World = world ?? throw new ArgumentNullException(nameof(world));
            Point = point;
            Rotation = rotation;
        }

        public Vec3f Point { get; }

        public Angles Rotation { get; }

        public WorldInst World { get; }
    }
}
