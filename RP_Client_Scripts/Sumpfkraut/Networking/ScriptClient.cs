using System;
using GUC.Network;
using GUC.Scripts.Arena;
using GUC.Scripts.Logging;
using GUC.Scripts.Sumpfkraut.Controls;
using GUC.Scripts.Sumpfkraut.VobSystem.Instances;
using GUC.WorldObjects;
using GUC.Types;
using GUC.Scripts.Sumpfkraut.WorldSystem;
using GUC.Utilities;
using RP_Shared_Script;

namespace GUC.Scripts.Sumpfkraut.Networking
{
    public class ScriptClient : ExtendedObject, GameClient.IScriptClient
    {
        private readonly Chat _Chat;
        private readonly IPacketWriterFactory _WriterFactory;
        private readonly ScriptMessageSender _MessageSender;
        private readonly IScriptMessageHandlerSelector _HandlerSelector;
        private readonly ILogger _Log;

        public ScriptClient(Chat chat, IPacketWriterFactory writerFactory, ScriptMessageSender messageSender, IScriptMessageHandlerSelector handlerSelector, ILoggerFactory loggerFactory)
        {
            if (loggerFactory == null)
            {
                throw new ArgumentNullException(nameof(loggerFactory));
            }

            _Chat = chat ?? throw new ArgumentNullException(nameof(chat));
            _WriterFactory = writerFactory;
            _MessageSender = messageSender ?? throw new ArgumentNullException(nameof(messageSender));
            _HandlerSelector = handlerSelector ?? throw new ArgumentNullException(nameof(handlerSelector));
            this.BaseClient = new GameClient(this);

            _Log = loggerFactory.GetLogger(GetType());
        }

        public int Id => this.BaseClient.ID;

        public GameClient BaseClient { get; }

        public NPCInst Character => (NPCInst)this.BaseClient.Character?.ScriptObject;

        /// <summary> Spawned or specating in a world. </summary>
        public bool IsIngame => this.BaseClient.IsIngame;

        public bool IsSpecating => this.BaseClient.IsSpectating;
        public bool IsCharacter => this.BaseClient.Character != null;

        public virtual void OnConnection()
        {
        }

        public virtual void OnDisconnection(int id)
        {
        }

        public void SetControl(NPC npc)
        {
            this.SetControl((NPCInst)npc.ScriptObject);
        }

        public virtual void SetControl(NPCInst npc)
        {
            this.BaseClient.SetControl(npc.BaseInst);
            Menus.PlayerInventory.Menu.Close();
            PlayerFocus.Activate(npc);
        }

        public void SetToSpectator(World world, Vec3f pos, Angles ang)
        {
            this.SetToSpectator((WorldInst)world.ScriptObject, pos, ang);
        }

        public void SetToSpectator(WorldInst world, Vec3f pos, Angles ang)
        {
            this.BaseClient.SetToSpectate(world.BaseWorld, pos, ang);
            Menus.PlayerInventory.Menu.Close();
        }

        public void OnReadProperties(PacketReader stream)
        {
        }

        public void OnWriteProperties(PacketWriter stream)
        {
        }

        public static ScriptClient Client => (ScriptClient)GameClient.Client.ScriptObject;




        public virtual void ReadScriptMessage(PacketReader stream)
        {
            if (stream == null)
            {
                throw new ArgumentNullException(nameof(stream));
            }

            ScriptMessages id = (ScriptMessages)stream.ReadByte();
            Log.Logger.Log(id);
            switch (id)
            {
                case ScriptMessages.PlayerQuit:
                    break;

                case ScriptMessages.GameInfo:
                    ReadGameInfo(stream);
                    break;

                case ScriptMessages.ChatMessage:
                    _Chat.ReadMessage(stream);
                    break;
                case ScriptMessages.ChatTeamMessage:
                    _Chat.ReadTeamMessage(stream);
                    break;
                default:
                    {
                        if (_HandlerSelector.TryGetMessageHandler(id, out IScriptMessageHandler handler))
                        {
                            handler.HandleMessage(this, stream);
                        }
                        else
                        {
                            _Log.Warn($"Unknown script message with id '{id}' received.");
                        }
                    }
                    break;
            }
        }

        public virtual void ReadScriptVobMessage(PacketReader stream, WorldObjects.BaseVob vob)
        {
            ((BaseVobInst)vob.ScriptObject).OnReadScriptVobMsg(stream);
        }


        public void SendJoinGameMessage()
        {
            var stream = _WriterFactory.Create(ScriptMessages.JoinGame);
            _MessageSender.SendScriptMessage(stream, NetPriority.Low, NetReliability.Reliable);
        }


        void ReadGameInfo(PacketReader stream)
        {
            PlayerInfo.ReadHeroInfo(stream);
            int count = stream.ReadByte();
            for (int i = 0; i < count; i++)
                PlayerInfo.ReadPlayerInfoMessage(stream);

        }

        public PacketWriter GetStream(ScriptMessages id)
        {
            var s = GameClient.GetScriptMessageStream();
            s.Write((byte)id);
            return s;
        }
    }
}