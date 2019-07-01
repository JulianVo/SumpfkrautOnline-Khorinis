namespace RP_Server_Scripts.VobSystem.Definitions
{
    public interface IVobDefRegistration
    {
        void Register(BaseVobDef def);
        void Unregister(BaseVobDef def);
    }
}
