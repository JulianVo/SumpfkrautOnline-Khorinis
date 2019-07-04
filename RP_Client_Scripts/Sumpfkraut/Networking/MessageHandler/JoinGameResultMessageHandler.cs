using System;
using GUC.Network;
using GUC.Scripts.Logging;
using RP_Shared_Script;

namespace GUC.Scripts.Sumpfkraut.Networking.MessageHandler
{
    internal sealed class JoinGameResultMessageHandler:IScriptMessageHandler
    {
        private readonly ILogger _Log;

        public event GenericEventHandler<JoinGameResultMessageHandler, JoinGameFailedReason> JoinGameFailedReceived;
        public event GenericEventHandler<JoinGameResultMessageHandler> JoinGameSuccessReceived;


        public JoinGameResultMessageHandler(ILoggerFactory  loggerFactory)
        {
            _Log = loggerFactory?.GetLogger(GetType()) ?? throw new ArgumentNullException(nameof(loggerFactory));
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
                    JoinGameSuccessReceived?.Invoke(this);
                }
                else
                {
                    JoinGameFailedReason reason = (JoinGameFailedReason)stream.ReadByte();
                    JoinGameFailedReceived?.Invoke(this, reason);
                }
            }
            catch (Exception e)
            {
                _Log.Error($"Failed to handle a script message of typ {SupportedMessage}. Exception: {e}");
                JoinGameFailedReceived?.Invoke(this,JoinGameFailedReason.MessageHandlingError);
            }
        }

        public ScriptMessages SupportedMessage => ScriptMessages.JoinGameResult;
    }
}
