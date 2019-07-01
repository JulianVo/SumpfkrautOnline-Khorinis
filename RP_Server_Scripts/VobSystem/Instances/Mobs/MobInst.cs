using GUC.WorldObjects;
using RP_Server_Scripts.Visuals;
using RP_Server_Scripts.VobSystem.Definitions.Mobs;

namespace RP_Server_Scripts.VobSystem.Instances.Mobs
{
    public class MobInst : NamedVobInst, Mob.IScriptMob
    {
        protected override BaseVob CreateVob()
        {
            return new Mob(new ModelInst(this), this);
        }

        public override VobType VobType => VobType.Mob;

        public new Mob BaseInst => (Mob) base.BaseInst;

        public new MobDef Definition
        {
            get => (MobDef) base.Definition;
            set => base.Definition = value;
        }

        public MobInst(MobDef def) : base(def)
        {
            Definition = def;
        }
    }
}