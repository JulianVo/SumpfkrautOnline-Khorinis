using System;
using RP_Server_Scripts.Authentication.Properties;
using RP_Server_Scripts.Chat;

namespace RP_Server_Scripts.Authentication.ChatCommands
{
    internal class LogOutChatCommand : IChatCommand
    {
        private readonly AuthenticationService _AuthenticationService;

        public LogOutChatCommand(AuthenticationService authenticationService)
        {
            _AuthenticationService = authenticationService ?? throw new ArgumentNullException(nameof(authenticationService));
        }


        public string[] Identifiers => new[] { "logout", "lo" };
        public void HandleCommand(Client.Client sender, string[] parameter)
        {
            if (sender == null)
            {
                throw new ArgumentNullException(nameof(sender));
            }

            _AuthenticationService.LogoutClient(sender);
            sender.SendServerNotification(Resources.Msg_YouHaveBeenLoggedOut);
        }

        public string DescriptionText => Resources.Msg_LogoutCommand_Description;
        public string UsageText => "/logout";
    }
}
