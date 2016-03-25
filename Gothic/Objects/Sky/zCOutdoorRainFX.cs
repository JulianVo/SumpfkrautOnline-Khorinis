﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WinApi;

namespace Gothic.Objects.Sky
{
    public class zCOutdoorRainFX : zClass
    {
        public zCOutdoorRainFX(int address)
            : base(address)
        {
        }

        public zCOutdoorRainFX()
        {
        }

        public abstract class VarOffsets
        {
            public const int numFlyParticles = 0xE000,
            numImpactParticles = 0xE004,
            numDestParticles = 0xE018,
            freeFlyParticleList = 0xE008;
        }

        public int NumFlyParticles
        {
            get { return Process.ReadInt(Address + VarOffsets.numFlyParticles); }
        }

        public int NumImpactParticles
        {
            get { return Process.ReadInt(Address + VarOffsets.numImpactParticles); }
        }

        public int NumDestParticles
        {
            get { return Process.ReadInt(Address + VarOffsets.numDestParticles); }
        }
    }
}
