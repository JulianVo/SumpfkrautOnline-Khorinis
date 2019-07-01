namespace GUC.Scripts.Sumpfkraut.Networking
{
    public interface IScriptMessageHandlerSelector
    {
        bool TryGetMessageHandler(RP_Shared_Script.ScriptMessages message, out IScriptMessageHandler handler);
    }
}
