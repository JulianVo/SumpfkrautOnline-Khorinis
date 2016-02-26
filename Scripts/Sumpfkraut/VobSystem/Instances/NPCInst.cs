﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GUC.Enumeration;
using GUC.WorldObjects;
using GUC.WorldObjects.Mobs;
using GUC.Network;

namespace GUC.Scripts.Sumpfkraut.VobSystem.Instances
{
    public partial class NPCInst : VobInst, WorldObjects.NPC.IScriptNPC
    {

        #region Properties

        public new WorldObjects.NPC BaseInst { get { return (WorldObjects.NPC)base.BaseInst; } }

        #endregion

        public NPCInst(PacketReader stream) : base(new NPC(), stream)
        {
        }

        #region Client Commands

        public void OnCmdDrawItem(Item item)
        {
            throw new NotImplementedException();
        }

        public void OnCmdDropItem(Item item)
        {
            throw new NotImplementedException();
        }

        public void OnCmdJump()
        {
            throw new NotImplementedException();
        }

        public void OnCmdMove(NPCStates state)
        {
            throw new NotImplementedException();
        }

        public void OnCmdMove(NPCStates state, NPC target)
        {
            throw new NotImplementedException();
        }

        public void OnCmdPickupItem(Item item)
        {
            throw new NotImplementedException();
        }

        public void OnCmdUseItem(Item item)
        {
            throw new NotImplementedException();
        }

        public void OnCmdUseMob(MobInter mob)
        {
            throw new NotImplementedException();
        }

        public void OnWriteTakeControl(PacketWriter stream)
        {
            // write everything the player needs to know about this npc
            // i.e. abilities, level, guild etc
        }

        public void OnReadTakeControl(PacketReader stream)
        {
            // read everything the player needs to know about this npc
            // i.e. abilities, level, guild etc
        }

        #endregion
    }
}
