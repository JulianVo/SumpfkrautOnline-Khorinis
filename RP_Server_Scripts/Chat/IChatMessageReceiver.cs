using RP_Server_Scripts.ReusedClasses;

namespace RP_Server_Scripts.Chat
{
    internal interface IChatMessageReceiver
    {
        void ReceiveChatMessage(Client.Client sender, string message, ChatMode mode);
    }
}
