using System;

namespace RP_Server_Scripts.Authentication.Login
{
    public sealed class LoginSuccessfulResult:LoginResult
    {
        public LoginSuccessfulResult(Session session) : base(true)
        {
            Session = session ?? throw new ArgumentNullException(nameof(session));
        }

        public Session Session { get; }
    }
}
