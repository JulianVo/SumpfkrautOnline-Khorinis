﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GUC.Enumeration
{
    public enum MoveState
    {
        Stand,

        Forward,
        Backward,
        Left,
        Right,

        Falling
    }

    public enum NPCState
    {
        None,
        InWater,
        Swimming,
        Diving,

        InAir,
    }
}
