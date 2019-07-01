using System;
using GUC.Scripts.Arena;
using GUC.Scripts.Sumpfkraut.Networking;
using GUC.Scripts.Sumpfkraut.Networking.MessageHandler;
using RP_Shared_Script;

namespace GUC.Scripts.Character
{
    internal sealed class CharacterCreation
    {
        private readonly IPacketWriterFactory _WriterFactory;
        private readonly ScriptMessageSender _MessageSender;


        public event GenericEventHandler<CharacterCreation, CharacterCreationFailedArgs> CharacterCreationFailed;
        public event GenericEventHandler<CharacterCreation> CharacterCreationSuccessful;
         
        public CharacterCreation(IPacketWriterFactory writerFactory, ScriptMessageSender messageSender, CharacterCreationResultMessageHandler resultMessageHandler)
        {
            _WriterFactory = writerFactory ?? throw new ArgumentNullException(nameof(writerFactory));
            _MessageSender = messageSender ?? throw new ArgumentNullException(nameof(messageSender));

            resultMessageHandler.CharacterCreationFailed += (sender, args) =>
            {
                string msg;
                switch (args)
                {
                    case CharacterCreationFailure.AlreadyExists:
                        msg = "Charactername bereits vergeben!";
                        break;
                    case CharacterCreationFailure.NameIsInvalid:
                        msg = "Charactername ist ungültig!";
                        break;
                    case CharacterCreationFailure.InvalidCreationInfo:
                        msg = "Fehlerhafte characterdaten(Netzwerkfehler?)!";
                        break;
                    case CharacterCreationFailure.CharacterLimitReached:
                        msg = "Charakterlimit erreicht!";
                        break;
                    default:
                        msg = "Unbekannter Fehler";
                        break;
                }
                CharacterCreationFailed?.Invoke(this,new CharacterCreationFailedArgs(msg));
            };

            resultMessageHandler.CharacterCreationSuccessful += sender => CharacterCreationSuccessful?.Invoke(this);
        }

        public void StartCharacterCreation(CharCreationInfo creationInfo)
        {
            if (creationInfo == null)
            {
                throw new ArgumentNullException(nameof(creationInfo));
            }

            var writer = _WriterFactory.Create(ScriptMessages.CreateCharacter);
            creationInfo.Write(writer);
            _MessageSender.SendScriptMessage(writer, NetPriority.Medium, NetReliability.Reliable);
        }
    }
}
