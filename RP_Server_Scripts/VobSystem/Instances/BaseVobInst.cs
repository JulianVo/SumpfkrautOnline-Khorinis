using System;
using GUC.Network;
using GUC.Types;
using GUC.Utilities;
using GUC.WorldObjects;
using RP_Server_Scripts.VobSystem.Definitions;
using RP_Server_Scripts.WorldSystem;

namespace RP_Server_Scripts.VobSystem.Instances
{
    public abstract class BaseVobInst : ExtendedObject, BaseVob.IScriptBaseVob
    {
        protected BaseVobInst(BaseVobDef def)
        {
            BaseInst = CreateVob();
            if (BaseInst == null)
            {
                throw new NullReferenceException("BaseInst is null!");
            }
            Definition = def;
        }

        public byte GetVobType()
        {
            return (byte) VobType;
        } // for base vob interface

        public abstract VobType VobType { get; }

        // GUC - Base - Object
        public BaseVob BaseInst { get; }

        protected abstract BaseVob CreateVob();

        // Definition 
        public BaseVobDef Definition
        {
            get => (BaseVobDef) BaseInst.Instance?.ScriptObject;
            set => BaseInst.Instance = value?.BaseDef;
        }

        public int ID => BaseInst.ID;
        public bool IsStatic => BaseInst.IsStatic;
        public bool IsSpawned => BaseInst.IsSpawned;

        public WorldInst World => (WorldInst) BaseInst.World.ScriptObject;

        public Vec3f GetPosition()
        {
            return BaseInst.Position;
        }

        public Angles GetAngles()
        {
            return BaseInst.Angles;
        }

        public void SetPosition(Vec3f position)
        {
            BaseInst.SetPosition(position);
        }

        public void SetAngles(Angles angles)
        {
            BaseInst.SetAngles(angles);
        }

        public void Spawn(World world)
        {
            Spawn((WorldInst) world.ScriptObject);
        }

        public void Spawn(WorldInst world)
        {
            Spawn(world, GetPosition(), GetAngles());
        }

        public virtual void Spawn(WorldInst world, Vec3f pos, Angles ang)
        {
            BaseInst.Spawn(world.BaseWorld, pos, ang);
        }

        public delegate void DespawnHandler(BaseVobInst vob, WorldInst oldWorld);

        public event DespawnHandler OnDespawn;

        public virtual void Despawn()
        {
            if (!IsSpawned)
            {
                return;
            }

            WorldInst oldWorld = World;
            BaseInst.Despawn();
            OnDespawn?.Invoke(this, oldWorld);
        }


        public virtual void OnReadProperties(PacketReader stream)
        {
        }

        public virtual void OnWriteProperties(PacketWriter stream)
        {
        }

        public virtual void OnReadScriptVobMsg(PacketReader stream)
        {
        }

        public float GetDistance(BaseVobInst other)
        {
            return GetPosition().GetDistance(other.GetPosition());
        }
    }
}