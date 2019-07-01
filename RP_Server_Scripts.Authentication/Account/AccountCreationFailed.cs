using System;

namespace RP_Server_Scripts.Authentication
{
    public sealed class AccountCreationFailed : AccountCreationResult
    {
        public AccountCreationFailed(string reasonText) : base(false)
        {
            if (string.IsNullOrWhiteSpace(reasonText))
            {
                throw new ArgumentException(@"Value cannot be null or whitespace.", nameof(reasonText));
            }

            ReasonText = reasonText;
        }

        public string ReasonText { get; }
    }
}