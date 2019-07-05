using System;
using GUC;
using GUC.Animations;
using GUC.Log;
using GUC.Network;
using GUC.Scripting;
using GUC.Types;
using GUC.Utilities;
using GUC.WorldObjects;
using GUC.WorldObjects.ItemContainers;
using RP_Server_Scripts.ReusedClasses;
using RP_Server_Scripts.Visuals;
using RP_Server_Scripts.VobSystem.Definitions;
using RP_Server_Scripts.VobSystem.Definitions.Item;
using RP_Server_Scripts.VobSystem.Instances.ItemContainers;
using RP_Server_Scripts.WorldSystem;
using RP_Shared_Script;

namespace RP_Server_Scripts.VobSystem.Instances
{
    public class NpcInst : VobInst, NPC.IScriptNPC, IContainer
    {
        protected override BaseVob CreateVob()
        {
            return new NPC(new ScriptInventory(this), new ModelInst(this), this);
        }

        public override VobType VobType => VobType.NPC;

        public new NPC BaseInst => (NPC)base.BaseInst;
        public ItemInventory BaseInventory => BaseInst.Inventory;
        public ScriptInventory Inventory => (ScriptInventory)BaseInventory.ScriptObject;

        public new NpcDef Definition => (NpcDef)base.Definition;

        public NPCMovement Movement => BaseInst.Movement;
        public VobEnvironment Environment => BaseInst.Environment;

        public bool IsDead => BaseInst.IsDead;
        public bool IsInFightMode => BaseInst.IsInFightMode;

        public bool IsWading
        {
            get
            {
                float waterLevel = Environment.WaterLevel;
                return waterLevel > 0 && waterLevel < 0.4f;
            }
        }

        public bool IsSwimming
        {
            get
            {
                float waterLevel = Environment.WaterLevel;
                return waterLevel > 0 && waterLevel >= 0.4f;
            }
        }

        public int HP => BaseInst.HP;
        public int HPMax => BaseInst.HPMax;

        public bool UseCustoms;
        public HumBodyTexs CustomBodyTex;
        public HumHeadMeshs CustomHeadMesh;
        public HumHeadTexs CustomHeadTex;
        public HumVoices CustomVoice;
        public float CustomFatness;
        public Vec3f CustomScale { get; set; } = new Vec3f(1, 1, 1);
        public string CustomName { get; set; }


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

        public ItemInst LastUsedWeapon;

        public ItemInst GetEquipmentBySlot(NPCSlots slotNum)
        {
            return BaseInst.TryGetEquippedItem((int)slotNum, out Item item) ? (ItemInst)item.ScriptObject : null;
        }

        public ItemInst GetArmor()
        {
            return GetEquipmentBySlot(NPCSlots.Armor);
        }

        public ItemInst GetAmmo()
        {
            return GetEquipmentBySlot(NPCSlots.Ammo);
        }

        public ItemInst GetRightHand()
        {
            return GetEquipmentBySlot(NPCSlots.RightHand);
        }

        public ItemInst GetLeftHand()
        {
            return GetEquipmentBySlot(NPCSlots.LeftHand);
        }

        public bool HasItemInHands()
        {
            return GetRightHand() != null || GetLeftHand() != null;
        }

        public ItemInst GetDrawnWeapon()
        {
            ItemInst item;
            if ((item = GetRightHand()) != null && item.IsWeapon
                || (item = GetLeftHand()) != null && item.IsWeapon)
            {
                return item;
            }

            return null;
        }

        public void EquipItem(int slot, Item item)
        {
            EquipItem((NPCSlots)slot, (ItemInst)item.ScriptObject);
        }



        public event OnEquipHandler OnEquip;


        public void EquipItem(NPCSlots slot, ItemInst item)
        {
            if (item.BaseInst.Slot == (int)slot)
            {
                return;
            }


            BaseInst.EquipItem((int)slot, item.BaseInst);


            OnEquip?.Invoke(item);
        }

        public void UnequipItem(Item item)
        {
            UnequipItem((ItemInst)item.ScriptObject);
        }

        public event OnEquipHandler OnUnequip;


        public void UnequipItem(ItemInst item)
        {
            BaseInst.UnequipItem(item.BaseInst);


            OnUnequip?.Invoke(item);
        }

        public void UnequipSlot(NPCSlots slot)
        {
            ItemInst item = GetEquipmentBySlot(slot);
            if (item != null)
            {
                UnequipItem(item);
            }
        }



        public static event OnDeathHandler sOnDeath;
        public event OnDeathHandler OnDeath;

        public void SetHealth(int hp)
        {
            SetHealth(hp, BaseInst.HPMax);
        }

        public int GetHealth()
        {
            return BaseInst.HP;
        }

        public event NpcEvent OnRevive;


        public void SetHealth(int hp, int hpmax)
        {
            if (HP <= 0 && hp > 0)
            {
                OnRevive?.Invoke(this);
            }

            BaseInst.SetHealth(hp, hpmax);
            if (hp <= 0)
            {
                if (IsSpawned && !IsPlayer)
                {
                    World.DespawnList_NPC.AddVob(this);
                }

                if (unconTimer != null && unconTimer.Started)
                {
                    unconTimer.Stop();
                }
            }

            if (hp <= 0)
            {
                _Uncon = Unconsciousness.None;
                OnDeath?.Invoke(this);
                sOnDeath?.Invoke(this);
            }
        }

        public override void OnReadProperties(PacketReader stream)
        {
            base.OnReadProperties(stream);
            UseCustoms = stream.ReadBit();
            if (UseCustoms)
            {
                CustomBodyTex = (HumBodyTexs)stream.ReadByte();
                CustomHeadMesh = (HumHeadMeshs)stream.ReadByte();
                CustomHeadTex = (HumHeadTexs)stream.ReadByte();
                CustomVoice = (HumVoices)stream.ReadByte();
                CustomFatness = stream.ReadFloat();
                CustomScale = stream.ReadVec3f();
                CustomName = stream.ReadString();
            }

            _Uncon = (Unconsciousness)stream.ReadByte();
            TeamId = stream.ReadSByte();
        }

        // ARENA
        public int TeamId = -1;

        public override void OnWriteProperties(PacketWriter stream)
        {
            base.OnWriteProperties(stream);
            if (UseCustoms)
            {
                stream.Write(true);
                stream.Write((byte)CustomBodyTex);
                stream.Write((byte)CustomHeadMesh);
                stream.Write((byte)CustomHeadTex);
                stream.Write((byte)CustomVoice);
                stream.Write(CustomFatness);
                stream.Write(CustomScale);
                stream.Write(CustomName ?? "");
            }
            else
            {
                stream.Write(false);
            }

            stream.Write((byte)_Uncon);
            stream.Write((sbyte)TeamId);
        }


        public void SetFightMode(bool fightMode)
        {
            BaseInst.SetFightMode(fightMode);
        }



        public event NpcEvent OnDespawn;
        public event NpcEvent OnSpawn;


        public override void Despawn()
        {
            if (unconTimer != null && unconTimer.Started)
            {
                unconTimer.Stop();
            }
            base.Despawn();
            OnDespawn?.Invoke(this);
        }


        public override void Spawn(WorldInst world, Vec3f pos, Angles ang)
        {
            if (ModelDef.Visual != "HUMANS.MDS" && ModelDef.Visual != "ORC.MDS" && ModelDef.Visual != "DRACONIAN.MDS")
            {
                SetFightMode(true);
            }

            base.Spawn(world, pos, ang);
            OnSpawn?.Invoke(this);
        }

        public NpcEvent OnUnconChange;

        private Unconsciousness _Uncon = Unconsciousness.None;
        public bool IsUnconscious => _Uncon != Unconsciousness.None;

        public Allegiance Guild;

        public static readonly NPCRequestReceiver Requests = new NPCRequestReceiver();

        public delegate void NpcInstMoveHandler(NpcInst npc, Vec3f oldPos, Angles oldAng, NPCMovement oldMovement);

        public static event NpcInstMoveHandler SOnNpcInstMove;

        static NpcInst()
        {
            NPC.OnNPCMove += (npc, p, d, m) => SOnNpcInstMove((NpcInst)npc.ScriptObject, p, d, m);
            SOnNpcInstMove += (npc, p, d, m) => npc.ChangePosDir(p, d, m);
        }

        private float highestY;

        private Vec3f lastRegPos;

        private void ChangePosDir(Vec3f oldPos, Angles oldAng, NPCMovement oldMovement)
        {
            Vec3f pos = GetPosition();

            var env = Environment;

            if (env.InAir)
            {
                if (pos.Y > highestY)
                {
                    highestY = pos.Y;
                }
            }
            else if (highestY != 0)
            {
                float dmg = 0.14f * (highestY - pos.Y) - 135;
                if (dmg > 0)
                {
                    Logger.Log("Damage: " + dmg);
                    //this.SetHealth(this.HP - (int)dmg);
                    highestY = 0;
                }
            }


            if (lastRegPos.GetDistance(pos) > 30.0f)
            {
                LastHitMove = GameTime.Ticks;
                lastRegPos = pos;
            }

            if (FightAnimation != null && CanCombo && Movement != NPCMovement.Stand)
            {
                // so the npc can instantly stop the attack and run into a direction
                ModelInst.StopAnimation(FightAnimation, false);
            }


            if (env.WaterLevel > 0.7f)
            {
                if (IsPlayer)
                {
                    //var client = ((Arena.ArenaClient)this.Client);
                    //client.KillCharacter();
                    //if (Arena.GameModes.Horde.HordeMode.IsActive && this.TeamID >= 0)
                    //{
                    //    Arena.GameModes.Horde.HordeMode.ActiveMode.RespawnClient(client);
                    //}
                }
                else
                {
                    SetHealth(0);
                }
            }

            if (env.InAir && !IsClimbing)
            {
                var aa = ModelInst.GetActiveAniFromLayer(1);
                if (aa != null)
                {
                    ModelInst.StopAnimation(aa, false);
                }
            }


            CheckUnconsciousness();
        }

        private void CheckUnconsciousness()
        {
            if (!IsUnconscious || Environment.InAir)
            {
                return;
            }

            var cat = AniCatalog.Unconscious;
            var dropJob = _Uncon == Unconsciousness.Front ? cat.DropFront : cat.DropBack;
            if (dropJob == null)
            {
                return;
            }

            var aa = ModelInst.GetActiveAniFromLayer(1);
            if (aa != null)
            {
                var job = (ScriptAniJob)aa.AniJob.ScriptObject;
                if (job == dropJob || job == dropJob.NextAni)
                {
                    return;
                }

                var standJob = _Uncon == Unconsciousness.Front ? cat.StandUpFront : cat.StandUpBack;
                if (standJob != null && standJob == job)
                {
                    return;
                }
            }

            ModelInst.StartAniJob(dropJob);
        }

        public NpcInst(NpcDef def) : base(def)
        {
        }



        public NPCCatalog AniCatalog => (NPCCatalog)ModelDef?.Catalog;

        public bool IsPlayer => BaseInst.IsPlayer;

        public Client.Client Client => (Client.Client)BaseInst.Client?.ScriptObject;

        public bool TryGetControllingClient(out Client.Client client)
        {
            client = Client;
            return client != null;
        }

        /// <summary>
        ///     Starts an uncontrolled jump animation, throws the npc with velocity.
        /// </summary>
        /// <param name="move"></param>
        /// <param name="velocity"></param>
        public void DoJump(JumpMoves move, Vec3f velocity)
        {
            ScriptAniJob job;
            switch (move)
            {
                case JumpMoves.Fwd:
                    job = AniCatalog.Jumps.Fwd;
                    break;
                case JumpMoves.Run:
                    job = AniCatalog.Jumps.Run;
                    break;
                case JumpMoves.Up:
                    job = AniCatalog.Jumps.Up;
                    break;
                default:
                    Logger.Log("Not existing jump move: " + move);
                    return;
            }

            if (job == null)
            {
                return;
            }

            ModelInst.StartAniJobUncontrolled(job);
            Throw(velocity);
        }

        public bool IsClimbing { get; private set; }

        public void DoClimb(ClimbMoves move, NPC.ClimbingLedge ledge)
        {
            ScriptAniJob job;
            switch (move)
            {
                case ClimbMoves.High:
                    job = AniCatalog.Climbs.High;
                    break;
                case ClimbMoves.Mid:
                    job = AniCatalog.Climbs.Mid;
                    break;
                case ClimbMoves.Low:
                    job = AniCatalog.Climbs.Low;
                    break;
                default:
                    Logger.Log("Not existing climb move: " + move);
                    return;
            }

            if (job == null)
            {
                return;
            }


            var stream = BaseInst.GetScriptVobStream();
            stream.Write((byte)ScriptVobMessageIDs.Climb);
            ledge.WriteStream(stream);
            BaseInst.SendScriptVobStream(stream);

            IsClimbing = true;
            ModelInst.StartAniJob(job, () => IsClimbing = false);
        }

        public void DoDropItem(ItemInst item, int amount, Vec3f position, Angles angles)
        {
            item = item.Split(amount);

            ScriptAniJob job = AniCatalog?.ItemHandling.DropItem;
            if (job != null && ModelInst.TryGetAniFromJob(job, out ScriptAni ani))
            {
                if (!ani.TryGetSpecialFrame(SpecialFrame.ItemHandle, out float frame))
                {
                    frame = float.MaxValue;
                }

                var pair = new FrameActionPair(frame, () => DropItem(item, position, angles));
                ModelInst.StartAniJob(job, 0.8f, 0, pair);
                return;
            }

            DropItem(item, position, angles);
        }

        private void DropItem(ItemInst item, Vec3f position, Angles angles)
        {
            item.Spawn(World, position, angles);
            item.BaseInst.SetNeedsClientGuide(true);
            item.Throw(Vec3f.Null);
        }

        public void DoTakeItem(ItemInst item)
        {
            ScriptAniJob job = AniCatalog?.ItemHandling.TakeItem;
            if (job != null && ModelInst.TryGetAniFromJob(job, out ScriptAni ani))
            {
                if (!ani.TryGetSpecialFrame(SpecialFrame.ItemHandle, out float frame))
                {
                    frame = float.MaxValue;
                }

                var pair = new FrameActionPair(frame, () => TakeItem(item));
                ModelInst.StartAniJob(job, 0.8f, 0, pair);
                return;
            }

            TakeItem(item);
        }

        private void TakeItem(ItemInst item)
        {
            if (item != null && item.IsSpawned)
            {
                // remove item in world
                item.Despawn();
                bool add = true;

                // stack items of the same kind
                Inventory.ForEachItemPredicate(invItem =>
                {
                    if (invItem.Definition == item.Definition)
                    {
                        invItem.SetAmount(invItem.Amount + item.Amount);
                        add = false;
                        return false;
                    }

                    return true;
                });

                if (add)
                {
                    Inventory.AddItem(item);

                    // check if this is ammo we need
                    ItemInst rangedWep = GetEquipmentBySlot(NPCSlots.Ranged);
                    if (rangedWep != null && GetEquipmentBySlot(NPCSlots.Ammo) == null
                                          && (item.ItemType == ItemTypes.AmmoBow &&
                                              rangedWep.ItemType == ItemTypes.WepBow
                                              || item.ItemType == ItemTypes.AmmoXBow &&
                                              rangedWep.ItemType == ItemTypes.WepXBow))
                    {
                        EquipItem(NPCSlots.Ammo, item);
                    }
                }
            }
        }

        public ActiveAni FightAnimation { get; private set; }

        public int ComboNum { get; private set; }

        public bool CanCombo { get; private set; } = true;

        public FightMoves CurrentFightMove { get; private set; } = FightMoves.None;

        public void DoFightMove(FightMoves move, int combo = 0)
        {
            NPCCatalog.FightAnis fightCatalog;
            var drawnWeapon = GetDrawnWeapon();
            if (drawnWeapon == null)
            {
                fightCatalog = AniCatalog.FightFist;
            }
            else
            {
                switch (drawnWeapon.ItemType)
                {
                    default:
                    case ItemTypes.Wep1H:
                        fightCatalog = AniCatalog.Fight1H;
                        break;
                    case ItemTypes.Wep2H:
                        fightCatalog = AniCatalog.Fight2H;
                        break;
                }
            }

            switch (move)
            {
                case FightMoves.Fwd:
                    DoAttack(fightCatalog.Fwd[combo], move, combo);
                    break;
                case FightMoves.Run:
                    DoAttack(fightCatalog.Run, move);
                    break;
                case FightMoves.Left:
                    DoAttack(fightCatalog.Left, move);
                    break;
                case FightMoves.Right:
                    DoAttack(fightCatalog.Right, move);
                    break;
                case FightMoves.Parry:
                    DoParry(fightCatalog.GetRandomParry());
                    break;
                case FightMoves.Dodge:
                    DoDodge(fightCatalog.Dodge);
                    break;
            }
        }

        private void DoAttack(ScriptAniJob job, FightMoves move, int fwdCombo = 0)
        {
            if (job == null)
            {
                return;
            }

            if (!ModelInst.TryGetAniFromJob(job, out ScriptAni ani))
            {
                return;
            }

            // combo window
            if (!ani.TryGetSpecialFrame(SpecialFrame.Combo, out float comboFrame))
            {
                comboFrame = ani.EndFrame;
            }

            var comboPair = new FrameActionPair(comboFrame, () => OpenCombo());

            // hit frame
            if (!ani.TryGetSpecialFrame(SpecialFrame.Hit, out float hitFrame))
            {
                hitFrame = comboFrame;
            }

            if (hitFrame > comboFrame)
            {
                hitFrame = comboFrame;
            }

            var hitPair = new FrameActionPair(hitFrame, () => CalcHit());

            // end of animation
            var endPair = FrameActionPair.OnEnd(() => EndFightAni());

            // start ani first, because the OnEnd-Callback from the former attack resets the fight stance
            FightAnimation = ModelInst.StartAniJob(job, comboPair, hitPair, endPair);
            CurrentFightMove = move;
            CanCombo = false;
            ComboNum = fwdCombo;
        }

        private void OpenCombo()
        {
            if (Movement != NPCMovement.Stand && FightAnimation != null)
            {
                // so the npc can instantly stop the attack and run into a direction
                ModelInst.StopAnimation(FightAnimation, false);
            }
            else
            {
                CanCombo = true;
            }
        }

        private void EndFightAni()
        {
            CurrentFightMove = FightMoves.None;
            CanCombo = true;
            ComboNum = 0;
            FightAnimation = null;
        }

        private void DoParry(ScriptAniJob job)
        {
            if (job == null)
            {
                return;
            }

            if (!ModelInst.TryGetAniFromJob(job, out ScriptAni ani))
            {
                return;
            }

            // end of animation
            var endPair = FrameActionPair.OnEnd(() => EndFightAni());

            FightAnimation = ModelInst.StartAniJob(job, endPair);
            CurrentFightMove = FightMoves.Parry;
            CanCombo = false;
            ComboNum = 0;
        }

        private void DoDodge(ScriptAniJob job)
        {
            if (job == null)
            {
                return;
            }

            if (!ModelInst.TryGetAniFromJob(job, out ScriptAni ani))
            {
                return;
            }

            // end of animation
            var endPair = FrameActionPair.OnEnd(() => EndFightAni());

            FightAnimation = ModelInst.StartAniJob(job, endPair);
            CurrentFightMove = FightMoves.Dodge;
            CanCombo = false;
            ComboNum = 0;
        }

        public void DoDrawWeapon(ItemInst item)
        {
            NPCCatalog.DrawWeaponAnis catalog;
            switch (item.ItemType)
            {
                default:
                case ItemTypes.Wep1H:
                    catalog = AniCatalog.Draw1H;
                    break;
                case ItemTypes.Wep2H:
                    catalog = AniCatalog.Draw2H;
                    break;
                case ItemTypes.WepBow:
                    catalog = AniCatalog.DrawBow;
                    break;
                case ItemTypes.WepXBow:
                    catalog = AniCatalog.DrawXBow;
                    break;
                case ItemTypes.Rune:
                case ItemTypes.Scroll:
                    catalog = AniCatalog.DrawMagic;
                    break;
            }

            if (IsInFightMode || GetDrawnWeapon() != null)
            {
                /*if (this.DrawnWeapon != null)
                {
                    ItemInst weapon = this.DrawnWeapon;
                    catalog = GetDrawWeaponCatalog(weapon.ItemType);
                    // weapon draw while running or when standing
                    if (this.Movement == NPCMovement.Forward || this.Movement == NPCMovement.Left || this.Movement == NPCMovement.Right)
                    {
                        this.ModelInst.StartAniJob(catalog.UndrawWhileRunning, 0.1f, () =>
                        {
                            this.UnequipItem(weapon); // take weapon from hand
                            this.EquipItem(weapon); // place weapon into parking slot
                        });
                    }
                    else
                    {
                        this.ModelInst.StartAniJob(catalog.Undraw, 0.1f, () =>
                        {
                            this.UnequipItem(weapon); // take weapon from hand
                            this.EquipItem(weapon); // place weapon into parking slot
                        });
                    }
                }
                this.SetFightMode(false);*/
            }
            else
            {
                ScriptAniJob job = Movement == NPCMovement.Stand && !Environment.InAir
                    ? catalog.Draw
                    : catalog.DrawWhileRunning;
                if (job == null || !ModelInst.TryGetAniFromJob(job, out ScriptAni ani)) // no animation
                {
                    DrawWeapon(item);
                }
                else
                {
                    if (!ani.TryGetSpecialFrame(SpecialFrame.Draw, out float drawFrame))
                    {
                        drawFrame = 0;
                    }

                    ModelInst.StartAniJob(job, new FrameActionPair(drawFrame, () => DrawWeapon(item)));
                }
            }
        }

        /// <summary>
        ///     Instantly puts the item in the hand and activates fight mode
        /// </summary>
        private void DrawWeapon(ItemInst item)
        {
            if (item.ItemType == ItemTypes.WepBow)
            {
                EquipItem(NPCSlots.LeftHand, item);
                var ammo = GetAmmo();
                if (ammo != null)
                {
                    EquipItem(NPCSlots.RightHand, ammo);
                }
            }
            else if (item.ItemType == ItemTypes.WepXBow)
            {
                EquipItem(NPCSlots.RightHand, item);
                var ammo = GetAmmo();
                if (ammo != null)
                {
                    EquipItem(NPCSlots.LeftHand, ammo);
                }
            }
            else
            {
                EquipItem(NPCSlots.RightHand, item); // put weapon into hand
                var melee2 = GetEquipmentBySlot(NPCSlots.OneHanded2);
                if (melee2 != null)
                {
                    EquipItem(NPCSlots.LeftHand, melee2);
                }
            }

            SetFightMode(true); // look angry
        }

        public void DoUndrawWeapon(ItemInst item)
        {
            NPCCatalog.DrawWeaponAnis catalog;
            switch (item.ItemType)
            {
                default:
                case ItemTypes.Wep1H:
                    catalog = AniCatalog.Draw1H;
                    break;
                case ItemTypes.Wep2H:
                    catalog = AniCatalog.Draw2H;
                    break;
                case ItemTypes.WepBow:
                    catalog = AniCatalog.DrawBow;
                    break;
                case ItemTypes.WepXBow:
                    catalog = AniCatalog.DrawXBow;
                    break;
                case ItemTypes.Rune:
                case ItemTypes.Scroll:
                    catalog = AniCatalog.DrawMagic;
                    break;
            }

            if (GetDrawnWeapon() == null)
            {
            }
            else
            {
                ScriptAniJob job = Movement == NPCMovement.Stand && !Environment.InAir
                    ? catalog.Undraw
                    : catalog.UndrawWhileRunning;
                if (job == null || !ModelInst.TryGetAniFromJob(job, out ScriptAni ani)) // no animation
                {
                    UndrawWeapon(item);
                }
                else
                {
                    if (!ani.TryGetSpecialFrame(SpecialFrame.Draw, out float drawFrame))
                    {
                        drawFrame = 0;
                    }

                    ModelInst.StartAniJob(job, new FrameActionPair(drawFrame, () => UndrawWeapon(item)));
                }
            }
        }

        /// <summary>
        ///     Instantly puts the item back and deactivates fight mode
        /// </summary>
        private void UndrawWeapon(ItemInst item)
        {
            switch (item.ItemType)
            {
                case ItemTypes.Wep1H:
                    EquipItem(NPCSlots.OneHanded1, item);
                    var other1H = GetLeftHand();
                    if (other1H != null && other1H.ItemType == ItemTypes.Wep1H)
                    {
                        EquipItem(NPCSlots.OneHanded2, other1H);
                    }

                    break;
                case ItemTypes.Wep2H:
                    EquipItem(NPCSlots.TwoHanded, item);
                    break;
                case ItemTypes.WepBow:
                    EquipItem(NPCSlots.Ranged, item);
                    var ammo = GetRightHand();
                    if (ammo != null && ammo.ItemType == ItemTypes.AmmoBow)
                    {
                        EquipItem(NPCSlots.Ammo, ammo);
                    }

                    break;
                case ItemTypes.WepXBow:
                    EquipItem(NPCSlots.Ranged, item);
                    ammo = GetLeftHand();
                    if (ammo != null && ammo.ItemType == ItemTypes.AmmoXBow)
                    {
                        EquipItem(NPCSlots.Ammo, ammo);
                    }

                    break;
            }

            SetFightMode(false);
        }

        /// <summary>
        ///     Plays the draw animation and activates fight mode
        /// </summary>
        public void DoDrawFists()
        {
            ScriptAniJob drawAniJob;
            if (Movement == NPCMovement.Stand && !Environment.InAir && Environment.WaterLevel < 0.4f)
            {
                drawAniJob = AniCatalog.DrawFists.Draw;
            }
            else
            {
                drawAniJob = AniCatalog.DrawFists.DrawWhileRunning;
            }

            if (drawAniJob == null)
            {
                return;
            }

            if (!ModelInst.TryGetAniFromJob(drawAniJob, out ScriptAni ani))
            {
                return;
            }

            if (!ani.TryGetSpecialFrame(0, out float drawFrame))
            {
                drawFrame = float.MaxValue;
            }

            ModelInst.StartAniJob(drawAniJob, new FrameActionPair(drawFrame, () => SetFightMode(true)));
        }

        public void DoUndrawFists()
        {
            ScriptAniJob undrawAniJob;
            if (Movement == NPCMovement.Stand && !Environment.InAir && Environment.WaterLevel < 0.4f)
            {
                undrawAniJob = AniCatalog.DrawFists.Undraw;
            }
            else
            {
                undrawAniJob = AniCatalog.DrawFists.UndrawWhileRunning;
            }

            if (undrawAniJob == null)
            {
                return;
            }

            if (!ModelInst.TryGetAniFromJob(undrawAniJob, out ScriptAni ani))
            {
                return;
            }

            if (!ani.TryGetSpecialFrame(0, out float undrawFrame))
            {
                undrawFrame = float.MaxValue;
            }

            ModelInst.StartAniJob(undrawAniJob, new FrameActionPair(undrawFrame, () => SetFightMode(false)));
        }

        public NPCCatalog.DrawWeaponAnis GetDrawWeaponCatalog(ItemTypes itemType)
        {
            switch (itemType)
            {
                case ItemTypes.Wep1H:
                    return AniCatalog.Draw1H;
                case ItemTypes.Wep2H:
                    return AniCatalog.Draw2H;
                case ItemTypes.WepBow:
                    return AniCatalog.DrawBow;
                case ItemTypes.WepXBow:
                    return AniCatalog.DrawXBow;
                case ItemTypes.Rune:
                    return AniCatalog.DrawMagic;
                case ItemTypes.Scroll:
                    return AniCatalog.DrawMagic;
                default:
                    return AniCatalog.Draw1H;
            }
        }

        public int Damage;
        public int Protection;

        public long LastHitMove { get; private set; }




        public static event OnHitHandler sOnHit;
        public event OnHitHandler OnHit;

        public void Hit(NpcInst attacker, int damage, bool fromFront = true)
        {
            var strm = BaseInst.GetScriptVobStream();
            strm.Write((byte)ScriptVobMessageIDs.HitMessage);
            strm.Write((ushort)attacker.ID);
            BaseInst.SendScriptVobStream(strm);

            int protection = Protection;
            var armor = GetArmor();
            if (armor != null)
            {
                protection += armor.Protection;
            }

            // two weapons
            ItemInst otherMelee;
            if ((otherMelee = GetLeftHand()) != null && otherMelee.ItemType == ItemTypes.Wep1H)
            {
                protection -= otherMelee.Damage / 4;
            }

            damage -= protection;


            if (damage <= 0)
            {
                damage = 1;
            }

            int resultingHP = GetHealth() - damage;

            if (DropUnconsciousOnDeath && resultingHP <= 1)
            {
                resultingHP = 1;
                DropUnconscious(UnconsciousDuration, !fromFront);
            }

            SetHealth(resultingHP);
            sOnHit?.Invoke(attacker, this, damage);
            OnHit?.Invoke(attacker, this, damage);
            LastHitMove = GameTime.Ticks;
        }

        public float GetFightRange()
        {
            ItemInst drawnWeapon = GetDrawnWeapon();
            return ModelDef.Radius + (drawnWeapon?.Definition.Range ?? ModelDef.FistRange);
        }

        /// <summary> Skips hit determination if false is returned. Arguments: Attacker, Target </summary>
        public static BoolEvent<NpcInst, NpcInst> AllowHitEvent;

        /// <summary> Skips hit determination if attacker returns false. Arguments: Attacker, Target </summary>
        public BoolEvent<NpcInst, NpcInst> AllowHitAttacker;

        // <summary> Skips hit determination if target returns false. Arguments: Attacker, Target </summary>
        public BoolEvent<NpcInst, NpcInst> AllowHitTarget;

        private void CalcHit()
        {
            try
            {
                if (IsDead || FightAnimation == null || IsUnconscious)
                {
                    return;
                }

                Vec3f attPos = GetPosition();
                Angles attAng = GetAngles();

                int baseDamage = 5 + Damage;

                ItemInst weapon;
                if ((weapon = GetDrawnWeapon()) != null)
                {
                    baseDamage += weapon.Damage;
                }

                // two weapons
                if ((weapon = GetLeftHand()) != null && weapon.ItemType == ItemTypes.Wep1H)
                {
                    baseDamage += weapon.Damage / 4;
                }

                float weaponRange = GetFightRange();
                BaseInst.World.ForEachNPCRough(attPos, GUCScripts.BiggestNPCRadius + weaponRange,
                    npc => // fixme: enemy model radius
                    {
                        NpcInst target = (NpcInst)npc.ScriptObject;
                        if (target == this || target.IsDead || target.IsUnconscious)
                        {
                            return;
                        }

                        if (!AllowHitEvent.TrueForAll(this, target)
                            || !AllowHitAttacker.TrueForAll(this, target) ||
                            !target.AllowHitTarget.TrueForAll(this, target))
                        {
                            return;
                        }

                        float realRange = weaponRange + target.ModelDef.Radius;
                        if (target.CurrentFightMove == FightMoves.Dodge)
                        {
                            realRange /= 3.0f; // decrease radius if target is backing up
                        }

                        Vec3f targetPos = npc.Position + npc.GetAtVector() * target.ModelDef.CenterOffset;

                        if ((targetPos - attPos).GetLength() > realRange)
                        {
                            return; // not in range
                        }

                        float hitHeight;
                        float hitYaw;
                        if (CurrentFightMove == FightMoves.Left || CurrentFightMove == FightMoves.Right)
                        {
                            hitHeight = target.ModelDef.HalfHeight;
                            hitYaw = Angles.PI * 0.4f;
                        }
                        else
                        {
                            hitHeight = target.ModelDef.HalfHeight + ModelDef.HalfHeight;
                            hitYaw = Angles.PI * 0.2f;
                        }

                        if (Math.Abs(targetPos.Y - attPos.Y) > hitHeight)
                        {
                            return; // not same height
                        }

                        float yaw = Angles.GetYawFromAtVector(targetPos - attPos);
                        if (Math.Abs(Angles.Difference(yaw, attAng.Yaw)) > hitYaw)
                        {
                            return; // target is not in front of attacker
                        }

                        float tdiff = Math.Abs(Angles.Difference(target.GetAngles().Yaw, yaw));
                        if (target.CurrentFightMove == FightMoves.Parry && tdiff > Angles.PI / 2) // parry 180 degrees
                        {
                            var strm = BaseInst.GetScriptVobStream();
                            strm.Write((byte)ScriptVobMessageIDs.ParryMessage);
                            strm.Write((ushort)npc.ID);
                            BaseInst.SendScriptVobStream(strm);
                        }
                        else // HIT
                        {
                            int damage = baseDamage;
                            if (CurrentFightMove == FightMoves.Left || CurrentFightMove == FightMoves.Right)
                            {
                                damage -= 2;
                            }
                            else if (CurrentFightMove == FightMoves.Fwd)
                            {
                                damage += (ComboNum - 1) * 2;
                            }
                            else if (CurrentFightMove == FightMoves.Run)
                            {
                                damage += 6;
                                if (Environment.InAir) // super jump attack
                                {
                                    damage += 2; // not too much because you can always jump
                                }

                                if (target.Environment.InAir)
                                {
                                    damage += 2;
                                }
                            }

                            bool frontAttack;
                            if (tdiff < Angles.PI / 4) // backstab
                            {
                                damage += 4;
                                frontAttack = false;
                            }
                            else
                            {
                                frontAttack = true;
                            }

                            target.Hit(this, damage, frontAttack);
                        }
                    });
            }
            catch (Exception e)
            {
                Logger.Log("CalcHit of npc " + ID + " " + BaseInst.HP + " " + e);
            }
        }


        public bool IsAiming()
        {
            var drawnWeapon = GetDrawnWeapon();
            if (drawnWeapon != null && drawnWeapon.IsWepRanged)
            {
                var aa = ModelInst.GetActiveAniFromLayer(1);
                if (aa != null)
                {
                    var scriptJob = aa.AniJob.ScriptObject;
                    return scriptJob == AniCatalog.FightBow.Aiming || scriptJob == AniCatalog.FightXBow.Aiming;
                }
            }

            return false;
        }

        public void DoAim()
        {
            var drawnWeapon = GetDrawnWeapon();
            ScriptAniJob job = drawnWeapon != null && drawnWeapon.ItemType == ItemTypes.WepXBow
                ? AniCatalog.FightXBow.Aim
                : AniCatalog.FightBow.Aim;
            if (job == null || !ModelInst.TryGetAniFromJob(job, out ScriptAni ani))
            {
            }
            else
            {
                ModelInst.StartAniJob(job);
            }
        }

        public void DoUnaim()
        {
            var drawnWeapon = GetDrawnWeapon();
            ScriptAniJob job = drawnWeapon != null && drawnWeapon.ItemType == ItemTypes.WepXBow
                ? AniCatalog.FightXBow.Unaim
                : AniCatalog.FightBow.Unaim;
            if (job == null || !ModelInst.TryGetAniFromJob(job, out ScriptAni ani))
            {
            }
            else
            {
                ModelInst.StartAniJob(job);
            }
        }

        public void DoShoot(Vec3f start, Vec3f end, ProjInst proj)
        {
            proj.Destination = end;
            proj.Shooter = this;

            Angles rotation = Angles.FromAtVector(end - start);
            var p = rotation.Pitch; // rotate arrow correctly
            rotation.Pitch = rotation.Roll;
            rotation.Yaw -= Angles.PI / 2;
            rotation.Roll = p;

            proj.Spawn(World, start, rotation);

            var drawnWeapon = GetDrawnWeapon();
            ScriptAniJob job = drawnWeapon != null && drawnWeapon.ItemType == ItemTypes.WepXBow
                ? AniCatalog.FightXBow.Reload
                : AniCatalog.FightBow.Reload;
            if (job != null && ModelInst.TryGetAniFromJob(job, out ScriptAni ani))
            {
                ModelInst.StartAniJob(job);
            }
        }

        public bool DropUnconsciousOnDeath = false;
        public long UnconsciousDuration = 15 * TimeSpan.TicksPerSecond;
        private GUCTimer unconTimer;

        public void DropUnconscious(long duration = -1, bool toFront = true)
        {
            var cat = AniCatalog.Unconscious;
            ScriptAniJob job = toFront ? cat.DropFront : cat.DropBack;
            if (job != null)
            {
                ModelInst.StartAniJob(job);
            }

            _Uncon = toFront ? Unconsciousness.Front : Unconsciousness.Back;

            var strm = BaseInst.GetScriptVobStream();
            strm.Write((byte)ScriptVobMessageIDs.Uncon);
            strm.Write((byte)_Uncon);
            BaseInst.SendScriptVobStream(strm);

            if (duration >= 0)
            {
                if (unconTimer == null)
                {
                    unconTimer = new GUCTimer(LiftUnconsciousness);
                }

                unconTimer.SetInterval(duration);
                unconTimer.Start();
            }

            OnUnconChange?.Invoke(this);
        }

        public void LiftUnconsciousness()
        {
            if (!IsUnconscious)
            {
                return;
            }

            var cat = AniCatalog.Unconscious;
            ScriptAniJob job = _Uncon == Unconsciousness.Front ? cat.StandUpFront : cat.StandUpBack;
            if (job != null)
            {
                ModelInst.StartAniJob(job, DoLiftUncon);
            }
            else
            {
                DoLiftUncon();
            }

            if (unconTimer != null && unconTimer.Started)
            {
                unconTimer.Stop();
            }
        }

        private void DoLiftUncon()
        {
            _Uncon = Unconsciousness.None;
            var strm = BaseInst.GetScriptVobStream();
            strm.Write((byte)ScriptVobMessageIDs.Uncon);
            strm.Write((byte)_Uncon);
            BaseInst.SendScriptVobStream(strm);

            int hp = HP + 25;
            SetHealth(hp > HPMax ? HPMax : hp);
            OnUnconChange?.Invoke(this);
        }

        public void UseItem(ItemInst item)
        {
            if (item.ItemType != ItemTypes.Drinkable)
            {
                return;
            }

            ScriptAniJob job = AniCatalog?.ItemHandling.DrinkPotion;
            if (job != null && ModelInst.TryGetAniFromJob(job, out ScriptAni ani))
            {
                if (!ani.TryGetSpecialFrame(SpecialFrame.ItemHandle, out float frame))
                {
                    frame = float.MaxValue;
                }

                EquipItem(NPCSlots.RightHand, item);
                var pair = new FrameActionPair(frame, () => ChugPotion(item));
                ModelInst.StartAniJob(job, pair);
                return;
            }

            ChugPotion(item);
        }

        private void ChugPotion(ItemInst item)
        {
            if (item == null)
            {
                return;
            }

            int hp = HP + 50;
            SetHealth(hp > HPMax ? HPMax : hp);
            if (item.IsEquipped)
            {
                UnequipItem(item);
            }

            item.SetAmount(item.Amount - 1);
        }

        public bool IsObstructed()
        {
            return IsSpawned && (IsDead || Movement != NPCMovement.Stand || ModelInst.IsInAnimation() ||
                                 Environment.InAir || IsInFightMode || IsUnconscious);
        }

        public void RandomizeCustomVisuals(string name, bool male)
        {
            if (male)
            {
                CustomBodyTex = (HumBodyTexs)Randomizer.GetInt(0, 4);
                CustomHeadMesh = (HumHeadMeshs)Randomizer.GetInt(6);
                CustomVoice = (HumVoices)Randomizer.GetInt(15);
                switch (CustomBodyTex)
                {
                    case HumBodyTexs.M_Pale:
                        CustomHeadTex = (HumHeadTexs)Randomizer.GetInt(41, 58);
                        break;
                    case HumBodyTexs.M_Normal:
                    case HumBodyTexs.G1Hero:
                    case HumBodyTexs.G2Hero:
                    case HumBodyTexs.M_Tattooed:
                        CustomHeadTex = (HumHeadTexs)Randomizer.GetInt(58, 120);
                        break;
                    case HumBodyTexs.M_Latino:
                        CustomHeadTex = (HumHeadTexs)Randomizer.GetInt(120, 129);
                        break;
                    case HumBodyTexs.M_Black:
                        CustomHeadTex = (HumHeadTexs)Randomizer.GetInt(129, 137);
                        break;
                }
            }

            var size = Randomizer.GetFloat(0.95f, 1.05f);
            CustomFatness = Randomizer.GetFloat(-1, 1);
            CustomScale = new Vec3f(size, 1.0f, size);
            CustomName = name;
            UseCustoms = true;
        }

        public void DoVoice(VoiceCmd cmd, bool shout = false)
        {
            var strm = BaseInst.GetScriptVobStream();
            strm.Write((byte)(shout ? ScriptVobMessageIDs.VoiceShout : ScriptVobMessageIDs.Voice));
            strm.Write((byte)cmd);
            BaseInst.SendScriptVobStream(strm);
        }
    }
}