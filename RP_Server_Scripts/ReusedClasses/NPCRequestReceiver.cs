using System;
using GUC.Network;
using GUC.Types;
using GUC.WorldObjects;
using RP_Server_Scripts.VobSystem.Instances;
using RP_Shared_Script;

namespace RP_Server_Scripts.ReusedClasses
{
    public class NPCRequestReceiver
    {
        public event Action<NpcInst, JumpMoves> OnJump;
        public event Action<NpcInst, ClimbMoves, NPC.ClimbingLedge> OnClimb;
        public event Action<NpcInst> OnDrawFists;
        public event Action<NpcInst, ItemInst> OnDrawWeapon;
        public event Action<NpcInst, FightMoves> OnFightMove;
        public event Action<NpcInst, ItemInst, int> OnDropItem;
        public event Action<NpcInst, ItemInst> OnTakeItem;
        public event Action<NpcInst, ItemInst> OnEquipItem;
        public event Action<NpcInst, ItemInst> OnUnequipItem;
        public event Action<NpcInst, ItemInst> OnUseItem;
        public event Action<NpcInst> OnAim;
        public event Action<NpcInst> OnUnaim;
        public event Action<NpcInst, Vec3f, Vec3f> OnShoot;
        public event Action<NpcInst, VoiceCmd> OnVoice;
        public event Action<NpcInst, NpcInst> OnHelpUp;


        public void ReadRequest(RequestMessageIDs id, PacketReader stream, NpcInst npc)
        {
            switch (id)
            {
                case RequestMessageIDs.JumpFwd:
                    OnJump?.Invoke(npc, JumpMoves.Fwd);
                    break;
                case RequestMessageIDs.JumpRun:
                    OnJump?.Invoke(npc, JumpMoves.Run);
                    break;
                case RequestMessageIDs.JumpUp:
                    OnJump?.Invoke(npc, JumpMoves.Up);
                    break;

                case RequestMessageIDs.ClimbHigh:
                    OnClimb?.Invoke(npc, ClimbMoves.High, new NPC.ClimbingLedge(stream));
                    break;
                case RequestMessageIDs.ClimbMid:
                    OnClimb?.Invoke(npc, ClimbMoves.Mid, new NPC.ClimbingLedge(stream));
                    break;
                case RequestMessageIDs.ClimbLow:
                    OnClimb?.Invoke(npc, ClimbMoves.Low, new NPC.ClimbingLedge(stream));
                    break;

                case RequestMessageIDs.DrawFists:
                    OnDrawFists?.Invoke(npc);
                    break;
                case RequestMessageIDs.DrawWeapon:
                    OnDrawWeapon?.Invoke(npc, npc.Inventory.GetItem(stream.ReadByte()));
                    break;

                case RequestMessageIDs.AttackForward:
                    OnFightMove?.Invoke(npc, FightMoves.Fwd);
                    break;
                case RequestMessageIDs.AttackLeft:
                    OnFightMove?.Invoke(npc, FightMoves.Left);
                    break;
                case RequestMessageIDs.AttackRight:
                    OnFightMove?.Invoke(npc, FightMoves.Right);
                    break;
                case RequestMessageIDs.AttackRun:
                    OnFightMove?.Invoke(npc, FightMoves.Run);
                    break;
                case RequestMessageIDs.Parry:
                    OnFightMove?.Invoke(npc, FightMoves.Parry);
                    break;
                case RequestMessageIDs.Dodge:
                    OnFightMove?.Invoke(npc, FightMoves.Dodge);
                    break;

                case RequestMessageIDs.DropItem:
                    OnDropItem?.Invoke(npc, npc.Inventory.GetItem(stream.ReadByte()), stream.ReadUShort());
                    break;
                case RequestMessageIDs.TakeItem:
                    if (npc.World.TryGetVob(stream.ReadUShort(), out ItemInst item))
                        OnTakeItem?.Invoke(npc, item);
                    break;

                case RequestMessageIDs.EquipItem:
                    OnEquipItem?.Invoke(npc, npc.Inventory.GetItem(stream.ReadByte()));
                    break;
                case RequestMessageIDs.UnequipItem:
                    OnUnequipItem?.Invoke(npc, npc.Inventory.GetItem(stream.ReadByte()));
                    break;

                case RequestMessageIDs.UseItem:
                    OnUseItem?.Invoke(npc, npc.Inventory.GetItem(stream.ReadByte()));
                    break;

                case RequestMessageIDs.Aim:
                    OnAim?.Invoke(npc);
                    break;
                case RequestMessageIDs.Unaim:
                    OnUnaim?.Invoke(npc);
                    break;
                case RequestMessageIDs.Shoot:
                    OnShoot?.Invoke(npc, stream.ReadVec3f(), stream.ReadVec3f());
                    break;

                case RequestMessageIDs.Voice:
                    OnVoice?.Invoke(npc, (VoiceCmd)stream.ReadByte());
                    break;
                case RequestMessageIDs.HelpUp:
                    if (npc.World.TryGetVob(stream.ReadUShort(), out NpcInst target))
                        OnHelpUp?.Invoke(npc, target);
                    break;

                default:
                    GUC.Log.Logger.Log("Received Script RequestMessage with invalid ID: " + id.ToString());
                    break;
            }
        }
    }
}
