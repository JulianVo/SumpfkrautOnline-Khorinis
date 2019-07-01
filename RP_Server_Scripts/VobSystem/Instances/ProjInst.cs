using System;
using GUC.Types;
using GUC.WorldObjects;
using RP_Server_Scripts.Visuals;
using RP_Server_Scripts.VobSystem.Definitions;

namespace RP_Server_Scripts.VobSystem.Instances
{
    public class ProjInst : BaseVobInst, Projectile.IScriptProjectile
    {
        public ItemInst Item
        {
            get => (ItemInst)this.BaseInst.Item?.ScriptObject;
            set => this.BaseInst.Item = value?.BaseInst;
        }

        public NpcInst Shooter;
        public int Damage;

        public ProjInst(ProjDef def) : base(def)
        {
            this.Definition = def;
        }


        bool DoHitDetection(Vec3f from, Vec3f to)
        {
            // fixme: expensive and also shitty check   

            if (DetectHit(from)) // check last position
                return true;

            float flownDist = from.GetDistance(to);
            if (flownDist < 20)
                return false;  // flew a really short distance since last check

            int interChecks = (int)(flownDist / GUCScripts.SmallestNPCRadius);
            if (interChecks > 1)
            {
                Vec3f dir = (to - from).Normalise();
                float inc = flownDist / interChecks;

                for (int i = 1; i < interChecks; i++)
                {
                    if (DetectHit(from + inc * dir))
                        return true;
                }
            }

            return DetectHit(to);
        }

        bool DetectHit(Vec3f position)
        {
            NpcInst target = null;
            this.World.BaseWorld.ForEachNPCRoughPredicate(position, GUCScripts.BiggestNPCRadius, baseNPC =>
            {
                NpcInst npc = (NpcInst)baseNPC.ScriptObject;
                if (!npc.IsDead && npc != Shooter
                && NpcInst.AllowHitEvent.TrueForAll(Shooter, npc)
                && npc.AllowHitTarget.TrueForAll(Shooter, npc) && Shooter.AllowHitAttacker.TrueForAll(Shooter, npc))
                {
                    var modelDef = npc.ModelDef;

                    var npcPos = npc.GetPosition() + npc.BaseInst.GetAtVector() * npc.ModelDef.CenterOffset;

                    if (npcPos.GetDistancePlanar(position) <= modelDef.Radius
                        && Math.Abs(npcPos.Y - position.Y) <= modelDef.HalfHeight)
                    {
                        target = npc;
                        return false;
                    }
                }
                return true;
            });

            if (target != null)
            {
                target.Hit(Shooter, Damage);
                return true;
            }

            return false;
        }



        public override VobType VobType => VobType.Projectile;

        public new Projectile BaseInst => (Projectile)base.BaseInst;
        public new ProjDef Definition
        {
            get => (ProjDef)base.Definition;
            set => base.Definition = value;
        }

        public ModelDef Model
        {
            get => (ModelDef)BaseInst.Model?.ScriptObject;
            set => BaseInst.Model = value?.BaseDef;
        }
        public float Velocity
        {
            get => BaseInst.Velocity;
            set => BaseInst.Velocity = value;
        }
        public Vec3f Destination
        {
            get => this.BaseInst.Destination;
            set => this.BaseInst.Destination = value;
        }

    


        protected override BaseVob CreateVob()
        {
            return new Projectile(this);
        }



        public void UpdatePos()
        {
            Vec3f curPos = GetPosition();

            if (curPos.GetDistance(BaseInst.StartPosition) > 100000)
            {
                this.Despawn();
                return;
            }

            if (DoHitDetection(BaseInst.LastPosition, curPos))
                this.Despawn();
        }


        public void OnEndPos()
        {
            if (DoHitDetection(BaseInst.LastPosition, Destination))
                return;

            ItemInst item = this.Item;
            if (item != null)
            {
                item.Spawn(this.World, this.Destination, this.GetAngles());
                this.World.DespawnList_PItems.AddVob(item);
            }
        }
    }
}
