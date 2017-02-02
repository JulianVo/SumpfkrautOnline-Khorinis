﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GUC.Scripts.Sumpfkraut.EffectSystem.Enumeration
{

    public enum ChangeType : int
    {
        Undefined,
        
        // changes influencing their own effect-container
        Effect_Name_Set,

        // often synchronized attributes
        Vob_Name_Set,

        // event driven
        World_Clock_Time_Set,
        World_Clock_Rate_Set,
        World_Clock_IsRunning_Set,

        // crafting
    }

}
