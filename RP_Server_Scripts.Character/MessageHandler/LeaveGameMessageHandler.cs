using System;
using GUC.Network;
using RP_Server_Scripts.Logging;
using RP_Server_Scripts.Network;
using RP_Server_Scripts.VobSystem.Instances;
using RP_Shared_Script;

namespace RP_Server_Scripts.Character.MessageHandler
{
    internal sealed class LeaveGameMessageHandler : IScriptMessageHandler
    {
        private readonly CharacterService _CharacterService;
        private readonly ILogger _Log;

        public LeaveGameMessageHandler(CharacterService characterService,ILoggerFactory loggerFactory)
        {
            _CharacterService = characterService ?? throw new ArgumentNullException(nameof(characterService));
            _Log=loggerFactory?.GetLogger(GetType()) ?? throw new ArgumentNullException(nameof(loggerFactory));
        }

        public void HandleMessage(Client.Client sender, PacketReader stream)
        {
            if (sender == null)
            {
                throw new ArgumentNullException(nameof(sender));
            }

            try
            {
                if (sender.TryGetControlledNpc(out NpcInst npc))
                {
                    sender.RemoveControl();

                    if (_CharacterService.TryGetMapping(npc, out CharacterMapping mapping))
                    {
                        mapping.Character.RemoveMapping();
                        mapping.Character.Save();
                    }

                    npc.Despawn();
                }
            }
            catch (Exception e)
            {
                _Log.Error($"Something went wrong while handling a '{SupportedMessage}' script message. Disconnecting the client to get back into a defined state. Exception:  {e}");
                sender.Disconnect();
            }

        }

        public ScriptMessages SupportedMessage => ScriptMessages.LeaveGame;
    }
}
