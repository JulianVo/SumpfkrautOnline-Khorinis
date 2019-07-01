using GUC.WorldObjects.Instances;

namespace RP_Server_Scripts.VobSystem.Definitions
{
    public interface IBaseDefFactory
    {
        BaseVobInstance Build(BaseVobInstance.IScriptBaseVobInstance scriptVob);
    }
}
