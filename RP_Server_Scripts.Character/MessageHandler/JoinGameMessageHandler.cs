using System;
using System.Collections.Generic;
using System.Linq;
using GUC;
using GUC.Network;
using RP_Server_Scripts.Authentication;
using RP_Server_Scripts.Logging;
using RP_Server_Scripts.Network;
using RP_Server_Scripts.VobSystem.Instances;
using RP_Shared_Script;

namespace RP_Server_Scripts.Character.MessageHandler
{
    internal sealed class JoinGameMessageHandler : IScriptMessageHandler
    {
        private readonly CharacterService _Service;
        private readonly AuthenticationService _AuthenticationService;
        private readonly IPacketWriterPool _PacketWriterPool;
        private readonly ILogger _Log;

        public JoinGameMessageHandler(CharacterService service, AuthenticationService authenticationService, IPacketWriterPool packetWriterPool, ILoggerFactory loggerFactory)
        {
            _Service = service ?? throw new ArgumentNullException(nameof(service));
            _AuthenticationService = authenticationService ?? throw new ArgumentNullException(nameof(authenticationService));
            _PacketWriterPool = packetWriterPool ?? throw new ArgumentNullException(nameof(packetWriterPool));
            _Log = loggerFactory?.GetLogger(GetType()) ?? throw new ArgumentNullException(nameof(loggerFactory));
        }

        public async void HandleMessage(Client.Client sender, PacketReader stream)
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
                int id = stream.ReadInt();

                //Projection against receiving the message again(which would be a client problem).
                if (sender.TryGetControlledNpc(out NpcInst controlledNpc))
                {
                    if (_Service.TryGetMapping(controlledNpc, out CharacterMapping characterMapping))
                    {
                        characterMapping.Character.RemoveMapping();
                    }
                    else
                    {
                        controlledNpc.Despawn();
                    }

                    _Log.Error($"A client send a {SupportedMessage} even through it does already control a npc. Trying to disconnect the player and remove the npc to get back into a defined state");
                    sender.Disconnect();
                    return;
                }

                //Get and map the selected character of the client.
                if (_AuthenticationService.TryGetSession(sender, out Session session))
                {
                    IList<Character> list = await _Service.GetAccountOwnedCharactersAsync(session.Account);
                    Character selectedChar = list.FirstOrDefault(c => c.CharacterId == id);

                    bool success = true;
                    JoinGameFailedReason failureReason = JoinGameFailedReason.None;


                    if (selectedChar == null)
                    {
                        //Invalid character id... return an error
                        success = false;
                        failureReason = JoinGameFailedReason.InvalidCharacterId;
                    }
                    else
                    {
                        if (!selectedChar.TryGetMapping(out CharacterMapping mapping))
                        {
                            mapping = selectedChar.SpawnAndMap();
                        }

                        if (mapping.CharacterNpc.TryGetControllingClient(out Client.Client controllingClient))
                        {
                            //Character is in use by someone else.
                            success = false;
                            failureReason = JoinGameFailedReason.CharacterInUse;
                        }
                        else
                        {
                            sender.SetControl(mapping.CharacterNpc);
                        }
                    }

                    //Send the result to the client.
                    using (var packet = _PacketWriterPool.GetScriptMessageStream(ScriptMessages.JoinGameResult))
                    {
                        packet.Write(success);
                        //Write the error code.
                        if (!success)
                        {
                            packet.Write((byte)failureReason);
                        }
                        sender.SendScriptMessage(packet, NetPriority.Medium, NetReliability.Reliable);
                    }
                }
            }
            catch (Exception e)
            {
                _Log.Error($"Something went wrong while handling a '{SupportedMessage}' script message. Disconnecting the client to get back into a defined state. Exception:  {e}");

                //Disconnect the client to get back into a defined state.
                sender.Disconnect();
            }
        }

        public ScriptMessages SupportedMessage => ScriptMessages.JoinGame;
    }
}
