using System;
using System.ComponentModel;
using RP_Shared_Script;
using RP_Shared_Script.Login;

namespace GUC.Scripts.Sumpfkraut.Networking
{
    internal sealed class LoginFailedArgs
    {
        public LoginFailedArgs(LoginFailedReason reason, string reasonText)
        {
            if (!Enum.IsDefined(typeof(LoginFailedReason), reason))
            {
                throw new InvalidEnumArgumentException(nameof(reason), (int)reason, typeof(LoginFailedReason));
            }

            Reason = reason;
            ReasonText = reasonText ?? throw new ArgumentNullException(nameof(reasonText));
        }

        public LoginFailedReason Reason { get; }
        public string ReasonText { get; }
    }
}
