using System;
using GUC.Network;
using GUC.Scripts.Logging;
using RP_Shared_Script;

namespace GUC.Scripts.Sumpfkraut.Networking
{
    internal class LoginDeniedMessageHandler : IScriptMessageHandler
    {
        public event GenericEventHandler<LoginDeniedMessageHandler, LoginFailedArgs> LoginFailureReceived;

        private readonly ILogger _Log;

        public LoginDeniedMessageHandler(ILoggerFactory loggerFactory)
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
                LoginFailedReason reason = (LoginFailedReason)stream.ReadByte();
                string reasonText = stream.ReadString();
                LoginFailureReceived?.Invoke(this, new LoginFailedArgs(reason, reasonText));
            }
            catch (Exception e)
            {
                _Log.Error($"Failed to handle a script message of typ {SupportedMessage}. Exception: {e}");
            }
        }

        public ScriptMessages SupportedMessage => ScriptMessages.LoginDenied;
    }
}
