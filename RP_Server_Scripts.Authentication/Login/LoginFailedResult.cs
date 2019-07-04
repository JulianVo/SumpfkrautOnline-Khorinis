using System;
using RP_Shared_Script;
using RP_Shared_Script.Login;

namespace RP_Server_Scripts.Authentication.Login
{
    public sealed class LoginFailedResult:LoginResult
    {
        public LoginFailedResult(LoginFailedReason reason, string reasonText) : base(false)
        {
            Reason = reason;
            ReasonText = reasonText ?? throw new ArgumentNullException(nameof(reasonText));
        }

        public LoginFailedReason Reason { get; }

        public string ReasonText { get; }
    }
}
