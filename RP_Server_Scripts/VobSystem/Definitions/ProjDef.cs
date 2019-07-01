using GUC.WorldObjects.Instances;

namespace RP_Server_Scripts.VobSystem.Definitions
{
    public class ProjDef : BaseVobDef, ProjectileInstance.IScriptProjectileInstance
    {
        public override VobType VobType => VobType.Projectile;

        public new ProjectileInstance BaseDef => (ProjectileInstance) base.BaseDef;

        public ProjDef(string codeName, IBaseDefFactory baseDefFactory,  IVobDefRegistration registration) : base(codeName, baseDefFactory, registration)
        {
        }
    }
}