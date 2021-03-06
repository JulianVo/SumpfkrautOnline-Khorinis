﻿using GUC.Network;

namespace GUC.Scripts.Sumpfkraut.VobSystem.Definitions.Mobs
{
    public partial class MobDef : NamedVobDef, WorldObjects.Instances.MobInstance.IScriptMobInstance
    {
        #region Properties 

        public override VobType VobType { get { return VobType.Mob; } }

        // Zugriff auf BasisKlasse (MobInstance = MobDef)
        new public WorldObjects.Instances.MobInstance BaseDef { get { return (WorldObjects.Instances.MobInstance)base.BaseDef; } }
        
        public string FocusName { get { return this.BaseDef.FocusName; } set { this.BaseDef.FocusName = value; } }

        #endregion

        #region Constructors

        public MobDef()
        {
        }
        
        protected override WorldObjects.Instances.BaseVobInstance CreateVobInstance()
        {
            return new WorldObjects.Instances.MobInstance(this);
        }

        #endregion

        #region Read & Write

        public override void OnReadProperties(PacketReader stream)
        {
            base.OnReadProperties(stream);
        }

        public override void OnWriteProperties(PacketWriter stream)
        {
            base.OnWriteProperties(stream);
        }

        #endregion
    }
}
