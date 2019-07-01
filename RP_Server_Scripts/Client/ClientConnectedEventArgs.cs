using System;

namespace RP_Server_Scripts.Client
{
    public sealed class ClientConnectedEventArgs : EventArgs
    {
        public ClientConnectedEventArgs(Client client)
        {
            Client = client ?? throw new ArgumentNullException(nameof(client));
        }

        public Client Client { get; }
    }
}
