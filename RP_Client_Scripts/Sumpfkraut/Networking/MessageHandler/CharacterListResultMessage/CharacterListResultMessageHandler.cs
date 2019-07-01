using System;
using System.Collections.Generic;
using GUC.Network;
using GUC.Scripts.Logging;
using GUC.Scripts.Sumpfkraut.Networking.MessageHandler.CharacterListResultMessage;
using RP_Shared_Script;

namespace GUC.Scripts.Sumpfkraut.Networking.MessageHandler
{
    internal sealed class CharacterListResultMessageHandler : IScriptMessageHandler
    {
        private readonly CharacterVisualsReader _CharacterVisualsReader;
        private readonly ILogger _Log;

        public event GenericEventHandler<CharacterListResultMessageHandler, CharacterListReceivedArgs> CharacterListReceived;

        public CharacterListResultMessageHandler(CharacterVisualsReader characterVisualsReader, ILoggerFactory loggerFactory)
        {
            _CharacterVisualsReader = characterVisualsReader ?? throw new ArgumentNullException(nameof(characterVisualsReader));
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
                //Get the id of the last active character and the amount of characters that are encoded in the script message.
                int activeCharacterId = stream.ReadInt();
                int characterCount = stream.ReadByte();

                //Read all characters that are encoded in the message.
                var characters = new List<Character.Character>(characterCount);
                for (int i = 0; i < characterCount; i++)
                {
                    characters.Add(_CharacterVisualsReader.ReadCharacter(stream));
                }


                CharacterListReceived?.Invoke(this, new CharacterListReceivedArgs(characters, activeCharacterId));
            }
            catch (Exception e)
            {
                string msg = $"Something went wrong while handling a script message of type '{SupportedMessage}'";
                _Log.Error(msg);
                throw new ScriptMessageHandlingException(msg, e);
            }
        }

        public ScriptMessages SupportedMessage => ScriptMessages.CharacterListResult;
    }
}
