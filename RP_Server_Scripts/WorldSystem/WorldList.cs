using System.Collections.Generic;
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
    }
}
