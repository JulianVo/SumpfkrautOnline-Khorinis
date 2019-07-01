using System;
using GUC.Network;
using GUC.Scripts.Logging;
using RP_Shared_Script;

namespace GUC.Scripts.Sumpfkraut.Networking.MessageHandler
{
    internal sealed class CharacterCreationResultMessageHandler : IScriptMessageHandler
    {
        private readonly ILogger _Log;

        public event GenericEventHandler<CharacterCreationResultMessageHandler> CharacterCreationSuccessful;
        public event GenericEventHandler<CharacterCreationResultMessageHandler, CharacterCreationFailure> CharacterCreationFailed;

        public CharacterCreationResultMessageHandler(ILoggerFactory loggerFactory)
        {
            _Log = loggerFactory?.GetLogger(GetType());
            if (_Log == null)
            {
                throw new ArgumentNullException(nameof(loggerFactory));
            }
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
                    CharacterCreationSuccessful?.Invoke(this);
                }
                else
                {
                    CharacterCreationFailure reason = (CharacterCreationFailure)stream.ReadByte();
                    CharacterCreationFailed?.Invoke(this, reason);
                }

            }
            catch (Exception e)
            {
                string msg = $"Something went wrong while handling a '{SupportedMessage}' script message from the server.";
                _Log.Error(msg);
                throw new ScriptMessageHandlingException(msg, e);
            }
        }

        public ScriptMessages SupportedMessage => ScriptMessages.CharacterCreationResult;
    }
}
