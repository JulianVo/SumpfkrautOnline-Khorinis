using GUC.WorldObjects;
using GUC.Scripts.Sumpfkraut.VobSystem.Definitions;

namespace GUC.Scripts.Sumpfkraut.VobSystem.Instances
{

    public abstract class NamedVobInst : VobInst, Vob.IScriptVob
    {
        public new NamedVobDef Definition
        {
            get => (NamedVobDef)base.Definition;
            set => base.Definition = value;
        }

        public string Name => this.Definition.Name;
    }

}
