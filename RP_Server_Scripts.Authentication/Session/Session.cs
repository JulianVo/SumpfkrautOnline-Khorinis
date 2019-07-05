using System;
using RP_Shared_Script;

namespace RP_Server_Scripts.Authentication
{
    public sealed class Session
    {
        public event GenericEventHandler<Session, LogoutEventArgs> LoggedOut;

        public Session(Client.Client client, Account account)
        {
            Client = client ?? throw new ArgumentNullException(nameof(client));
            Account = account ?? throw new ArgumentNullException(nameof(account));
            ClientId = Client.Id;
        }

        public Client.Client Client { get; }
        public Account Account { get; }

        public bool IsValid { get; private set; }

        public int ClientId { get; private set; }

        internal void Invalidate()
        {
            LoggedOut?.Invoke(this, new LogoutEventArgs(Client, Account));
            IsValid = false;
            ClientId = -1;
        }
    }
}
