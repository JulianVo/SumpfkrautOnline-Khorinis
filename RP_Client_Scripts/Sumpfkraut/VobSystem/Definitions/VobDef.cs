﻿using GUC.WorldObjects.Instances;
using GUC.Scripts.Sumpfkraut.Visuals;


namespace GUC.Scripts.Sumpfkraut.VobSystem.Definitions
{
    public partial class VobDef : BaseVobDef, VobInstance.IScriptVobInstance
    {
        partial void pConstruct();
        public VobDef()
        {
            pConstruct();
        }


        protected override BaseVobInstance CreateVobInstance()
        {
            return new VobInstance(this);
        }

        public override VobType VobType { get { return VobType.Vob; } }

        new public VobInstance BaseDef { get { return (VobInstance)base.BaseDef; } }
        
        public ModelDef Model
        {
            get { return (ModelDef)this.BaseDef.ModelInstance.ScriptObject; }
            set { this.BaseDef.ModelInstance = value.BaseDef; }
        }

        public bool CDDyn { get { return BaseDef.CDDyn; } set { BaseDef.CDDyn = value; } }
        public bool CDStatic { get { return BaseDef.CDStatic; } set { BaseDef.CDStatic = value; } }
    }
}
