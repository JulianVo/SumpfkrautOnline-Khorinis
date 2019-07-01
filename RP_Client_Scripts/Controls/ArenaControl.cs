using System;
using GUC.Scripts.Sumpfkraut.Controls;
using WinApi.User.Enumeration;
using GUC.Scripts.Sumpfkraut.Networking;
using Gothic.Objects;
using GUC.Scripts.Arena.Menus;
using GUC.Scripts.Menus;
using GUC.Scripts.Menus.IngameMenu;
using GUC.Scripts.Sumpfkraut.VobSystem.Definitions;
using GUC.Scripts.Sumpfkraut.VobSystem.Instances;
using GUC.Types;
using GUC.Utilities;

namespace GUC.Scripts.Arena.Controls
{
    internal sealed class ArenaControl : InputControl
    {
        private readonly ScriptClient _Client;
        private readonly ChatMenu _ChatMenu;
        private readonly InGameMenu _InGameMenu;
        const float defaultSpeed = 1.0f;
        const float fastSpeed = 3.0f;
        static float speedMultiplier = defaultSpeed;

        readonly KeyDictionary _SpectatorControls;

        public ArenaControl(ScriptClient client,ChatMenu chatMenu, InGameMenu inGameMenu )
        {
            _Client = client ?? throw new ArgumentNullException(nameof(client));
            _ChatMenu = chatMenu ?? throw new ArgumentNullException(nameof(chatMenu));
            _InGameMenu = inGameMenu ?? throw new ArgumentNullException(nameof(inGameMenu));

            _PlayerControls = new KeyDictionary()
            {
                {KeyBind.Jump, Jump},
                {
                    KeyBind.DrawFists, d =>
                    {
                        if (d) NPCInst.Requests.DrawFists(ScriptClient.Client.Character);
                    }
                },
                {KeyBind.MoveForward, d => CheckFightMove(d, FightMoves.Fwd)},
                {KeyBind.TurnLeft, d => CheckFightMove(d, FightMoves.Left)},
                {KeyBind.TurnRight, d => CheckFightMove(d, FightMoves.Right)},
                {KeyBind.MoveBack, d => CheckFightMove(d, FightMoves.Parry)},
                {KeyBind.MoveLeft, d => CheckFightMove(d, FightMoves.Left)},
                {KeyBind.MoveRight, d => CheckFightMove(d, FightMoves.Right)},
                {KeyBind.Action, PlayerActionButton},
                {KeyBind.DrawWeapon, DrawWeapon},
                {KeyBind.ScoreBoard, ToggleScoreBoard},
                {
                    KeyBind.ChatAll, d =>
                    {
                        if (d) _ChatMenu.OpenAllChat();
                    }
                },
                {
                    KeyBind.ChatTeam, d =>
                    {
                        if (d) _ChatMenu.OpenTeamChat();
                    }
                },
                {
                    KeyBind.Inventory, d =>
                    {
                        if (d) Sumpfkraut.Menus.PlayerInventory.Menu.Open();
                    }
                },
                {VirtualKeys.P, PrintPosition},
                {VirtualKeys.F2, d => Menus.PlayerList.TogglePlayerList()},
                {VirtualKeys.F3, ToggleG1Camera},
                {VirtualKeys.F5, ToggleScreenInfo},
                {VirtualKeys.N1, DrawMeleeWeapon},
                {VirtualKeys.N2, DrawRangedWeapon},
                {VirtualKeys.RightButton, FreeAim},
                {KeyBind.StatusMenu, OpenStatusMenu}
            };

            _SpectatorControls = new KeyDictionary()
            {
                { VirtualKeys.Shift, down => speedMultiplier = !down ? defaultSpeed : fastSpeed },
                { KeyBind.ChatAll, d => { if (d) _ChatMenu.OpenAllChat(); } },
                { KeyBind.ChatTeam, d => { if (d) _ChatMenu.OpenTeamChat(); } },
                { KeyBind.ScoreBoard, ToggleScoreBoard },
                { VirtualKeys.F2, d => Menus.PlayerList.TogglePlayerList() },
                { VirtualKeys.F3, ToggleG1Camera },
                { VirtualKeys.F5, ToggleScreenInfo },
            };
        }

        long lastUpdate = 0;
        void SpectatorUpdate()
        {
            if (InputHandler.MouseDistY != 0)
            {
                float angle = InputHandler.MouseDistY * 0.2f;
                GothicGlobals.Game.GetCameraVob().RotateLocalX(angle > 20 ? 20 : angle);
            }
            if (InputHandler.MouseDistX != 0)
            {
                float angle = InputHandler.MouseDistX * 0.2f;
                GothicGlobals.Game.GetCameraVob().RotateWorldY(angle > 20 ? 20 : angle);
            }

            long diff = GameTime.Ticks - lastUpdate;
            lastUpdate = GameTime.Ticks;
            if (diff > TimeSpan.TicksPerMillisecond * 50)
                diff = TimeSpan.TicksPerMillisecond * 50;

            float speed = (float)diff / 18000.0f * speedMultiplier;
            if (InputHandler.IsPressed(VirtualKeys.Up) || InputHandler.IsPressed(VirtualKeys.W))
            {
                var cam = GothicGlobals.Game.GetCameraVob();
                var dir = new Vec3f(cam.Direction) * speed;
                cam.MoveWorld(dir.X, dir.Y, dir.Z);
            }
            else if (InputHandler.IsPressed(VirtualKeys.Down) || InputHandler.IsPressed(VirtualKeys.S))
            {
                var cam = GothicGlobals.Game.GetCameraVob();
                var dir = new Vec3f(cam.Direction) * -speed;
                cam.MoveWorld(dir.X, dir.Y, dir.Z);
            }

            if (InputHandler.IsPressed(VirtualKeys.A))
            {
                var cam = GothicGlobals.Game.GetCameraVob();
                var dir = new Vec3f(cam.Direction).Cross(new Vec3f(0, 1, 0));
                dir *= speed;
                cam.MoveWorld(dir.X, dir.Y, dir.Z);
            }
            else if (InputHandler.IsPressed(VirtualKeys.D))
            {
                var cam = GothicGlobals.Game.GetCameraVob();
                var dir = new Vec3f(cam.Direction).Cross(new Vec3f(0, 1, 0));
                dir *= -speed;
                cam.MoveWorld(dir.X, dir.Y, dir.Z);
            }

            if (InputHandler.IsPressed(VirtualKeys.Space))
            {
                var cam = GothicGlobals.Game.GetCameraVob();
                var dir = new Vec3f(0, 1, 0);
                dir *= speed;
                cam.MoveWorld(dir.X, dir.Y, dir.Z);
            }
            else if (InputHandler.IsPressed(VirtualKeys.Control))
            {
                var cam = GothicGlobals.Game.GetCameraVob();
                var dir = new Vec3f(0, 1, 0);
                dir *= -speed;
                cam.MoveWorld(dir.X, dir.Y, dir.Z);
            }
        }

        private readonly KeyDictionary _PlayerControls;

        void OpenStatusMenu(bool down)
        {
            if (!down) return;

            if (_Client.Character != null)
            {
                Arena.Menus.StatusMenu.Instance.Open();
            }
        }

        static void DrawMeleeWeapon(bool down)
        {
            if (!down) return;

            var hero = ScriptClient.Client.Character;
            if (hero.ModelInst.IsInAnimation() || hero.IsDead || hero.IsUnconscious)
                return;

            ItemInst weapon;
            if (((weapon = hero.GetDrawnWeapon()) != null && weapon.IsWepMelee) || // undraw
                 (weapon = hero.GetEquipmentBySlot(NPCSlots.OneHanded1)) != null || // draw
                 (weapon = hero.GetEquipmentBySlot(NPCSlots.TwoHanded)) != null) // draw
                NPCInst.Requests.DrawWeapon(hero, weapon);
        }

        static void DrawRangedWeapon(bool down)
        {
            if (!down) return;

            var hero = ScriptClient.Client.Character;
            if (hero.ModelInst.IsInAnimation() || hero.IsDead || hero.IsUnconscious)
                return;

            ItemInst weapon;
            if (((weapon = hero.GetDrawnWeapon()) != null && weapon.IsWepRanged) ||  // undraw
                 (weapon = hero.GetEquipmentBySlot(NPCSlots.Ranged)) != null) // draw
            {
                NPCInst.Requests.DrawWeapon(hero, weapon);
            }
        }

        static void DrawWeapon(bool down)
        {
            if (!down) return;

            var hero = ScriptClient.Client.Character;
            if (hero.ModelInst.IsInAnimation() || hero.IsDead || hero.IsUnconscious)
                return;

            ItemInst weapon;
            if ((weapon = hero.GetDrawnWeapon()) != null ||
               ((weapon = hero.LastUsedWeapon) != null && weapon.Container == hero && weapon.IsEquipped) ||
               (weapon = hero.GetEquipmentBySlot(NPCSlots.OneHanded1)) != null ||
               (weapon = hero.GetEquipmentBySlot(NPCSlots.TwoHanded)) != null ||
               (weapon = hero.GetEquipmentBySlot(NPCSlots.Ranged)) != null)
            {
                NPCInst.Requests.DrawWeapon(hero, weapon);
            }
            else
            {
                NPCInst.Requests.DrawFists(hero);
            }
        }

        static void Jump(bool d)
        {
            var hero = NPCInst.Hero;
            if (!d || hero == null || hero.IsDead || hero.IsUnconscious) return;

            if (hero.ModelInst.GetActiveAniFromLayer(1) != null || hero.BaseInst.gAI.GetFoundLedge() || IsWarmup())
                return;

            var ledge = hero.BaseInst.DetectClimbingLedge();
            if (ledge == null)
            {
                if (hero.BaseInst.gAI.CheckEnoughSpaceMoveForward(true))
                    NPCInst.Requests.Jump(hero);
            }
            else
            {
                NPCInst.Requests.Climb(hero, ledge);
            }
        }

        static Vec3f lastPos;
        static void PrintPosition(bool down)
        {
            var hero = NPCInst.Hero;
            if (!down || hero == null)
                return;

            var pos = hero.GetPosition();
            var ang = hero.GetAngles();

            Log.Logger.Log(pos + " " + ang + " Distance to last: " + lastPos.GetDistance(pos));
            System.IO.File.AppendAllText("positions.txt", string.Format(System.Globalization.CultureInfo.InvariantCulture, "{{ new Vec3f({0}f, {1}f, {2}f), new Angles({3}f, {4}f, {5}f) }},\n", pos.X, pos.Y, pos.Z, ang.Pitch, ang.Yaw, ang.Roll));
            lastPos = pos;
        }

        static void ToggleScoreBoard(bool down)
        {
        }


        static void CheckFightMove(bool down, FightMoves move)
        {
            if (!down)
                return;

            var hero = ScriptClient.Client.Character;
            if (hero.IsDead || hero.IsUnconscious || hero.Movement != NPCMovement.Stand || !hero.IsInFightMode || hero.Environment.InAir
                || IsWarmup())
                return;

            var drawnWeapon = hero.GetDrawnWeapon();
            if (drawnWeapon != null && drawnWeapon.IsWepRanged)
            {
                if (freeAim)
                    RequestShootFree(hero);
                else if (KeyBind.Action.IsPressed())
                    RequestShootAuto(hero);
            }
            else if (KeyBind.Action.IsPressed())
            {
                NPCInst.Requests.Attack(hero, move);
            }
        }

        static LockTimer screamLock = new LockTimer(3000);
        static void PlayerActionButton(bool down)
        {
            var hero = ScriptClient.Client.Character;
            if (hero.IsDead)
                return;

            if (hero.IsUnconscious)
            {
                return;
            }


            if (freeAim)
            {
                if (!IsWarmup())
                    RequestShootFree(hero);
                return;
            }

            if (hero.IsInFightMode)
            {
                var drawnWeapon = hero.GetDrawnWeapon();
                if (drawnWeapon != null && drawnWeapon.IsWepRanged)
                {
                    NPCInst.Requests.Aim(hero, down);
                }
                else if (down && KeyBind.MoveForward.IsPressed() && !IsWarmup())
                {
                    NPCInst.Requests.Attack(hero, FightMoves.Run);
                }
            }
            else if (down)
            {
                if (PlayerFocus.FocusVob is ItemInst item)
                {
                    NPCInst.Requests.TakeItem(hero, item);
                }
            }
        }

        static bool IsWarmup()
        {
            return false;
        }

        void LookAround(NPCInst hero)
        {
            const float maxLookSpeed = 2f;

            float rotSpeed = 0;
            if (InputHandler.MouseDistY != 0)
            {
                rotSpeed = Alg.Clamp(-maxLookSpeed, InputHandler.MouseDistY * 0.35f * MouseSpeed, maxLookSpeed);

                var cam = zCAICamera.CurrentCam;
                cam.BestElevation = Alg.Clamp(-50, cam.BestElevation + rotSpeed, 85);
            }

            if (InputHandler.MouseDistX != 0)
            {
                rotSpeed = Alg.Clamp(-maxLookSpeed, InputHandler.MouseDistX * 0.35f * MouseSpeed, maxLookSpeed);

                var cam = zCAICamera.CurrentCam;
                cam.BestAzimuth -= rotSpeed;
            }
        }

        LockTimer dodgeLock = new LockTimer(200);
        const long StrafeInterval = 200 * TimeSpan.TicksPerMillisecond;
        long nextStrafeChange = 0;
        void PlayerUpdate()
        {

            NPCInst hero = ScriptClient.Client.Character;
            var gAI = hero.BaseInst.gAI;
            if (hero.IsDead || hero.IsUnconscious)
            {
                LookAround(hero);
                return;
            }

            zCAICamera.CurrentCam.BestAzimuth = 0;

            if (freeAim)
            {
                FreeAiming(hero);
                return;
            }

            DoTurning(hero);

            if (KeyBind.Action.IsPressed() && hero.IsInFightMode)
            {
                var enemy = PlayerFocus.GetFocusNPC();
                if (enemy == null || !enemy.IsSpawned || enemy.IsDead)
                {
                    PlayerFocus.SetLockedTarget(null); // updates for new target
                }
                PlayerFocus.SetLockedTarget(PlayerFocus.GetFocusNPC());
            }
            else
            {
                PlayerFocus.SetLockedTarget(null);
            }

            NPCMovement state = NPCMovement.Stand;
            if (!KeyBind.Action.IsPressed() || hero.Movement != NPCMovement.Stand)
            {
                if (KeyBind.MoveForward.IsPressed()) // move forward
                {
                    state = NPCMovement.Forward;
                }
                else if (KeyBind.MoveBack.IsPressed()) // move backward
                {
                    var drawnWeapon = hero.GetDrawnWeapon();
                    if (hero.IsInFightMode && (drawnWeapon == null || drawnWeapon.IsWepMelee))
                    {
                        if (dodgeLock.IsReady) // don't spam
                        {
                            if (IsWarmup())
                                return;

                            NPCInst.Requests.Attack(hero, FightMoves.Dodge);
                        }
                        return;
                    }
                    else
                    {
                        state = NPCMovement.Backward;
                    }
                }
                else if (KeyBind.MoveLeft.IsPressed()) // strafe left
                {
                    state = NPCMovement.Left;
                }
                else if (KeyBind.MoveRight.IsPressed()) // strafe right
                {
                    state = NPCMovement.Right;
                }
                else
                {
                    state = NPCMovement.Stand;
                }
            }

            if (nextStrafeChange > GameTime.Ticks)
            {
                state = hero.Movement;
            }

            if (state == NPCMovement.Forward)
            {   // FIXME: use only a better CheckEnoughSpaceMoveForward
                if (hero.Movement == NPCMovement.Stand && !gAI.CheckEnoughSpaceMoveForward(true))
                {
                    state = NPCMovement.Stand;
                }
                else
                {
                    gAI.CalcForceModelHalt();
                    if ((gAI.Bitfield0 & zCAIPlayer.Flags.ForceModelHalt) != 0)
                    {
                        gAI.Bitfield0 &= ~zCAIPlayer.Flags.ForceModelHalt;
                        state = NPCMovement.Stand;
                    }
                }
            }
            else if (state == NPCMovement.Backward)
            {
                if (!gAI.CheckEnoughSpaceMoveBackward(true))
                    state = NPCMovement.Stand;
            }
            else if (state == NPCMovement.Left)
            {
                if (!gAI.CheckEnoughSpaceMoveLeft(true))
                    state = NPCMovement.Stand;
            }
            else if (state == NPCMovement.Right)
            {
                if (!gAI.CheckEnoughSpaceMoveRight(true))
                    state = NPCMovement.Stand;
            }

            if (state != NPCMovement.Stand && IsWarmup())
                state = NPCMovement.Stand;

            if (state == NPCMovement.Left || state == NPCMovement.Right || (state == NPCMovement.Forward && hero.IsInFightMode))
            {
                if (hero.Movement != state)
                    nextStrafeChange = GameTime.Ticks + StrafeInterval;
            }
            else
                nextStrafeChange = 0;

            hero.SetMovement(state);
        }

        static void DoTurning(NPCInst hero)
        {
            if (hero?.BaseInst?.gAI == null)
            {
                return;
            }

            if (hero.BaseInst.gAI.GetFoundLedge())
                return;

            const float maxTurnFightSpeed = 0.07f;

            NPCInst enemy = PlayerFocus.LockedTarget;
            if (enemy != null)
            {
                Vec3f heroPos = hero.GetPosition();
                Vec3f enemyPos = enemy.GetPosition();

                float bestYaw = Angles.GetYawFromAtVector(enemyPos - heroPos);
                Angles curAngles = hero.GetAngles();

                float yawDiff = Angles.Difference(bestYaw, curAngles.Yaw);
                curAngles.Yaw += Alg.Clamp(-maxTurnFightSpeed, yawDiff, maxTurnFightSpeed);

                hero.SetAngles(curAngles);
                CheckCombineAim(hero, enemyPos - heroPos);
                return;
            }

            const float maxLookupSpeed = 2f;
            float rotSpeed = 0;
            if (InputHandler.MouseDistY != 0)
            {
                rotSpeed = Alg.Clamp(-maxLookupSpeed, InputHandler.MouseDistY * 0.35f * MouseSpeed, maxLookupSpeed);

                if (hero.Environment.WaterLevel >= 1)
                {
                    hero.BaseInst.gAI.DiveRotateX(rotSpeed);
                }
                else
                {
                    var cam = zCAICamera.CurrentCam;
                    cam.BestElevation = Alg.Clamp(-50, cam.BestElevation + rotSpeed, 85);
                }
            }

            // Fixme: do own turning
            const float maxTurnSpeed = 2f;
            if (!KeyBind.Action.IsPressed())
            {
                float turn = 0;
                if (KeyBind.TurnLeft.IsPressed())
                    turn = -maxTurnSpeed;
                else if (KeyBind.TurnRight.IsPressed())
                    turn = maxTurnSpeed;
                else if (Math.Abs(InputHandler.MouseDistX) > ((rotSpeed > 0.5f && hero.Movement == NPCMovement.Stand) ? 18 : 2))
                {
                    turn = Alg.Clamp(-maxTurnSpeed, InputHandler.MouseDistX * 0.45f * MouseSpeed, maxTurnSpeed);
                }

                if (turn != 0)
                {
                    hero.BaseInst.gAI.Turn(turn, !hero.ModelInst.IsInAnimation());
                    return;
                }
            }
            hero.BaseInst.gVob.AniCtrl.StopTurnAnis();
        }

        static KeyHoldHelper fwdTelHelper = new KeyHoldHelper(500, 100)
        {
            { () =>
                {
                    NPCInst hero = ScriptClient.Client.Character;
                    Vec3f pos = hero.GetPosition();
                    Vec3f dir = (Vec3f)hero.BaseInst.gVob.Direction;
                    pos += dir * 150.0f;
                    hero.SetPosition(pos);
                }, VirtualKeys.K
            }
        };

        static KeyHoldHelper upTelHelper = new KeyHoldHelper(500, 100)
        {
            { () =>
                {
                    NPCInst hero = ScriptClient.Client.Character;
                    Vec3f pos = hero.GetPosition();
                    pos.Y += 200.0f;
                    hero.SetPosition(pos);
                    hero.BaseInst.gVob.SetPhysicsEnabled(false);
                }, VirtualKeys.F8
            }
        };

        static void RequestShootAuto(NPCInst hero)
        {
            Vec3f start;
            using (var matrix = Gothic.Types.zMat4.Create())
            {
                var weapon = hero.GetDrawnWeapon();
                var node = (weapon == null || weapon.ItemType == ItemTypes.WepBow) ? oCNpc.NPCNodes.RightHand : oCNpc.NPCNodes.LeftHand;
                hero.BaseInst.gVob.GetTrafoModelNodeToWorld(node, matrix);
                start = (Vec3f)matrix.Position;
            }

            const zCWorld.zTraceRay traceType = zCWorld.zTraceRay.Ignore_Alpha | zCWorld.zTraceRay.Ignore_Projectiles | zCWorld.zTraceRay.Ignore_Vob_No_Collision | zCWorld.zTraceRay.Ignore_NPC;

            NPCInst enemy = PlayerFocus.GetFocusNPC();
            Vec3f dir = enemy == null ? (Vec3f)hero.BaseInst.gVob.Direction : (enemy.GetPosition() - start).Normalise();
            Vec3f ray = 500000f * dir;

            Vec3f end;
            using (var zStart = start.CreateGVec())
            using (var zRay = ray.CreateGVec())
            {
                var gWorld = GothicGlobals.Game.GetWorld();

                if (gWorld.TraceRayNearestHit(zStart, zRay, traceType))
                {
                    end = (Vec3f)gWorld.Raytrace_FoundIntersection;
                }
                else
                {
                    end = start + ray;
                }
            }

            NPCInst.Requests.Shoot(hero, start, end);
        }

        static void RequestShootFree(NPCInst hero)
        {
            CalcRangedTrace(hero, out Vec3f start, out Vec3f end);
            NPCInst.Requests.Shoot(hero, start, end);
        }

        static void CalcRangedTrace(NPCInst npc, out Vec3f start, out Vec3f end)
        {
            Vec3f projStartPos;
            using (var matrix = Gothic.Types.zMat4.Create())
            {
                var weapon = npc.GetDrawnWeapon();
                var node = (weapon == null || weapon.ItemType == ItemTypes.WepBow) ? oCNpc.NPCNodes.RightHand : oCNpc.NPCNodes.LeftHand;
                npc.BaseInst.gVob.GetTrafoModelNodeToWorld(node, matrix);
                projStartPos = (Vec3f)matrix.Position;
            }

            const zCWorld.zTraceRay traceType = zCWorld.zTraceRay.Ignore_Alpha | zCWorld.zTraceRay.Ignore_Projectiles | zCWorld.zTraceRay.Ignore_Vob_No_Collision | zCWorld.zTraceRay.Ignore_NPC;

            var camVob = GothicGlobals.Game.GetCameraVob();
            start = (Vec3f)camVob.Position;
            Vec3f ray = 500000f * (Vec3f)camVob.Direction;
            end = start + ray;

            using (var zStart = start.CreateGVec())
            using (var zRay = ray.CreateGVec())
            {
                var gWorld = GothicGlobals.Game.GetWorld();

                if (gWorld.TraceRayNearestHit(zStart, zRay, traceType))
                {
                    end = (Vec3f)gWorld.Raytrace_FoundIntersection;
                }

                start = projStartPos;
                ray = end - start;

                start.SetGVec(zStart);
                ray.SetGVec(zRay);

                if (gWorld.TraceRayNearestHit(zStart, zRay, traceType))
                {
                    end = (Vec3f)gWorld.Raytrace_FoundIntersection;
                }
            }
        }

        static Sumpfkraut.GUI.GUCWorldSprite crosshair;
        static bool freeAim = false;
        static void FreeAim(bool down)
        {

            if (crosshair == null)
            {
                crosshair = new Sumpfkraut.GUI.GUCWorldSprite(10, 10);
                crosshair.SetBackTexture("crosshair.tga");
                crosshair.ShowOutOfScreen = false;
            }

            var hero = NPCInst.Hero;

            string CamModFreeAim = "CAMMODRANGED_FREEAIM";
            if (hero.ModelDef.Visual == "ORC.MDS" || hero.ModelDef.Visual == "DRACONIAN.MDS")
                CamModFreeAim += "_ORC";

            if (down && hero != null && !hero.IsDead && hero.IsInFightMode
                && !hero.Environment.InAir && !hero.ModelInst.IsInAnimation())
            {
                var drawnWeapon = hero.GetDrawnWeapon();
                if (drawnWeapon != null && drawnWeapon.IsWepRanged)
                {
                    if (!freeAim)
                    {
                        hero.SetMovement(NPCMovement.Stand);
                        NPCInst.Requests.Aim(hero, true);

                        // no auto-lock
                        PlayerFocus.SetLockedTarget(null);

                        // zoom in
                        FOVTransition(60, TimeSpan.TicksPerSecond / 2);

                        zCAICamera.CamModRanged.Set(CamModFreeAim); // replace so gothic sets it to this while in bow mode
                        zCAICamera.CurrentCam.SetByScript(CamModFreeAim); // change camera
                        freeAim = true;
                    }
                    return;
                }
            }

            if (freeAim)
            {
                NPCInst.Requests.Aim(hero, false);
                crosshair.Hide();
                FOVTransition(90, TimeSpan.TicksPerSecond / 2);

                var cam = zCAICamera.CurrentCam;
                zCAICamera.CamModRanged.Set("CAMMODRANGED"); // reset
                if (cam.CurrentMode.ToString().Equals(CamModFreeAim, StringComparison.OrdinalIgnoreCase))
                    cam.SetByScript("CAMMODRANGED"); // change camera
                freeAim = false;
            }
        }

        static void FreeAiming(NPCInst hero)
        {
            if (InputHandler.MouseDistY != 0)
            {
                const float maxSpeed = 1.0f;
                float rotSpeed = Alg.Clamp(-maxSpeed, InputHandler.MouseDistY * 0.16f * MouseSpeed, maxSpeed);

                var cam = zCAICamera.CurrentCam;
                cam.BestElevation = Alg.Clamp(-65, cam.BestElevation + rotSpeed, 85);
            }

            if (InputHandler.MouseDistX != 0)
            {
                const float maxSpeed = 1.2f;

                float rotSpeed = Alg.Clamp(-maxSpeed, InputHandler.MouseDistX * 0.20f * MouseSpeed, maxSpeed);
                hero.BaseInst.gAI.Turn(rotSpeed, false);
            }

            CalcRangedTrace(hero, out Vec3f start, out Vec3f end);
            crosshair.SetTarget(end);

            CheckCombineAim(hero, (Vec3f)GothicGlobals.Game.GetCameraVob().Direction);
        }

        static void CheckCombineAim(NPCInst hero, Vec3f direction)
        {
            var gModel = hero.BaseInst.gModel;
            int aniID;
            if (gModel.IsAnimationActive("S_BOWAIM"))
            {
                aniID = gModel.GetAniIDFromAniName("S_BOWAIM");
            }
            else if (gModel.IsAnimationActive("S_CBOWAIM"))
            {
                aniID = gModel.GetAniIDFromAniName("S_CBOWAIM");
            }
            else
            {
                if (freeAim)
                    crosshair.Hide();
                return;
            }

            if (freeAim)
                crosshair.Show();

            Angles heroAngles = hero.GetAngles();
            Angles angles = Angles.FromAtVector(direction);
            float pitch = Angles.Difference(angles.Pitch, heroAngles.Pitch);
            float yaw = Angles.Difference(angles.Yaw, heroAngles.Yaw);

            float x = 0.6f;// Alg.Clamp(0, 0.5f - yaw / Angles.PI, 1);
            float y = Alg.Clamp(0, 0.47f - (pitch < 0 ? 1.2f : 1.0f) * pitch / Angles.PI, 1);

            hero.BaseInst.gAI.InterpolateCombineAni(x, y, aniID);
        }

        protected override void KeyDown(VirtualKeys key)
        {
            if (key == VirtualKeys.Escape)
            {
                _InGameMenu.Open();
                return;
            }

            if (ScriptClient.Client.IsCharacter)
                _PlayerControls.TryCall(key, true);
            else if (ScriptClient.Client.IsSpecating)
                _SpectatorControls.TryCall(key, true);
        }

        protected override void KeyUp(VirtualKeys key)
        {
            if (ScriptClient.Client.IsCharacter)
                _PlayerControls.TryCall(key, false);
            else if (ScriptClient.Client.IsSpecating)
                _SpectatorControls.TryCall(key, false);
        }

        protected override void Update(long now)
        {
            if (ScriptClient.Client.IsCharacter)
                PlayerUpdate();
            if (ScriptClient.Client.IsSpecating)
                SpectatorUpdate();

            UpdateFOV(now);
        }

        static bool showScreenInfo = true;
        static void ToggleScreenInfo(bool down)
        {
            if (!down) return;

            showScreenInfo = !showScreenInfo;
            Network.GameClient.ShowInfo = showScreenInfo;
        }


        static bool g1 = false;
        static void ToggleG1Camera(bool down)
        {
            zCAICamera cam;
            if (!down || (cam = zCAICamera.CurrentCam).IsNull)
                return;

            if (Gothic.System.zCParser.GetCameraParser().LoadDat(g1 ? "CAMERA.DAT" : "CAMERA.DAT.G1"))
            {
                cam.CreateInstance(cam.CurrentMode); // update camera
                g1 = !g1;
                SetFOV(currentFOV); // update fov
            }
        }

        static float startFOV = 90.0f;
        static long startFOVTime = 0;
        static float currentFOV = 90.0f;
        static float endFOV = 90.0f;
        static long endFOVTime = 0;

        static void FOVTransition(float fov, long intermission = 0)
        {
            startFOV = currentFOV;
            endFOV = Alg.Clamp(10, fov, 160);

            startFOVTime = GameTime.Ticks;
            endFOVTime = startFOVTime + intermission;
        }

        static void UpdateFOV(long now)
        {
            if (currentFOV == endFOV)
                return;

            long elapsed = now - startFOVTime;
            long duration = endFOVTime - startFOVTime;

            float fov;
            if (elapsed >= duration || duration <= 0)
            {
                fov = endFOV;
            }
            else
            {
                fov = startFOV + (endFOV - startFOV) * elapsed / duration;
            }

            SetFOV(fov);
        }

        static void SetFOV(float fov)
        {
            var cam = zCCamera.ActiveCamera;
            if (cam.IsNull)
                return;

            float ratio = 0.75f;
            if (g1)
            {
                var screen = GUI.GUCView.GetScreenSize();
                if (screen.X != 0 && screen.Y != 0)
                    ratio = (float)screen.Y / screen.X;
            }

            cam.SetFOV(fov, fov * ratio);
            currentFOV = fov;
        }

        private static float _MouseSpeed;
        static float MouseSpeed
        {
            get
            {
                if (Math.Abs(_MouseSpeed) < 0.0001)
                {
                    var section = Gothic.zCOption.GetSectionByName("GAME");
                    if (!section.IsNull)
                    {
                        var entry = section.GetEntryByName("mouseSensitivity");
                        if (!entry.IsNull)
                        {
                            if (float.TryParse(entry.VarValue.ToString(), System.Globalization.NumberStyles.Any,
                                               System.Globalization.CultureInfo.InvariantCulture, out _MouseSpeed))
                            {
                                return _MouseSpeed;
                            }
                        }
                    }
                    _MouseSpeed = 0.5f;
                }
                return _MouseSpeed;
            }
        }
    }
}
