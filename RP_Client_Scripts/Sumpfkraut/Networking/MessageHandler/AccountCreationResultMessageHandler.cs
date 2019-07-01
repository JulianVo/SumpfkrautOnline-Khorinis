using System;
using GUC.Network;
using GUC.Scripts.Logging;
using RP_Shared_Script;

namespace GUC.Scripts.Sumpfkraut.Networking
{
    internal sealed class AccountCreationResultMessageHandler : IScriptMessageHandler
    {
        private readonly ILogger _Log;

        public event GenericEventHandler<AccountCreationResultMessageHandler, AccountCreationResultMessage> AccountCreationResultReceived;

        public AccountCreationResultMessageHandler(ILoggerFactory loggerFactory)
        {
            if (loggerFactory == null)
            {
                throw new ArgumentNullException(nameof(loggerFactory));
            }

            _Log = loggerFactory.GetLogger(GetType());
        }

        public void HandleMessage(ScriptClient sender, PacketReader stream)
        {
            if (sender == null)
            {
                throw new ArgumentNullException(nameof(sender));
            }

            if (stream == null)
            {
                throw new ArgumentNullException(nameof(stream));
            }

            try
            {
                bool success = stream.ReadBit();
                if (success)
                {
                    AccountCreationResultReceived?.Invoke(this, new AccountCreationResultMessage(true, string.Empty));
                }
                else
                {
                    string reasonText = stream.ReadString();
                    AccountCreationResultReceived?.Invoke(this, new AccountCreationResultMessage(false, reasonText));
                }
            }
            catch (Exception e)
            {
                _Log.Error($"Something went wrong while handling a script message of type '{SupportedMessage}'. Exception: {e}");
            }
        }

        public ScriptMessages SupportedMessage => ScriptMessages.AccountCreationResult;
    }

    internal class AccountCreationResultMessage
    {
        public AccountCreationResultMessage(bool success, string reasonText)
        {
            Success = success;
            ReasonText = reasonText ?? throw new ArgumentNullException(nameof(reasonText));
        }

        public bool Success { get; }
        public string ReasonText { get; }
    }
}
