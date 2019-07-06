using System;
using GUC.Scripts.Sumpfkraut.Networking;
using RP_Shared_Script;
using RP_Shared_Script.Login;

namespace GUC.Scripts.Login
{
    internal sealed class Login
    {
        private readonly LoginPacketWriter _LoginPacketWriter;
        private readonly ScriptMessageSender _MessageSender;

        public event GenericEventHandler<Login, LoginDeniedArgs> LoginDenied;
        public event GenericEventHandler<Login, LoginAcknowledgedArgs> LoginAcknowledged;
        public event GenericEventHandler<Login> LoggedOut;

        public Login(LoginPacketWriter loginPacketWriter, ScriptMessageSender messageSender, LoginDeniedMessageHandler deniedMessageHandler, LoginAcknowledgedMessageHandler loginAcknowledgedMessage,LogoutAcknowledgeMessageHandler logoutMessageHandler)
        {
            if (deniedMessageHandler == null)
            {
                throw new ArgumentNullException(nameof(deniedMessageHandler));
            }

            if (loginAcknowledgedMessage == null)
            {
                throw new ArgumentNullException(nameof(loginAcknowledgedMessage));
            }

            if (logoutMessageHandler == null)
            {
                throw new ArgumentNullException(nameof(logoutMessageHandler));
            }

            deniedMessageHandler.LoginFailureReceived += DeniedMessageHandlerOnLoginFailureReceived;
            loginAcknowledgedMessage.LoginAcknowledgementReceived +=
                sender => LoginAcknowledged?.Invoke(this, new LoginAcknowledgedArgs());

            logoutMessageHandler.LogoutAcknowledgmentReceived += sender => LoggedOut?.Invoke(this);

            _LoginPacketWriter = loginPacketWriter ?? throw new ArgumentNullException(nameof(loginPacketWriter));
            _MessageSender = messageSender ?? throw new ArgumentNullException(nameof(messageSender));

        }

        private void DeniedMessageHandlerOnLoginFailureReceived(LoginDeniedMessageHandler sender, LoginFailedArgs args)
        {
            string message;
            switch (args.Reason)
            {
                case LoginFailedReason.InvalidLoginData:
                    message = $"[Ungültige Logindaten] {args.ReasonText}";
                    break;
                case LoginFailedReason.Banned:
                    message = $"[Account gebannt] {args.ReasonText}";
                    break;
                case LoginFailedReason.UserNameAlreadyInUse:
                    message = $"[Accountname bereits vergeben] {args.ReasonText}";
                    break;
                case LoginFailedReason.AccountAlreadyLoggedIn:
                    message = $"[Account bereits in Benutzung] {args.ReasonText}";
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            LoginDenied?.Invoke(this, new LoginDeniedArgs(message));
        }

        public void StartLogin(string userName, string password)
        {
            if (string.IsNullOrWhiteSpace(userName))
            {
                throw new ArgumentException(@"Value cannot be null or whitespace.", nameof(userName));
            }

            if (string.IsNullOrWhiteSpace(password))
            {
                throw new ArgumentException(@"Value cannot be null or whitespace.", nameof(password));
            }

            var packet = _LoginPacketWriter.WriteLogin(userName, password);
            _MessageSender.SendScriptMessage(packet, NetPriority.High, NetReliability.Reliable);
        }

        public void StartLogout()
        {
          _MessageSender.SendScriptMessage(_LoginPacketWriter.WriteLogout(),NetPriority.High,NetReliability.Reliable);
        }

    }
}
