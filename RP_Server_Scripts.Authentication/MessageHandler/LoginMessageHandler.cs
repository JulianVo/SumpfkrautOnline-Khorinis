using System;
using System.Threading.Tasks;
using GUC;
using GUC.Network;
using RP_Server_Scripts.Authentication.Login;
using RP_Server_Scripts.Logging;
using RP_Server_Scripts.Network;
using RP_Shared_Script;

namespace RP_Server_Scripts.Authentication.MessageHandler
{
    internal sealed class LoginMessageHandler : IScriptMessageHandler
    {
        private readonly AuthenticationService _AuthenticationService;
        private readonly IPacketWriterPool _PacketWriterPool;
        private readonly ILogger _Logger;

        internal LoginMessageHandler(AuthenticationService authenticationService, ILoggerFactory loggerFactory, IPacketWriterPool packetWriterPool)
        {
            if (loggerFactory == null)
            {
                throw new ArgumentNullException(nameof(loggerFactory));
            }

            _AuthenticationService = authenticationService ?? throw new ArgumentNullException(nameof(authenticationService));
            _PacketWriterPool = packetWriterPool ?? throw new ArgumentNullException(nameof(packetWriterPool));
            _Logger = loggerFactory.GetLogger(GetType());
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
                LoginResult result = await _AuthenticationService.LoginClientAsync(sender, userName, password);
                if (result.Successful)
                {
                    using (var writer = _PacketWriterPool.GetScriptMessageStream(ScriptMessages.LoginAcknowledge))
                    {
                        sender.SendScriptMessage(writer, NetPriority.High, NetReliability.ReliableOrdered);
                    }
                }
                else
                {
                    if (result is LoginFailedResult failedResult)
                    {
                        using (var writer = _PacketWriterPool.GetScriptMessageStream(ScriptMessages.LoginDenied))
                        {
                            writer.Write((byte)failedResult.Reason);
                            writer.Write(failedResult.ReasonText);
                            sender.SendScriptMessage(writer, NetPriority.High, NetReliability.ReliableOrdered);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                _Logger.Error($"An exception occured while trying to handle a '{SupportedMessage}' script message. Exception: {e}");
            }
        }

        public ScriptMessages SupportedMessage => ScriptMessages.Login;
    }
}
