﻿using GUC.Scripts.Sumpfkraut.WorldSystem;
using GUC.Types;
using Gothic.Types;
using Gothic.Objects;

namespace GUC.Scripts.Sumpfkraut.VobSystem.Instances
{
    public partial class ProjInst
    {
        static readonly SoundDefinition sfx_shoot = new SoundDefinition("BOWSHOOT");
        static readonly SoundDefinition sfx_wo_wo = new SoundDefinition("CS_IHL_WO_WO");
        static readonly SoundDefinition sfx_wo_me = new SoundDefinition("CS_IHL_WO_ME");
        static readonly SoundDefinition sfx_wo_st = new SoundDefinition("CS_IHL_WO_ST");
        static readonly SoundDefinition sfx_wo_wa = new SoundDefinition("CS_IHL_WO_WA");
        static readonly SoundDefinition sfx_wo_ea = new SoundDefinition("CS_IHL_WO_EA");
        static readonly SoundDefinition sfx_wo_sa = new SoundDefinition("CS_IHL_WO_SA");

        partial void pSpawn(WorldInst world, Vec3f pos, Angles ang)
        {
            // create arrow trail
            var ai = oCAIArrow.Create();
            var gVob = this.BaseInst.gVob;
            gVob.SetSleeping(true);
            gVob.SetAI(ai);
            ai.CreateTrail(gVob);
            gVob.SetSleeping(false);

            // play shooting sound
            SoundHandler.PlaySound3D(sfx_shoot, this.BaseInst);
        }

        partial void pDespawn()
        {
            // check the level for hits
            //Vec3f currentPos = this.GetPosition();
            //Vec3f direction = (this.Destination - currentPos).Normalise();

            Vec3f startPos = this.BaseInst.StartPosition;// currentPos + direction * -100;
            Vec3f ray = (Destination - startPos).Normalise() * (Destination.GetDistance(startPos) + 100f);//direction * 200;

            using (zVec3 zStart = startPos.CreateGVec())
            using (zVec3 zRay = ray.CreateGVec())
            {
                var gWorld = GothicGlobals.Game.GetWorld();

                const zCWorld.zTraceRay parm = zCWorld.zTraceRay.Ignore_Alpha | zCWorld.zTraceRay.Test_Water | zCWorld.zTraceRay.Return_POLY | zCWorld.zTraceRay.Ignore_NPC | zCWorld.zTraceRay.Ignore_Projectiles | zCWorld.zTraceRay.Ignore_Vob_No_Collision;
                if (gWorld.TraceRayNearestHit(zStart, zRay, parm))
                {
                    var poly = gWorld.Raytrace_FoundPoly;

                    SoundDefinition sfx;
                    if (poly.Address == 0)
                    {
                        sfx = sfx_wo_wo; // wood
                    }
                    else
                    {
                        switch (poly.Material.MatGroup)
                        {
                            default:
                            case 0: // undef
                            case 3: // wood
                                sfx = sfx_wo_wo;
                                break;
                            case 1: // metal
                                sfx = sfx_wo_me;
                                break;
                            case 2: // stone
                                sfx = sfx_wo_st;
                                break;
                            case 4: // earth
                                sfx = sfx_wo_ea;
                                break;
                            case 5: // water
                                sfx = sfx_wo_wa;
                                break;
                            case 6: // snow
                                sfx = sfx_wo_sa;
                                break;
                        }
                    }
                    SoundHandler.PlaySound3D(sfx, Destination);
                }
            }
        }
    }
}
