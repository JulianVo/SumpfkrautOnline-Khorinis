using System;
using System.Threading.Tasks;
using GUC;
using GUC.Network;
using RP_Server_Scripts.Logging;
using RP_Server_Scripts.Network;
using RP_Shared_Script;

namespace RP_Server_Scripts.Authentication.MessageHandler
{
    internal sealed class CreateAccountMessageHandler : IScriptMessageHandler
    {
        private readonly AuthenticationService _AuthenticationService;
        private readonly IPacketWriterPool _PacketWriterPool;
        private readonly ILogger _Log;

        public CreateAccountMessageHandler(AuthenticationService authenticationService, ILoggerFactory loggerFactory, IPacketWriterPool packetWriterPool)
        {
            if (loggerFactory == null)
            {
                throw new ArgumentNullException(nameof(loggerFactory));
            }

            _AuthenticationService = authenticationService ?? throw new ArgumentNullException(nameof(authenticationService));
            _PacketWriterPool = packetWriterPool ?? throw new ArgumentNullException(nameof(packetWriterPool));
            _Log = loggerFactory.GetLogger(GetType());
        }

        public async void HandleMessage(Client.Client sender, PacketReader stream)
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
                string userName = stream.ReadString();
                string password = stream.ReadString();

                AccountCreationResult result =
                    await _AuthenticationService.CreateAccountAsync(userName, password);

                if (result is AccountCreationSuccessful successfulResult)
                {
                    using (var writer = _PacketWriterPool.GetScriptMessageStream(ScriptMessages.AccountCreationResult))
                    {
                        writer.Write(true);
                        sender.SendScriptMessage(writer, NetPriority.Medium, NetReliability.ReliableOrdered);
                    }

                }
                else if (result is AccountCreationFailed failedResult)
                {
                    using (var writer = _PacketWriterPool.GetScriptMessageStream(ScriptMessages.AccountCreationResult))
                    {
                        writer.Write(false);
                        writer.Write(failedResult.ReasonText);
                        sender.SendScriptMessage(writer, NetPriority.Medium, NetReliability.ReliableOrdered);
                    }
                }
            }
            catch (Exception e)
            {
                _Log.Error($"Something went wrong while trying to handle a '{SupportedMessage}' script message. Exception: {e}");
            }
        }

        public ScriptMessages SupportedMessage => ScriptMessages.CreateAccount;
    }
}
