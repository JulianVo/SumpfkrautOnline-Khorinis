﻿using System;
using RP_Server_Scripts.Authentication;
using RP_Server_Scripts.Logging;
using RP_Server_Scripts.VobSystem.Instances;

namespace RP_Server_Scripts.Character
{
    internal sealed class AccountLogoutHandling
    {
        public AccountLogoutHandling(AuthenticationService service, CharacterService characterService, ILoggerFactory loggerFactory)
        {
            if (service == null)
            {
                throw new ArgumentNullException(nameof(service));
            }

            if (characterService == null)
            {
                throw new ArgumentNullException(nameof(characterService));
            }

            if (loggerFactory == null)
            {
                throw new ArgumentNullException(nameof(loggerFactory));
            }

            ILogger log = loggerFactory.GetLogger(GetType());


            service.ClientLoggedOut += (sender, args) =>
            {
                try
                {
                    if (args.Client.TryGetControlledNpc(out NpcInst npc))
                    {
                        //If the client is still connected, remove the control. If it is not connected, it does not matter.

                        if (characterService.TryGetMapping(npc, out CharacterMapping mapping))
                        {
                            mapping.Character.RemoveMapping();
                            mapping.Character.Save();
                        }

                        if (args.Client.IsConnected)
                        {
                            args.Client.RemoveControl();
                        }

                    

                        npc.Despawn();
                    }
                }
                catch (Exception e)
                {
                    log.Error($"Something went wrong while trying to remove any mapped characters of the client '{args.Client.SystemAddress}'(logout). Exception: {e}");
                }
            };
        }
    }
}
