using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RP_Shared_Script;

namespace GUC.Scripts
{
    public sealed class GameState
    {
        public GameState()
        {
            GUCScripts.OnWorldEnter += () => WorldEntered?.Invoke(this);
        }

        public event GenericEventHandler<GameState> WorldEntered;
    }
}
