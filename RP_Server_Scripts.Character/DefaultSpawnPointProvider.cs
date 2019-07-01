using System;
using System.Linq;
using RP_Server_Scripts.RP;
using RP_Server_Scripts.WorldSystem;

namespace RP_Server_Scripts.Character
{
    /// <summary>
    /// An implementation of <see cref="ISpawnPointProvider"/> that returns a <see cref="SpawnPoint"/> based up on the values in the active <see cref="RpConfig"/> instance.
    /// </summary>
    internal sealed class DefaultSpawnPointProvider : ISpawnPointProvider
    {
        private readonly WorldList _WorldList;
        private readonly RpConfig _Config;

        public DefaultSpawnPointProvider(WorldList worldList, RpConfig config)
        {
            _WorldList = worldList ?? throw new ArgumentNullException(nameof(worldList));
            _Config = config ?? throw new ArgumentNullException(nameof(config));
        }

        public SpawnPoint GetSpawnPoint()
        {
            //Get the world that is defined a the default spawn world in the rp config.
            var world = _WorldList.Worlds.FirstOrDefault(w => w.Path.Equals(_Config.DefaultSpawnWorld)) ??
                        throw new RpConfigurationException("A nonexistent world is defined a the default spawn world");


            return new SpawnPoint(world, _Config.DefaultSpawnPoint, _Config.DefaultSpawnRotation);
        }
    }
}