using GUC.WorldObjects.Instances;

namespace RP_Server_Scripts.VobSystem.Definitions.Mobs
{
    public class MobInterDef : MobDef, MobInterInstance.IScriptMobInterInstance
    {
        public override VobType VobType => VobType.MobInter;

        // Zugriff auf BasisKlasse (MobInstance = MobDef)
        public new MobInterInstance BaseDef => (MobInterInstance) base.BaseDef;

        public MobInterDef(string codeName, IBaseDefFactory baseDefFactory,   IVobDefRegistration registration) : base(codeName, baseDefFactory,  registration)
        {
        }
    }
}