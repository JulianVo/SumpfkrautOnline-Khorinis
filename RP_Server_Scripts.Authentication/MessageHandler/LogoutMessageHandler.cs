using System;
using System.Threading.Tasks;
using GUC;
using GUC.Network;
using RP_Server_Scripts.Network;
using RP_Shared_Script;

namespace RP_Server_Scripts.Authentication.MessageHandler
{
    internal sealed class LogoutMessageHandler : IScriptMessageHandler
    {
        private readonly AuthenticationService _AuthenticationService;

        public LogoutMessageHandler(AuthenticationService authenticationService)
        {
            _AuthenticationService = authenticationService ?? throw new ArgumentNullException(nameof(authenticationService));
        }

        public void HandleMessage(Client.Client sender, PacketReader stream)
        {
            if (sender == null)
            {
                throw new ArgumentNullException(nameof(sender));
            }

            if (stream == null)
            {
                throw new ArgumentNullException(nameof(stream));
            }

            _AuthenticationService.LogoutClient(sender);
        }

        public ScriptMessages SupportedMessage => ScriptMessages.Logout;
    }
}
