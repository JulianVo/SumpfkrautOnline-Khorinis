using System;
using System.Collections.Generic;
using System.Linq;
using GUC.WorldObjects;


namespace RP_Server_Scripts.WorldSystem
{
    public sealed class WorldList
    {
        public IEnumerable<WorldInst> Worlds
        {
            get
            {
                var list = new List<World>();
                World.ForEach(world => list.Add(world));
                foreach (var world in list)
                {
                    yield return (WorldInst)world.ScriptObject;
                }
            }
        }

        public WorldInst GetWorldOrFallback(string worldFile)
        {
            if (string.IsNullOrWhiteSpace(worldFile))
            {
                throw new ArgumentException(@"Value cannot be null or whitespace.", nameof(worldFile));
            }

            return Worlds.FirstOrDefault(w => w.Path.Equals(worldFile)) ?? Worlds.First();
        }
    }
}
