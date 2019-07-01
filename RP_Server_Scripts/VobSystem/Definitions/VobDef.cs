using System.Diagnostics;
using GUC.WorldObjects.Instances;
using RP_Server_Scripts.Visuals;

namespace RP_Server_Scripts.VobSystem.Definitions
{
    [DebuggerDisplay("VobDef:{CodeName}")]
    public class VobDef : BaseVobDef, VobInstance.IScriptVobInstance
    {
        public override VobType VobType => VobType.Vob;

        public new VobInstance BaseDef => (VobInstance) base.BaseDef;

        public ModelDef Model
        {
            get => (ModelDef) BaseDef.ModelInstance.ScriptObject;
            set => BaseDef.ModelInstance = value.BaseDef;
        }

        public bool CDDyn
        {
            get => BaseDef.CDDyn;
            set => BaseDef.CDDyn = value;
        }

        public bool CDStatic
        {
            get => BaseDef.CDStatic;
            set => BaseDef.CDStatic = value;
        }

        public VobDef(string codeName, IBaseDefFactory baseDefFactory,   IVobDefRegistration registration) : base(codeName,baseDefFactory,  registration)
        {
        }
    }
}