using GUC.WorldObjects.Instances;


namespace GUC.Scripts.Sumpfkraut.VobSystem.Definitions
{
    public partial class ProjDef : BaseVobDef, ProjectileInstance.IScriptProjectileInstance
    {
        partial void pConstruct();
        public ProjDef()
        {
            pConstruct();
        }

        protected override BaseVobInstance CreateVobInstance()
        {
            return new ProjectileInstance(this);
        }


        public override VobType VobType { get { return VobType.Projectile; } }

        new public ProjectileInstance BaseDef { get { return (ProjectileInstance)base.BaseDef; } }
    }
}
