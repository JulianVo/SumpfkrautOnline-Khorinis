using System;
using RP_Server_Scripts.Authentication.Login;
using RP_Server_Scripts.Authentication.Properties;
using RP_Server_Scripts.Chat;

namespace RP_Server_Scripts.Authentication.ChatCommands
{
    internal class RegisterChatCommand : IChatCommand
    {
        private readonly AuthenticationService _AuthenticationService;

        public RegisterChatCommand(AuthenticationService authenticationService)
        {
            _AuthenticationService = authenticationService ?? throw new ArgumentNullException(nameof(authenticationService));
        }

        public string[] Identifiers => new[] {"register","r"};
        public async void HandleCommand(Client.Client sender, string[] parameter)
        {
            if (parameter.Length != 2)
            {
                sender.SendServerNotification(string.Format(Resources.Msg_CommandRequiresNArguments, Identifiers, 2));
            }

            string userName = parameter[0];
            string password = parameter[1];

            LoginResult result = await _AuthenticationService.CreateAndLoginAccountAsync(sender, userName, password);

            if (result is LoginSuccessfulResult successfulResult)
            {
                sender.SendServerNotification(Resources.Msg_AccountSuccessfullyCreated);
                sender.SendServerNotification(string.Format(Resources.Msg_LoginSuccessful, successfulResult.Session.Account.UserName));
            }
            else
            {
                sender.SendServerNotification(Resources.Msg_AccountCreationFailed);
            }

        }

        public string DescriptionText => Resources.Msg_RegisterChatCommand_Description;
        public string UsageText => "/register <username> <password>";
    }
}
