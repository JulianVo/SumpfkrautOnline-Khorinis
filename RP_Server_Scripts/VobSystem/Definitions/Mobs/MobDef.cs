using GUC.WorldObjects.Instances;

namespace RP_Server_Scripts.VobSystem.Definitions.Mobs
{
    public class MobDef : NamedVobDef, MobInstance.IScriptMobInstance
    {
        public override VobType VobType => VobType.Mob;

        // Zugriff auf BasisKlasse (MobInstance = MobDef)
        public new MobInstance BaseDef => (MobInstance) base.BaseDef;

        public string FocusName
        {
            get => BaseDef.FocusName;
            set => BaseDef.FocusName = value;
        }


        public MobDef(string codeName, IBaseDefFactory baseDefFactory,   IVobDefRegistration registration) : base(codeName, baseDefFactory,  registration)
        {
        }
    }
}