using System;
using GUC;
using GUC.GameObjects;
using GUC.Log;
using GUC.Network;
using GUC.Types;
using GUC.WorldObjects;
using GUC.WorldObjects.VobGuiding;
using RP_Server_Scripts.Network;
using RP_Server_Scripts.VobSystem.Instances;
using RP_Server_Scripts.WorldSystem;
using RP_Shared_Script;

namespace RP_Server_Scripts.Client
{
    public class Client : GameClient.IScriptClient
    {
        private readonly IScriptMessageHandlerSelector _HandlerSelector;
        private readonly IPacketWriterFactory _StreamFactory;
        private readonly ClientList _ClientList;

        internal Client(IScriptMessageHandlerSelector handlerSelector, IPacketWriterFactory streamFactory, ClientList clientList)
        {
            _HandlerSelector = handlerSelector ?? throw new ArgumentNullException(nameof(handlerSelector));
            _StreamFactory = streamFactory ?? throw new ArgumentNullException(nameof(streamFactory));
            _ClientList = clientList ?? throw new ArgumentNullException(nameof(clientList));
            BaseClient = new GameClient(this);
        }

        /// <summary>
        /// Gets the id that identifies the current <see cref="Client"/> within the active server instance.
        /// </summary>
        public int Id => BaseClient.ID;

        public GameClient BaseClient { get; }

        public NpcInst ControlledNpc => (NpcInst)BaseClient.Character?.ScriptObject;

        public bool TryGetControlledNpc(out NpcInst npc)
        {
            npc = ControlledNpc;
            return npc != null;
        }

        public event EventHandler<ClientDisconnectedEventArgs> OnDisconnected;
        public event EventHandler<ClientConnectedEventArgs> OnConnected;

        /// <summary>
        /// Gets a value indicating whether or not the current <see cref="Client"/> is connected.
        /// </summary>
        public bool IsConnected => BaseClient.IsConnected;


        /// <summary>
        /// Closes the connection to the current <see cref="Client"/>.
        /// </summary>
        public void Disconnect() => BaseClient.Disconnect();

        /// <summary>
        /// Sets the <see cref="NpcInst"/> that this client is controlling.
        /// </summary>
        /// <param name="npc">The <see cref="NpcInst"/> that should be controlled by the current <see cref="Client"/>.</param>
        public void SetControl(NpcInst npc)
        {
            BaseClient.SetControl(npc?.BaseInst);
        }

        /// <summary>
        /// Removes the control of the current <see cref="Client"/> from its <see cref="ControlledNpc"/>(if it has one).
        /// <remarks>Does nothing if the current <see cref="Client"/> does not have a character.</remarks>
        /// </summary>
        public void RemoveControl()
        {
            if (ControlledNpc != null)
            {
                BaseClient.SetControl(null);
            }
        }

        void GameObject.IScriptGameObject.OnReadProperties(PacketReader stream)
        {

        }

        void GameObject.IScriptGameObject.OnWriteProperties(PacketWriter stream)
        {

        }

        /// <summary>
        /// Gets the system address of the <see cref="Client"/>.
        /// </summary>
        public string SystemAddress => BaseClient.SystemAddress;

        /// <summary>
        /// Sends a message to the chat of the <see cref="Client"/>.
        /// </summary>
        /// <param name="message">The message that should be send to the chat of the <see cref="Client"/></param>
        public void SendChatMessage(string message)
        {
            if (string.IsNullOrWhiteSpace(message))
            {
                throw new ArgumentException(@"Value cannot be null or whitespace.", nameof(message));
            }

            var stream = _StreamFactory.GetScriptMessageStream();
            stream.Write((byte)ScriptMessages.ChatMessage);
            stream.Write(message);
            BaseClient.SendScriptMessage(stream, NetPriority.Low, NetReliability.Reliable);
        }

        public void SendServerNotification(string message)
        {
            SendChatMessage("<Server>:" + message);
        }

        public void SendScriptMessage(PacketWriter writer,NetPriority priority,NetReliability reliability)
        {
            if (writer == null)
            {
                throw new ArgumentNullException(nameof(writer));
            }
            Logger.Log(Logger.LOG_INFO, $"Send Script message to '{SystemAddress}'");
            BaseClient.SendScriptMessage(writer, priority, reliability);
        }


        void GameClient.IScriptClient.OnConnection()
        {
            OnConnected?.Invoke(this, new ClientConnectedEventArgs(this));
        }

        void GameClient.IScriptClient.OnDisconnection(int id)
        {
            this.ControlledNpc?.Despawn();
            var s = _StreamFactory.GetScriptMessageStream(ScriptMessages.PlayerQuit);
            s.Write((byte)id);
            OnDisconnected?.Invoke(this, new ClientDisconnectedEventArgs(this));
            _ClientList.ForEach(c => c.BaseClient.SendScriptMessage(s, NetPriority.Low, NetReliability.ReliableOrdered));
        }

        void GameClient.IScriptClient.SetControl(NPC npc)
        {
            if (npc == null)
            {
                throw new ArgumentNullException(nameof(npc));
            }

            var npcInst = (NpcInst)npc.ScriptObject;
            // old npc
            if (npc.IsSpawned && npc.IsDead)
            {
                npcInst.World.DespawnList_NPC.AddVob(npcInst);
            }
            BaseClient.SetControl(npcInst.BaseInst);
            // new npc
            if (npcInst.IsSpawned && npcInst.IsDead)
            {
                npcInst.World.DespawnList_NPC.RemoveVob(npcInst);
            }
        }

        void GameClient.IScriptClient.SetToSpectator(World world, Vec3f pos, Angles ang)
        {
            if (world == null)
            {
                throw new ArgumentNullException(nameof(world));
            }

            BaseClient.SetToSpectate(((WorldInst)world.ScriptObject).BaseWorld, pos, ang);
        }

        bool GameClient.IScriptClient.IsAllowedToConnect()
        {
            return true;
        }

        void GameClient.IScriptClient.ReadScriptMessage(PacketReader stream)
        {
            ScriptMessages id = (ScriptMessages)stream.ReadByte();
            Logger.Log(Logger.LOG_INFO,$"Received Script message '{id}' from '{SystemAddress}'");

            //Log.Logger.Log(id);
            if (_HandlerSelector.TryGetMessageHandler(id, out IScriptMessageHandler handler))
            {
                handler.HandleMessage(this, stream);
            }
            else
            {
                Logger.LogWarning($"Unhandled ScriptMessage '{id.ToString()}'");
            }
        }

        void GameClient.IScriptClient.ReadScriptRequestMessage(PacketReader stream, GuidedVob vob)
        {
            RequestMessageIDs id = (RequestMessageIDs)stream.ReadByte();

            if (id > RequestMessageIDs.MaxGuidedMessages && vob != ControlledNpc.BaseInst)
            {
                return; // client sent a request for a bot which is only allowed for players
            }

            if (id < RequestMessageIDs.MaxNPCRequests)
            {
                if (vob is NPC)
                {
                    NpcInst.Requests.ReadRequest(id, stream, (NpcInst)vob.ScriptObject);
                }
            }
        }
    }
}
