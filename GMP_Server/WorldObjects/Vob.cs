﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GUC.Server.Network;
using GUC.Types;
using RakNet;
using GUC.Network;
using GUC.Enumeration;
using GUC.Server.Network.Messages;
using GUC.Server.WorldObjects.Collections;
using GUC.Server.WorldObjects.Instances;

namespace GUC.Server.WorldObjects
{
    public class Vob : ServerObject, IVobObj<uint>
    {
        public static readonly VobDictionary Vobs = Network.Server.sVobs.GetDict(VobInstance.sVobType);

        static uint idCount = 1; // Start with 1 cause a "null-vob" (id = 0) is needed for networking

        public uint ID { get; protected set; }
        public VobInstance Instance { get; protected set; }

        public VobType VobType { get { return Instance.VobType; } }

        public string Visual { get { return Instance.Visual; } }
        public bool CDDyn { get { return Instance.CDDyn; } }
        public bool CDStatic { get { return Instance.CDStatic; } }
    
        public World World { get; internal set; }
        public bool IsSpawned { get { return World != null; } }

        internal WorldCell cell;

        #region Position
        internal Vec3f pos = new Vec3f(0, 0, 0);
        internal Vec3f dir = new Vec3f(0, 0, 1);

        public virtual Vec3f Position
        {
            get { return this.pos; }
            set
            {
                this.pos = value;
                if (this.IsSpawned)
                {
                    this.World.UpdatePosition(this, null);
                }
            }
        }

        public virtual Vec3f Direction
        {
            get { return this.dir; }
            set
            {
                if (value.IsNull())
                {
                    this.dir = new Vec3f(0, 0, 1);
                }
                else
                {
                    this.dir = value;
                }
                if (this.IsSpawned)
                {
                    VobMessage.WritePosDir(this.cell.SurroundingClients(), null);
                }
            }
        }
        #endregion

        public Vob(VobInstance instance, object scriptObject) : base(scriptObject)
        {
            this.Instance = instance;
        }

        public override void Create()
        {
            this.ID = Vob.idCount++;
            Network.Server.sVobs.Add(this);
            base.Create();
        }

        /// <summary> Despawns and removes the vob from the server. </summary>
        public override void Delete()
        {
            this.Despawn();
            Network.Server.sVobs.Remove(this);
            base.Delete();
        }

        #region Spawn
        public void Spawn(World world)
        {
            Spawn(world, this.pos, this.dir);
        }

        public void Spawn(World world, Vec3f position)
        {
            Spawn(world, position, this.dir);
        }

        public virtual void Spawn(World world, Vec3f position, Vec3f direction)
        {
            this.pos = position;
            this.dir = direction;
            world.SpawnVob(this);
        }

        public virtual void Despawn()
        {
            if (this.IsSpawned)
            {
                this.World.DespawnVob(this);
            }
        }
        #endregion

        public static Action<Vob, PacketWriter> OnWriteSpawn;
        internal virtual void WriteSpawn(PacketWriter stream)
        {
            stream.Write(ID);
            stream.Write(Instance.ID);
            stream.Write(pos);
            stream.Write(dir);
            //stream.Write(physicsEnabled);

            if (Vob.OnWriteSpawn != null)
                Vob.OnWriteSpawn(this, stream);
        }

        internal virtual void WriteSpawnMessage(IEnumerable<Client> list)
        {
            PacketWriter stream = Program.server.SetupStream(NetworkID.WorldVobSpawnMessage);
            this.WriteSpawn(stream);

            foreach (Client client in list)
            {
                client.Send(stream, PacketPriority.LOW_PRIORITY, PacketReliability.RELIABLE_ORDERED, 'W');
            }
        }

        internal void WriteDespawnMessage(IEnumerable<Client> list)
        {
            PacketWriter stream = Program.server.SetupStream(NetworkID.WorldVobDeleteMessage);
            stream.Write(this.ID);

            foreach (Client client in list)
                client.Send(stream, PacketPriority.HIGH_PRIORITY, PacketReliability.RELIABLE_ORDERED, 'W');
        }
    }
}