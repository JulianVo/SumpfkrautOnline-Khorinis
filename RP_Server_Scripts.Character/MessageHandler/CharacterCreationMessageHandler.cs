using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GUC;
using GUC.Network;
using GUC.Scripts.ReusedClasses;
using RP_Server_Scripts.Authentication;
using RP_Server_Scripts.Logging;
using RP_Server_Scripts.Network;
using RP_Server_Scripts.RP;
using RP_Shared_Script;

namespace RP_Server_Scripts.Character.MessageHandler
{
    internal class CharacterCreationMessageHandler : IScriptMessageHandler
    {
        private readonly CharacterService _CharacterService;
        private readonly AuthenticationService _AuthenticationService;
        private readonly IPacketWriterPool _PacketWriterPool;
        private readonly RpConfig _RpConfig;
        private readonly ILogger _Log;

        public CharacterCreationMessageHandler(CharacterService characterService, AuthenticationService authenticationService, ILoggerFactory loggerFactory, IPacketWriterPool packetWriterPool, RpConfig rpConfig)
        {
            _CharacterService = characterService ?? throw new ArgumentNullException(nameof(characterService));
            _AuthenticationService = authenticationService ?? throw new ArgumentNullException(nameof(authenticationService));
            _PacketWriterPool = packetWriterPool ?? throw new ArgumentNullException(nameof(packetWriterPool));
            _RpConfig = rpConfig ?? throw new ArgumentNullException(nameof(rpConfig));
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
                CharCreationInfo creationInfo = new CharCreationInfo();
                creationInfo.Read(stream);

                //Get the account session of the client.
                if (_AuthenticationService.TryGetSession(sender, out Session session))
                {
                    //Lets first check if the account can have an additional account(limit is taken from the rp config).
                    int charOwnershipsCount = await _CharacterService.GetCharacterOwnershipsCountAsync(session.Account);


                    if (charOwnershipsCount >= _RpConfig.MaxCharacterPerAccount)
                    {
                        using (var packet = _PacketWriterPool.GetScriptMessageStream(ScriptMessages.CharacterCreationResult))
                        {
                            packet.Write(false);
                            packet.Write((byte)CharacterCreationFailure.CharacterLimitReached);
                            sender.SendScriptMessage(packet, NetPriority.Medium, NetReliability.Reliable);
                        }
                        return;
                    }


                    //Create the new character.
                    CharacterCreationResult result =
                        await _CharacterService.CreateHumanPlayerCharacterAsync(creationInfo);



                    if (result is CharacterCreationFailed failed)
                    {
                        using (var packet = _PacketWriterPool.GetScriptMessageStream(ScriptMessages.CharacterCreationResult))
                        {
                            //Failure
                            packet.Write(false);
                            packet.Write((byte)failed.Reason);
                            sender.SendScriptMessage(packet, NetPriority.Medium, NetReliability.Reliable);
                        }
                    }
                    else if (result is CharacterCreationSuccess success)
                    {
                        //The character was created. Add an ownership entity and set the active character for account of the message sender.
                        await Task.WhenAll(new List<Task>
                        {
                             _CharacterService.AddCharacterOwnershipAsync(session.Account, success.Character),
                             _CharacterService.SetAccountActiveCharacterAsync(session.Account, success.Character)
                        });


                        using (var packet = _PacketWriterPool.GetScriptMessageStream(ScriptMessages.CharacterCreationResult))
                        {
                            //Success
                            packet.Write(true);
                            sender.SendScriptMessage(packet, NetPriority.Medium, NetReliability.Reliable);
                        }
                    }


                }
                else
                {
                    //This should not even be possible. Disconnect the client.
                    _Log.Error($"The client '{sender.SystemAddress}' tried to create a character while not being logged in(should never be possible)");
                    sender.Disconnect();
                }
            }
            catch (Exception e)
            {
                _Log.Error($"Something went wrong while handling a '{SupportedMessage}' message from the client '{sender.SystemAddress}'. Exception: {e}");
                sender.Disconnect();
            }
        }

        public ScriptMessages SupportedMessage => ScriptMessages.CreateCharacter;
    }
}
