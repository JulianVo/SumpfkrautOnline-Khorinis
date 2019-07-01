using System;
using RP_Server_Scripts.Authentication.Properties;
using RP_Server_Scripts.Chat;

namespace RP_Server_Scripts.Authentication.ChatCommands
{
    internal class LoginStateChatCommand : IChatCommand
    {
        private readonly AuthenticationService _AuthenticationService;

        internal LoginStateChatCommand(AuthenticationService authenticationService)
        {
            _AuthenticationService = authenticationService ?? throw new ArgumentNullException(nameof(authenticationService));
        }

        public string[] Identifiers => new[] { "loginstate", "ls" };

        public void HandleCommand(Client.Client sender, string[] parameter)
        {
            if (_AuthenticationService.TryGetSession(sender, out Session session))
            {
                sender.SendServerNotification(string.Format(Resources.Msg_LoggedIn, session.Account.UserName));
            }
            else
            {
                sender.SendServerNotification(Resources.Msg_NotLoggedIn);
            }
        }

        public string DescriptionText => Resources.Msg_Command_LoginState_Description;
        public string UsageText => @"/loginstate";
    }
}
