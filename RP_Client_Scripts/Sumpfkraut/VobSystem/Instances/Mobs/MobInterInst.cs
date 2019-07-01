using GUC.Network;
using GUC.Scripts.Sumpfkraut.VobSystem.Definitions.Mobs;
using GUC.Scripts.Sumpfkraut.Visuals;

namespace GUC.Scripts.Sumpfkraut.VobSystem.Instances.Mobs
{
    public  partial class MobInterInst : MobInst, WorldObjects.MobInter.IScriptMobInter
    {
        #region Constructors

        public MobInterInst()
        {
        }

        protected override WorldObjects.BaseVob CreateVob()
        {
            return new WorldObjects.MobInter(new ModelInst(this), this);
        }


        #endregion

        #region Properties

        public override VobType VobType { get { return VobType.MobInter; } }

        new public WorldObjects.MobInter BaseInst { get { return (WorldObjects.MobInter)base.BaseInst; } }
        new public MobInterDef Definition { get { return (MobInterDef)base.Definition; } set { base.Definition = value; } }

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
