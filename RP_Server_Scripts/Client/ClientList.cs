using System;
using System.Collections.Generic;

namespace RP_Server_Scripts.Client
{
    /// <summary>
    /// Keeps track of all connected <see cref="IClient"/>s.
    /// Provides a list of all clients and events for connects/disconnects of clients.
    /// </summary>
    public sealed class ClientList 
    {
        private readonly List<Client> _Clients = new List<Client>();
        private readonly object _Lock = new object();

        public event EventHandler<ClientDisconnectedEventArgs> ClientDisconnected;

        public void ForEach(Action<Client> action)
        {
            foreach (var client in Clients)
            {
                action.Invoke(client);
            }
        }

        public event EventHandler<ClientConnectedEventArgs> ClientConnected;


        public void RegisterClient(Client client)
        {
            if (client == null)
            {
                throw new ArgumentNullException(nameof(client));
            }

            client.OnConnected += ClientOnOnConnected;
            client.OnDisconnected += ClientOnDisconnect;
        }

        public IList<Client> Clients
        {
            get
            {
                lock (_Lock)
                {
                    return new List<Client>(_Clients).AsReadOnly();
                }
            }
        }

        public int Count
        {
            get
            {
                lock (_Lock)
                {
                    return _Clients.Count;
                }
            }
        }

        private void ClientOnDisconnect(object sender, ClientDisconnectedEventArgs e)
        {
            lock (_Lock)
            {
                _Clients.Remove(e.Client);
            }

            ClientDisconnected?.Invoke(this, e);
        }

        private void ClientOnOnConnected(object sender, ClientConnectedEventArgs e)
        {
            lock (_Lock)
            {
                _Clients.Add(e.Client);
            }

            ClientConnected?.Invoke(this, e);
        }
    }
}