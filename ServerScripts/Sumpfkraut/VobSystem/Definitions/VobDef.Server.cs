﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GUC.WorldObjects.Instances;

namespace GUC.Scripts.Sumpfkraut.VobSystem.Definitions
{
    public partial class VobDef
    {
        public VobDef(string codeName) : this(new VobInstance(), codeName)
        {
        }

        protected VobDef(VobInstance baseDef, string codeName) : base(baseDef, codeName)
        {
        }
    }
}
