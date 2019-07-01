using GUC.Scripts.Sumpfkraut.Visuals;
using GUC.WorldObjects;
using GUC.Scripts.Sumpfkraut.VobSystem.Definitions;
using GUC.Types;


namespace GUC.Scripts.Sumpfkraut.VobSystem.Instances
{
    public partial class VobInst : BaseVobInst, Vob.IScriptVob
    {
        partial void pConstruct();
        public VobInst()
        {
            pConstruct();
        }

        protected override BaseVob CreateVob()
        {
            return new Vob(new ModelInst(this), this);
        }


        public override VobType VobType { get { return VobType.Vob; } }


        public new Vob BaseInst { get { return (Vob)base.BaseInst; } }

        public ModelInst ModelInst { get { return (ModelInst)this.BaseInst.Model.ScriptObject; } }

        new public VobDef Definition { get { return (VobDef)base.Definition; } set { base.Definition = value; } }
        public ModelDef ModelDef { get { return this.Definition.Model; } }

        public void Throw(Vec3f velocity)
        {
            this.BaseInst.Throw(velocity);
        }
    }
}
