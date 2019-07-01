namespace RP_Server_Scripts.Network
{
    public interface IScriptMessageHandlerSelector
    {
        bool TryGetMessageHandler(RP_Shared_Script.ScriptMessages message, out IScriptMessageHandler handler);
    }
}
