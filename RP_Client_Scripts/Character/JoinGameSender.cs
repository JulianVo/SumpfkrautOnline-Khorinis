using System;
using System.ComponentModel;
using GUC.Network;
using GUC.Scripts.Sumpfkraut.Networking;
using GUC.Scripts.Sumpfkraut.Networking.MessageHandler;
using RP_Shared_Script;

namespace GUC.Scripts.Character
{
    internal sealed class JoinGameSender
    {
        private readonly IPacketWriterFactory _PacketWriterFactory;
        private readonly ScriptMessageSender _MessageSender;

        public event GenericEventHandler<JoinGameSender> JoinGameRequestSuccessful;
        public event GenericEventHandler<JoinGameSender, JoinGameFailedArgs> JoinGameFailed;

        public JoinGameSender(IPacketWriterFactory packetWriterFactory, ScriptMessageSender messageSender, JoinGameResultMessageHandler messageHandler)
        {
            _PacketWriterFactory = packetWriterFactory ?? throw new ArgumentNullException(nameof(packetWriterFactory));
            _MessageSender = messageSender ?? throw new ArgumentNullException(nameof(messageSender));
            messageHandler.JoinGameSuccessReceived += sender => JoinGameRequestSuccessful?.Invoke(this);
            messageHandler.JoinGameFailedReceived += MessageHandlerOnJoinGameFailedReceived;
        }

        private void MessageHandlerOnJoinGameFailedReceived(JoinGameResultMessageHandler sender, JoinGameFailedReason args)
        {
            string reasonText = string.Empty;
            switch (args)
            {
                case JoinGameFailedReason.CharacterInUse:
                    reasonText = "Der character wird bereits verwendet";
                    break;
                case JoinGameFailedReason.Timeout:
                    reasonText = "Der Server hat zu lange nicht geantwortet";
                    break;
                case JoinGameFailedReason.MessageHandlingError:
                    reasonText = "Die Antwort des Servers war fehlerhaft.";
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(args), args, null);
            }
            JoinGameFailed?.Invoke(this, new JoinGameFailedArgs(args, reasonText));
        }

        public void StartJoinGame(Character characterToJoinWith)
        {
            if (characterToJoinWith == null)
            {
                throw new ArgumentNullException(nameof(characterToJoinWith));
            }

            PacketWriter writer = _PacketWriterFactory.Create(ScriptMessages.JoinGame);
            writer.Write(characterToJoinWith.CharacterId);
            _MessageSender.SendScriptMessage(writer, NetPriority.Medium, NetReliability.Reliable);
        }


    }

    internal class JoinGameFailedArgs
    {
        public JoinGameFailedArgs(JoinGameFailedReason reason, string reasonText)
        {
            if (!Enum.IsDefined(typeof(JoinGameFailedReason), reason))
            {
                throw new InvalidEnumArgumentException(nameof(reason), (int)reason, typeof(JoinGameFailedReason));
            }

            Reason = reason;
            ReasonText = reasonText ?? throw new ArgumentNullException(nameof(reasonText));
        }

        public JoinGameFailedReason Reason { get; }

        public string ReasonText { get; }
    }
}
