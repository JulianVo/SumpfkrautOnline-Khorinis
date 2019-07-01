using System;
using GUC.Scripts.CleanAPI.Interface;

namespace GUC.Scripts.Arena
{
    internal interface IChat
    {
        void SendMessageToAll(string message);
        event EventHandler<ChatMessageEventArgs> ChatMessageReceived;
        event EventHandler<ChatCommandReceivedEventArgs> ChatCommandReceived;
    }
}