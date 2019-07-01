using System;

namespace RP_Server_Scripts.Authentication
{   
    public sealed class LoginEventArgs
    {
        public LoginEventArgs(Session session)
        {
            Session = session ?? throw new ArgumentNullException(nameof(session));
        }

        public Session Session { get; }
    }
}
