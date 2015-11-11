﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gothic.zClasses;
using GUC.Enumeration;
using GUC.Types;
using Gothic.zTypes;
using RakNet;
using GUC.Network;
using Gothic.zStruct;
using GUC.Client.Hooks;

namespace GUC.Client.WorldObjects
{
    class NPC : AbstractVob
    {
        public const long PositionUpdateTime = 1200000; //120ms
        public const long DirectionUpdateTime = PositionUpdateTime + 100000;

        public NPCInstance instance;

        public MobInter UsedMob;

        public NPC(uint id, ushort instanceID)
            : base(id)
        {
            instance = NPCInstance.Table.Get(instanceID);
            instance.SetProperties(this);
        }

        public NPC(uint id, ushort instanceID, oCNpc npc)
            : base(id, npc)
        {
            instance = NPCInstance.Table.Get(instanceID);
            instance.SetProperties(this);
        }

        protected override void CreateVob(bool createNew)
        {
            if (createNew)
            {
                gVob = oCNpc.Create(Program.Process);
            }

            gNpc.Instance = instance.ID;
            gNpc.Name.Set(Name);
            gNpc.SetVisual(Visual);
            gNpc.SetAdditionalVisuals(BodyMesh, BodyTex, 0, HeadMesh, HeadTex, 0, -1);
            using (zVec3 z = zVec3.Create(Program.Process))
            {
                z.X = BodyWidth;
                z.Y = BodyHeight;
                z.Z = BodyWidth;
                gNpc.SetModelScale(z);
            }
            gNpc.SetFatness(Fatness);

            gNpc.Voice = Voice;

            gNpc.HPMax = HPMax;
            gNpc.HP = HP;

            foreach (Item item in equippedSlots.Values)
            {
                if (item.IsMeleeWeapon)
                    gNpc.EquipWeapon(item.gItem);
                else if (item.IsRangedWeapon)
                    gNpc.EquipFarWeapon(item.gItem);
                else if (item.IsArmor)
                    gNpc.EquipArmor(item.gItem);
                else
                    gNpc.EquipItem(item.gItem);
            }

            gNpc.InitHumanAI();
            gAniCtrl = gNpc.AniCtrl;
        }

        protected ushort hpmax = 100;
        public ushort HPMax
        {
            get { return hpmax; }
            set
            {
                hpmax = value;
                if (Spawned)
                {
                    gNpc.HPMax = value;
                }
            }
        }

        protected ushort hp = 100;
        public ushort HP
        {
            get { return hp; }
            set
            {
                hp = value;
                if (Spawned)
                {
                    gNpc.HP = value;
                }
            }
        }

        protected string name = "";
        public string Name
        {
            set
            {
                name = value;
                if (Spawned)
                {
                    using (zString z = zString.Create(Program.Process, value))
                    {
                        gNpc.SetName(z);
                    }
                }

            }
            get
            {
                return name;
            }
        }

        public oCNpc gNpc
        {
            get
            {
                return new oCNpc(Program.Process, gVob.Address);
            }
        }

        public oCAniCtrl_Human gAniCtrl { get; protected set; }

        public bool HasFreeHands
        {
            get
            {
                //return (this.gNpc.BodyState & 65536) != 0;
                return true;
            }
        }

        protected int voice = 0;
        public int Voice
        {
            get { return voice; }
            set
            {
                voice = value;
                if (Spawned)
                {
                    gNpc.Voice = value;
                }
            }
        }

        public NPCState State = NPCState.Stand;

        #region Visual

        public string Visual { get { return instance.visual; } }
        public string BodyMesh { get { return instance.bodyMesh; } }
        public int BodyTex { get; protected set; }
        public string HeadMesh { get; protected set; }
        public int HeadTex { get; protected set; }

        public void SetBodyVisuals(int bodyTex, string headMesh, int headTex)
        {
            this.BodyTex = bodyTex;
            this.HeadMesh = headMesh;
            this.HeadTex = headTex;
            if (Spawned)
            {
                gNpc.SetAdditionalVisuals(BodyMesh, bodyTex, 0, headMesh, headTex, 0, -1);
            }
        }

        protected float fatness = 0;
        public float Fatness
        {
            get
            {
                return fatness;
            }
            set
            {
                fatness = value;
                if (Spawned)
                {
                    gNpc.SetFatness(value);
                }
            }
        }

        protected float bodyHeight = 1.0f;
        public float BodyHeight
        {
            get
            {
                return bodyHeight;
            }
            set
            {
                bodyHeight = value;
                if (Spawned)
                {
                    using (zVec3 scale = zVec3.Create(Program.Process))
                    {
                        scale.X = gNpc.Scale.X;
                        scale.Y = value;
                        scale.Z = gNpc.Scale.Z;
                        gNpc.SetModelScale(scale);
                    }
                }
            }
        }

        //x & z together
        protected float bodyWidth = 1.0f;
        public float BodyWidth
        {
            get
            {
                return bodyWidth;
            }
            set
            {
                bodyWidth = value;
                if (Spawned)
                {
                    using (zVec3 scale = zVec3.Create(Program.Process))
                    {
                        scale.X = value;
                        scale.Y = gNpc.Scale.Y;
                        scale.Z = value;
                        gNpc.SetModelScale(scale);
                    }
                }
            }
        }
        #endregion

        #region Animation

        public int TurnAnimation = 0;

        public void AnimationStart(Animations ani)
        {
            using (zString z = zString.Create(Program.Process, ani.ToString()))
            {
                gNpc.GetModel().StartAnimation(z);
            }
        }

        public void AnimationStop(Animations ani)
        {
            using (zString z = zString.Create(Program.Process, ani.ToString()))
            {
                gNpc.GetModel().StopAnimation(z);
            }
        }

        public void AnimationFade(Animations ani)
        {
            using (zString z = zString.Create(Program.Process, ani.ToString()))
            {
                int id = gNpc.GetModel().GetAniIDFromAniName(z);
                gNpc.GetModel().FadeOutAni(id);
            }
        }

        #endregion

        public Item DrawnItem = null;

        #region Equipment

        public Dictionary<byte, Item> equippedSlots = new Dictionary<byte, Item>();

        public void EquipSlot(byte slot, Item item)
        {
            if (item != null && !item.Spawned)
            {
                item.Slot = slot;
                if (UnequipSlot(slot))
                {
                    equippedSlots[slot] = item;
                }
                else
                {
                    equippedSlots.Add(slot, item);
                }

                if (Spawned)
                {
                    if (item.IsMeleeWeapon)
                        gNpc.EquipWeapon(item.gItem);
                    else if (item.IsRangedWeapon)
                        gNpc.EquipFarWeapon(item.gItem);
                    else if (item.IsArmor)
                        gNpc.EquipArmor(item.gItem);
                    else
                        gNpc.EquipItem(item.gItem);
                }
            }
        }

        public bool UnequipSlot(byte slot)
        {
            Item item;
            if (equippedSlots.TryGetValue(slot, out item))
            {
                item.Slot = 0;
                if (Spawned)
                {
                    gNpc.UnequipItem(item.gItem);
                }
                return true;
            }
            return false;
        }

        #endregion

        public Vec3f lastDir;
        public Vec3f nextDir;
        public long lastDirTime;
        public int turn;

        public void StartTurnAni(bool right)
        {
            turn = right ? 1 : -1;
            DoTurnAni();
        }

        void DoTurnAni()
        {
            TurnAnimation = 0;
            zCModel model = gNpc.GetModel();

            if (model.IsAniActive(model.GetAniFromAniID(gNpc.AniCtrl._s_walk)))
            {
                if (turn > 0)
                {
                    TurnAnimation = gNpc.AniCtrl._t_turnr;
                }
                else if (turn < 0)
                {
                    TurnAnimation = gNpc.AniCtrl._t_turnl;
                }
            }
            else if (model.IsAniActive(model.GetAniFromAniID(gNpc.AniCtrl._s_dive)))
            {
                if (turn > 0)
                {
                    TurnAnimation = gNpc.AniCtrl._t_diveturnr;
                }
                else if (turn < 0)
                {
                    TurnAnimation = gNpc.AniCtrl._t_diveturnl;
                }
            }
            else if (model.IsAniActive(model.GetAniFromAniID(gNpc.AniCtrl._s_swim)))
            {
                if (turn > 0)
                {
                    TurnAnimation = gNpc.AniCtrl._t_swimturnr;
                }
                else if (turn < 0)
                {
                    TurnAnimation = gNpc.AniCtrl._t_swimturnl;
                }
            }

            if (TurnAnimation != 0)
            {
                gNpc.GetModel().StartAni(TurnAnimation, 0);
                turn = 0;
            }
        }

        public void StopTurnAnis()
        {
            gNpc.GetModel().FadeOutAni(TurnAnimation);
            TurnAnimation = 0;
            lastDir = null;
            nextDir = null;
            turn = 0;
        }

        public long nextPosUpdate = 0;
        public long nextForwardUpdate = 0;
        public long nextStandUpdate = 0;
        public long nextBackwardUpdate = 0;
        public long nextJumpUpdate = 0;

        public bool DoJump = false;

        public override void Update(long now)
        {
            if (this != Player.Hero)
            {
                if (nextDir != null) //turn!
                {
                    if (turn != 0)
                    {
                        DoTurnAni();
                    }

                    float diff = (float)(DateTime.Now.Ticks - lastDirTime) / (float)DirectionUpdateTime;

                    if (diff < 1.0f)
                    {
                        this.Direction = lastDir + (nextDir - lastDir) * diff;
                    }
                    else
                    {
                        this.Direction = nextDir;
                        StopTurnAnis();
                    }
                }

                switch (State)
                {
                    case NPCState.MoveForward:
                        gNpc.AniCtrl._Forward();
                        break;
                    case NPCState.MoveBackward:
                        gNpc.AniCtrl._Backward();
                        break;
                    case NPCState.MoveRight:
                        gVob.GetEM(0).KillMessages();
                        gNpc.DoStrafe(true);
                        break;
                    case NPCState.MoveLeft:
                        gVob.GetEM(0).KillMessages();
                        gNpc.DoStrafe(false);
                        break;
                    case NPCState.Stand:
                        gNpc.AniCtrl._Stand();
                        break;
                    default:
                        break;
                }
            }
            else
            {
                if (nextPosUpdate < DateTime.Now.Ticks)
                {
                    Network.Messages.VobMessage.WritePosDir(this);
                    nextPosUpdate = DateTime.Now.Ticks + PositionUpdateTime;
                }
            }
        }

        public void DrawItem(Item item, bool fast)
        {
            if (item == null) return;

            DrawnItem = item;

            if (item == Item.Fists)
            {
                if (fast)
                {
                    gNpc.SetWeaponMode2(2);
                }
                else
                {
                    gVob.GetEM(0).StartMessage(oCMsgWeapon.Create(Program.Process, oCMsgWeapon.SubTypes.DrawWeapon, 0, 0), gVob);
                }
            }
            else
            {
                switch (item.Type)
                {
                    case ItemType.Sword_1H:
                    case ItemType.Sword_2H:
                    case ItemType.Blunt_1H:
                    case ItemType.Blunt_2H:
                        if (fast)
                        {
                            gNpc.SetWeaponMode2(3);
                        }
                        else
                        {
                            gVob.GetEM(0).StartMessage(oCMsgWeapon.Create(Program.Process, oCMsgWeapon.SubTypes.DrawWeapon, 0, 0), gVob);
                        }
                        break;
                    case ItemType.Bow:
                    case ItemType.XBow:
                        if (fast)
                        {
                            gNpc.SetWeaponMode2(4);
                        }
                        else
                        {
                            gVob.GetEM(0).StartMessage(oCMsgWeapon.Create(Program.Process, oCMsgWeapon.SubTypes.DrawWeapon, 4, 0), gVob);
                        }
                        break;
                    case ItemType.Armor:
                        break;
                    case ItemType.Ring:
                        break;
                    case ItemType.Amulet:
                        break;
                    case ItemType.Belt:
                        break;
                    case ItemType.Food_Huge:
                    case ItemType.Food_Small:
                    case ItemType.Drink:
                    case ItemType.Potions:
                        break;
                    case ItemType.Document:
                    case ItemType.Book:
                        break;
                    case ItemType.Rune:
                    case ItemType.Scroll:
                        break;
                    case ItemType.Misc_Usable:
                        break;
                    case ItemType.Misc:
                        break;
                }
            }
        }

        public void UndrawItem(bool altRemove, bool fast)
        {
            Item item = DrawnItem;
            if (item == null)
                return;

            if (item == Item.Fists || (item.Type >= ItemType.Sword_1H && item.Type <= ItemType.XBow) || item.Type == ItemType.Scroll || item.Type == ItemType.Rune)
            {
                if (fast)
                {
                    gNpc.SetWeaponMode2(0);
                    if (this == Player.Hero)
                    {
                        oCNpcFocus.SetFocusMode(Program.Process, 0);
                    }
                }
                else
                {
                    if (altRemove && gAniCtrl.IsStanding())
                    {
                        gVob.GetEM(0).StartMessage(oCMsgWeapon.Create(Program.Process, oCMsgWeapon.SubTypes.RemoveWeapon1, gAniCtrl.wmode_last, 0), gVob);
                    }
                    else
                    {
                        gVob.GetEM(0).StartMessage(oCMsgWeapon.Create(Program.Process, oCMsgWeapon.SubTypes.RemoveWeapon, 0, 0), gVob);
                    }
                }
            }

            DrawnItem = null;
        }
    }
}
