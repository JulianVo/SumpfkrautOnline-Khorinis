using GUC.WorldObjects;
using RP_Server_Scripts.Visuals;
using RP_Server_Scripts.VobSystem.Definitions.Mobs;

namespace RP_Server_Scripts.VobSystem.Instances.Mobs
{
    public class MobInterInst : MobInst, MobInter.IScriptMobInter
    {
        protected override BaseVob CreateVob()
        {
            return new MobInter(new ModelInst(this), this);
        }

        public override VobType VobType => VobType.MobInter;

        public new MobInter BaseInst => (MobInter) base.BaseInst;

        public new MobInterDef Definition
        {
            get => (MobInterDef) base.Definition;
            set => base.Definition = value;
        }


        public MobInterInst(MobInterDef def) :base(def)
        {
            Definition = def;
        }
    }
}