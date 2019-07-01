using System;
using GUC.Scripts.Sumpfkraut.Networking;
using RP_Shared_Script;

namespace GUC.Scripts.Account
{
    internal sealed class AccountCreation
    {
        private readonly AccountCreationMessageWriter _CreationMessageWriter;
        private readonly ScriptMessageSender _ScriptMessageSender;

        public event GenericEventHandler<AccountCreation> AccountCreationSuccessful;

        public event GenericEventHandler<AccountCreation, AccountCreationFailedArgs> AccountCreationFailed;

        public AccountCreation(AccountCreationMessageWriter creationMessageWriter, ScriptMessageSender scriptMessageSender, AccountCreationResultMessageHandler handler)
        {
            if (handler == null)
            {
                throw new ArgumentNullException(nameof(handler));
            }

            _CreationMessageWriter = creationMessageWriter ?? throw new ArgumentNullException(nameof(creationMessageWriter));
            _ScriptMessageSender = scriptMessageSender ?? throw new ArgumentNullException(nameof(scriptMessageSender));

            handler.AccountCreationResultReceived += HandlerOnAccountCreationResultReceived;
        }

        private void HandlerOnAccountCreationResultReceived(AccountCreationResultMessageHandler sender, AccountCreationResultMessage args)
        {
            if (args.Success)
            {
                AccountCreationSuccessful?.Invoke(this);
            }
            else
            {
                AccountCreationFailed?.Invoke(this, new AccountCreationFailedArgs(args.ReasonText));
            }
        }

        public void StartAccountCreation(string username, string password)
        {
            if (string.IsNullOrWhiteSpace(username))
            {
                throw new ArgumentException(@"Value cannot be null or whitespace.", nameof(username));
            }

            if (string.IsNullOrWhiteSpace(password))
            {
                throw new ArgumentException(@"Value cannot be null or whitespace.", nameof(password));
            }

            var packet = _CreationMessageWriter.Write(username, password);
            _ScriptMessageSender.SendScriptMessage(packet, NetPriority.Medium, NetReliability.ReliableOrdered);
        }
    }
}
