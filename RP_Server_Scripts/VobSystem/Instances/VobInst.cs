using GUC.Types;
using GUC.WorldObjects;
using RP_Server_Scripts.Visuals;
using RP_Server_Scripts.VobSystem.Definitions;

namespace RP_Server_Scripts.VobSystem.Instances
{
    public class VobInst : BaseVobInst, Vob.IScriptVob
    {
        public VobInst(VobDef def):base(def)
        {
        }


        protected override BaseVob CreateVob()
        {
            return new Vob(new ModelInst(this), this);
        }


        public override VobType VobType => VobType.Vob;

        public new Vob BaseInst => (Vob)base.BaseInst;

        public ModelInst ModelInst => (ModelInst)this.BaseInst.Model.ScriptObject;

        public new VobDef Definition { get => (VobDef)base.Definition;
            set => base.Definition = value;
        }
        public ModelDef ModelDef => this.Definition.Model;

        public void Throw(Vec3f velocity)
        {
            this.BaseInst.Throw(velocity);
        }
    }
}
