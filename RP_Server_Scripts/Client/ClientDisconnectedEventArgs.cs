using System;

namespace RP_Server_Scripts.Client
{
    public sealed class ClientDisconnectedEventArgs : EventArgs
    {
        public ClientDisconnectedEventArgs(Client client)
        {
            Client = client ?? throw new ArgumentNullException(nameof(client));
        }

        public Client Client { get; }
    }
}
