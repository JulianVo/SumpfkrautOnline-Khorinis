using System;

namespace RP_Server_Scripts.Authentication
{
    public sealed class LogoutEventArgs
    {
        public LogoutEventArgs(Client.Client client, Account account)
        {
            Client = client ?? throw new ArgumentNullException(nameof(client));
            Account = account ?? throw new ArgumentNullException(nameof(account));
        }

        public Client.Client Client { get; }
        public Account Account { get; }
    }
}
