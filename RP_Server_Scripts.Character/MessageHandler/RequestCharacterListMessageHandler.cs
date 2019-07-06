using System;
using System.Threading.Tasks;
using GUC;
using GUC.Network;
using RP_Server_Scripts.Authentication;
using RP_Server_Scripts.Character.MessageHandler.InformationWriter;
using RP_Server_Scripts.Logging;
using RP_Server_Scripts.Network;
using RP_Shared_Script;

namespace RP_Server_Scripts.Character.MessageHandler
{
    internal sealed class RequestCharacterListMessageHandler : IScriptMessageHandler
    {
        private readonly CharacterService _CharacterService;
        private readonly AuthenticationService _AuthenticationService;
        private readonly IPacketWriterPool _WriterPool;
        private readonly CharacterVisualsWriter _VisualsWriter;
        private readonly ILogger _Log;

        public RequestCharacterListMessageHandler(CharacterService characterService, ILoggerFactory loggerFactory, AuthenticationService authenticationService,IPacketWriterPool writerPool, CharacterVisualsWriter visualsWriter)
        {
            _CharacterService = characterService ?? throw new ArgumentNullException(nameof(characterService));
            _AuthenticationService = authenticationService ?? throw new ArgumentNullException(nameof(authenticationService));
            _WriterPool = writerPool ?? throw new ArgumentNullException(nameof(writerPool));
            _VisualsWriter = visualsWriter ?? throw new ArgumentNullException(nameof(visualsWriter));
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
                //Get the account session of the client.
                if (_AuthenticationService.TryGetSession(sender, out Session session))
                {

                    var ownedCharsTask = _CharacterService.GetAccountOwnedCharactersAsync(session.Account);
                    var activeCharTask = _CharacterService.GetAccountActiveCharacterTransactionAsync(session.Account);
                    await Task.WhenAll(ownedCharsTask, activeCharTask);

                    //Get a message writer from the pool and use it to send a message.
                    using (var writer = _WriterPool.GetScriptMessageStream(ScriptMessages.CharacterListResult))
                    {
                        //Write the id of the active character. Write -1 if no active character was found.
                        if (activeCharTask.Result is ActiveCharacterFound activeCharacterFound)
                        {
                            writer.Write(activeCharacterFound.CharacterId);
                        }
                        else
                        {
                            writer.Write((int)-1);
                        }

                        var charList = ownedCharsTask.Result;

                        //Write the length of the character list and the visuals of all characters.
                        writer.Write((byte)charList.Count);
                        foreach (var character in charList)
                        {
                            _VisualsWriter.WriteCharacter(writer, character);
                        }

                        //Send the message to the client. This message does not need to be transmitted quickly but it has to be reliable.
                        sender.SendScriptMessage(writer, NetPriority.Medium, NetReliability.Reliable);
                    }
                }
                else
                {
                    //This should not even be possible. Disconnect the client.
                    _Log.Error($"The client '{sender.SystemAddress}' tried to request a character list while not being logged in(should never be possible)");
                    sender.Disconnect();
                }
            }
            catch (Exception e)
            {
                _Log.Error($"Something went wrong while handling a script message of type '{SupportedMessage}' from client '{sender.SystemAddress}'. Exception {e}");
                //Something went wrong really bad. Disconnect the client to get back to a consistent state.
                sender.Disconnect();
            }
        }

        public ScriptMessages SupportedMessage => ScriptMessages.RequestCharacterList;
    }
}
