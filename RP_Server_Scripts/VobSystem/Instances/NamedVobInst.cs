using RP_Server_Scripts.VobSystem.Definitions;

namespace RP_Server_Scripts.VobSystem.Instances
{

    public abstract class NamedVobInst : VobInst
    {
        protected NamedVobInst(NamedVobDef def):base(def)
        {
            this.Definition = def;
        }



        public new NamedVobDef Definition { get => (NamedVobDef)base.Definition;
            set => base.Definition = value;
        }

        public string Name => this.Definition.Name;
    }

}
