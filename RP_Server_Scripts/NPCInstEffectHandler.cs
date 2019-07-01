//using System;
//using System.Collections.Generic;
//using GUC.Scripts.CleanAPI.Implementation.VobSystem.Definitions;
//using GUC.Scripts.CleanAPI.Implementation.VobSystem.Instances;
//using GUC.Scripts.CleanAPI.Interface.VobSystem.Definitions.Item;
//using GUC.Scripts.CleanAPI.Interface.VobSystem.Instances;
//using GUC.Scripts.Sumpfkraut.Visuals;
//using GUC.Scripts.Sumpfkraut.VobSystem.Enumeration;
//using GUC.Types;
//using GUC.Utilities;
//using GUC.WorldObjects;

//namespace GUC.Scripts.Sumpfkraut.EffectSystem.EffectHandlers
//{
//    public class NPCInstEffectHandler
//    {
//        public new NpcInst Host => (NpcInst) host;


//        static NPCInstEffectHandler()
//        {
//            PrintStatic(typeof(NPCInstEffectHandler), "Start subscribing ChangeDestinations and EventHandler...");

//            //NPCInst.sOnHit += OnHit;

//            PrintStatic(typeof(NPCInstEffectHandler), "Finished subscribing ChangeDestinations and EventHandler...");
//        }


//        public NPCInstEffectHandler(List<Effect> effects, NpcInst host)
//            : this("NPCInstEffectHandler", effects, host)
//        {
//        }

//        public NPCInstEffectHandler(string objName, List<Effect> effects, NpcInst host)
//            : base(objName, effects, host)
//        {
//        }

//        private static void OnHit(NpcInst attacker, NpcInst target, int damage)
//        {
//            throw new NotImplementedException();
//        }

//        private readonly LockTimer jumpLockTimer = new LockTimer(500);

//        public void TryJump(JumpMoves move)
//        {
//            if (Host.IsDead || Host.Environment.InAir)
//            {
//                return;
//            }

//            if (Host.ModelInst.GetActiveAniFromLayer(1) != null)
//            {
//                return;
//            }

//            if (!jumpLockTimer.IsReady) // don't spam
//            {
//                return;
//            }

//            Host.DoJump(move, new Vec3f(0, move == JumpMoves.Fwd ? 300 : 250, 0));
//        }

//        internal void TryClimb(ClimbMoves move, NPC.ClimbingLedge ledge)
//        {
//            if (Host.IsDead || Host.Environment.InAir)
//            {
//                return;
//            }

//            if (Host.ModelInst.IsInAnimation())
//            {
//                return;
//            }

//            if (!jumpLockTimer.IsReady) // don't spam
//            {
//                return;
//            }

//            Host.DoClimb(move, ledge);
//        }

//        public void TryDrawFists()
//        {
//            if (Host.ModelDef.Visual != "HUMANS.MDS")
//            {
//                return;
//            }

//            if (Host.IsDead || Host.ModelInst.IsInAnimation())
//            {
//                return;
//            }

//            if (Host.IsInFightMode)
//            {
//                if (Host.GetDrawnWeapon() != null)
//                {
//                    //this.UnequipItem(this.DrawnWeapon);
//                }
//                else
//                {
//                    Host.DoUndrawFists();
//                }
//            }
//            else
//            {
//                Host.DoDrawFists();
//            }
//        }

//        public void TryDrawWeapon(IItemInst item)
//        {
//            if (item == null)
//            {
//                return;
//            }

//            if (Host.IsDead || Host.ModelInst.IsInAnimation())
//            {
//                return;
//            }

//            if (Host.GetDrawnWeapon() != null)
//            {
//                Host.DoUndrawWeapon(item);
//            }

//            IItemInst removeItem = Host.GetRightHand();
//            if (removeItem != null)
//            {
//                if (removeItem.IsWeapon)
//                {
//                    Host.DoUndrawWeapon(removeItem);
//                    return;
//                }

//                TryUnequipItem(removeItem);
//                if (removeItem.IsEquipped) // still equipped?
//                {
//                    return; // dunno
//                }
//            }

//            removeItem = Host.GetLeftHand();
//            if (removeItem != null &&
//                (item.ItemType != ItemTypes.Wep1H || Host.GetEquipmentBySlot(NPCSlots.OneHanded2) != null))
//            {
//                if (removeItem.IsWeapon)
//                {
//                    Host.DoUndrawWeapon(removeItem);
//                    return;
//                }

//                TryUnequipItem(removeItem);
//                if (removeItem.IsEquipped) // still equipped?
//                {
//                    return; // dunno
//                }
//            }

//            // Hands should be free now
//            Host.DoDrawWeapon(item);
//        }

//        public void TryUndrawWeapon(IItemInst item)
//        {
//            if (item == null)
//            {
//                return;
//            }

//            if (Host.IsDead || Host.ModelInst.IsInAnimation())
//            {
//                return;
//            }

//            if (Host.GetRightHand() == item || Host.GetLeftHand() == item)
//            {
//                Host.DoUndrawWeapon(item);
//            }
//        }

//        public void TryFightMove(FightMoves move)
//        {
//            if (Host.IsDead || !Host.IsInFightMode
//                            || Host.Environment.InAir && move != FightMoves.Run
//                            || Host.ModelInst.GetActiveAniFromLayer(2) != null)
//            {
//                return;
//            }

//            var meleeWep = Host.GetDrawnWeapon();
//            if (meleeWep != null && !meleeWep.IsWepMelee)
//            {
//                return;
//            }

//            var otherAni = Host.ModelInst.GetActiveAniFromLayer(1);
//            if (otherAni != null && otherAni != Host.FightAnimation)
//            {
//                return;
//            }

//            if (Host.FightAnimation != null)
//            {
//                if (Host.CanCombo)
//                {
//                    var current = Host.CurrentFightMove;
//                    if (current == FightMoves.Right && move == FightMoves.Right)
//                    {
//                        return;
//                    }

//                    if (current == FightMoves.Left && move == FightMoves.Left)
//                    {
//                        return;
//                    }

//                    Host.DoFightMove(move, current == FightMoves.Fwd ? Host.ComboNum + 1 : 0);
//                }
//            }
//            else
//            {
//                Host.DoFightMove(move, 0);
//            }
//        }

//        /// <summary> Player equips item through inventory </summary>
//        public void TryEquipItem(IItemInst item)
//        {
//            if (item == null || Host.IsObstructed())
//            {
//                return;
//            }

//            NPCSlots slot;
//            switch (item.ItemType)
//            {
//                case ItemTypes.AmmoBow:
//                case ItemTypes.AmmoXBow:
//                    slot = NPCSlots.Ammo;
//                    break;
//                case ItemTypes.Armor:
//                    slot = NPCSlots.Armor;
//                    break;
//                case ItemTypes.Wep1H: // FIXME: Let player choose the slots
//                    if (Host.GetEquipmentBySlot(NPCSlots.OneHanded1) == null ||
//                        Host.GetEquipmentBySlot(NPCSlots.OneHanded2) != null)
//                    {
//                        slot = NPCSlots.OneHanded1;
//                    }
//                    else
//                    {
//                        slot = NPCSlots.OneHanded2;
//                    }

//                    Host.UnequipSlot(NPCSlots.TwoHanded);
//                    break;
//                case ItemTypes.Wep2H:
//                    slot = NPCSlots.TwoHanded;
//                    Host.UnequipSlot(NPCSlots.OneHanded1);
//                    Host.UnequipSlot(NPCSlots.OneHanded2);
//                    break;
//                case ItemTypes.WepBow:
//                case ItemTypes.WepXBow:
//                    slot = NPCSlots.Ranged;
//                    break;
//                case ItemTypes.Torch:
//                    slot = NPCSlots.LeftHand;
//                    if (Host.ModelDef.TryGetOverlay("humans_torch", out ScriptOverlay ov))
//                    {
//                        Host.ModelInst.ApplyOverlay(ov);
//                    }

//                    break;
//                default:
//                    return;
//            }

//            IItemInst otherItem = Host.GetEquipmentBySlot(slot);
//            if (otherItem != null)
//            {
//                Host.UnequipItem(otherItem);
//            }

//            Host.EquipItem(slot, item);

//            if (item.IsWepRanged)
//            {
//                UpdateAmmo(item);
//            }
//        }

//        private void UpdateAmmo(IItemInst rangedWeapon = null)
//        {
//            IItemInst item = rangedWeapon ?? Host.GetEquipmentBySlot(NPCSlots.Ranged);
//            if (item == null)
//            {
//                return;
//            }

//            IItemInst currentAmmo = Host.GetAmmo();
//            ItemTypes ammoType = item.ItemType == ItemTypes.WepXBow ? ItemTypes.AmmoXBow : ItemTypes.AmmoBow;
//            if (currentAmmo == null || currentAmmo.ItemType != ammoType) // no ammo equipped or wrong type
//            {
//                Host.Inventory.ForEachItemPredicate(i =>
//                {
//                    if (i.ItemType == ammoType)
//                    {
//                        if (currentAmmo != null)
//                        {
//                            Host.UnequipItem(currentAmmo);
//                        }

//                        Host.EquipItem(NPCSlots.Ammo, i);
//                        return false;
//                    }

//                    return true;
//                });
//            }
//        }


//        public void TryUnequipItem(IItemInst item)
//        {
//            if (item == null || Host.IsObstructed())
//            {
//                return;
//            }

//            Host.UnequipItem(item);
//            switch (item.ItemType)
//            {
//                case ItemTypes.Wep1H:
//                    Host.UnequipSlot(NPCSlots.OneHanded2);
//                    break;
//                case ItemTypes.Torch:
//                    if (Host.ModelDef.TryGetOverlay("humans_torch", out ScriptOverlay ov))
//                    {
//                        Host.ModelInst.RemoveOverlay(ov);
//                    }

//                    break;
//            }
//        }

//        public void TryAim()
//        {
//            if (Host.IsDead || Host.IsAiming() || Host.ModelInst.IsInAnimation()
//                || Host.Environment.InAir || !Host.IsInFightMode)
//            {
//                return;
//            }

//            var drawnWep = Host.GetDrawnWeapon();
//            if (drawnWep == null || !drawnWep.IsWepRanged)
//            {
//                return;
//            }

//            Host.DoAim();
//        }

//        public void TryUnaim()
//        {
//            if (Host.IsDead || !Host.ModelInst.IsInAnimation())
//            {
//                return;
//            }

//            Host.DoUnaim();
//        }

//        public void TryShoot(Vec3f start, Vec3f end)
//        {
//            if (Host.IsDead || !Host.IsAiming())
//            {
//                return;
//            }

//            if (start.GetDistance(end) > 500000) // just in case
//            {
//                end = start + 500000 * (end - start).Normalise();
//            }

//            var drawnWeapon = Host.GetDrawnWeapon();
//            if (drawnWeapon == null || !drawnWeapon.IsWepRanged)
//            {
//                return;
//            }

//            IItemInst ammo = drawnWeapon.ItemType == ItemTypes.WepBow ? Host.GetRightHand() : Host.GetLeftHand();
//            if (ammo == null || !ammo.IsAmmo)
//            {
//                return;
//            }

//            ProjInst inst = new ProjInst(BaseVobDef.Get<ProjDef>("arrow"))
//            {
//                Item = new ItemInst(ammo.Definition),
//                Damage = drawnWeapon.Damage,
//                Velocity = 0.0004f,
//                Model = ammo.ModelDef
//            };

//            end = end - (end - start).Normalise() *
//                  (ammo.ItemType == ItemTypes.AmmoBow ? 40 : 10); // so arrows' bodies aren't 90% inside walls
//            Host.DoShoot(start, end, inst);

//            int ammoCount = ammo.Amount - 1;
//            if (ammoCount <= 0)
//            {
//                Host.UnequipItem(ammo);
//            }

//            ammo.SetAmount(ammo.Amount - 1);
//        }


//        public void TryUse(IItemInst item)
//        {
//            if (item == null || Host.IsObstructed())
//            {
//                return;
//            }

//            Host.UseItem(item);
//        }

//        public void TryDropItem(IItemInst item, int amount)
//        {
//            if (item == null || Host.IsObstructed())
//            {
//                return;
//            }

//            var pos = Host.GetPosition();
//            var ang = Host.GetAngles();

//            pos += ang.ToAtVector() * 100f;

//            Host.DoDropItem(item, amount, pos, ang);
//        }

//        public void TryTakeItem(IItemInst item)
//        {
//            if (item == null || !item.IsSpawned || Host.IsObstructed())
//            {
//                return;
//            }

//            var potion = ItemDef.Get("hptrank");
//            if (item.Definition == potion && Host.Inventory.Contains(potion))
//            {
//                Host.ModelInst.StartAniJob(Host.AniCatalog.Gestures.DontKnow);
//                return;
//            }

//            Host.DoTakeItem(item);
//        }

//        public void TryVoice(VoiceCmd cmd)
//        {
//            if (Host.IsDead || Host.CustomVoice == 0)
//            {
//                return;
//            }

//            bool shout;
//            switch (cmd)
//            {
//                case VoiceCmd.HELP:
//                    shout = true;
//                    break;
//                default:
//                    shout = false;
//                    break;
//            }

//            Host.DoVoice(cmd, shout);
//        }

//        public static event Action<NpcInst> OnHelpUp;

//        public void TryHelpUp(NpcInst target)
//        {
//            if (Host.IsDead || Host.Movement != NPCMovement.Stand || Host.ModelInst.IsInAnimation() ||
//                Host.Environment.InAir || Host.IsUnconscious
//                || !target.IsUnconscious || target.GetPosition().GetDistance(Host.GetPosition()) > 300)
//            {
//                return;
//            }

//            float speed = 1.0f;
//            Host.ModelInst.StartAniJob(Host.AniCatalog.Gestures.Plunder, speed);
//            Host.DoVoice((VoiceCmd) (1 + Host.Guild));
//            target.LiftUnconsciousness();
//            OnHelpUp?.Invoke(Host);
//        }
//    }
//}