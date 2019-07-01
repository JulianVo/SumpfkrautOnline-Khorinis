using System;
using Autofac;
using GUC.Log;
using GUC.Scripting;
using GUC.Scripts.Account;
using GUC.Scripts.Arena;
using GUC.Scripts.Arena.Controls;
using GUC.Scripts.Character;
using GUC.Scripts.GuiEventWiring;
using GUC.Scripts.Logging;
using GUC.Scripts.Menus;
using GUC.Scripts.Menus.AccountCreationMenu;
using GUC.Scripts.Menus.CharacterCreationGUI;
using GUC.Scripts.Menus.CharacterSelectionMenu;
using GUC.Scripts.Menus.IngameMenu;
using GUC.Scripts.Sumpfkraut.VobSystem;
using GUC.Scripts.Sumpfkraut.Visuals;
using GUC.Scripts.Sumpfkraut.VobSystem.Instances;
using GUC.Scripts.Sumpfkraut.VobSystem.Definitions;
using GUC.Scripts.Sumpfkraut.VobSystem.Instances.Mobs;
using GUC.Scripts.Sumpfkraut.VobSystem.Definitions.Mobs;
using GUC.Scripts.Sumpfkraut.WorldSystem;
using GUC.WorldObjects.VobGuiding;
using GUC.Scripts.Sumpfkraut.AI.GuideCommands;
using GUC.Scripts.Sumpfkraut.Controls;
using GUC.Scripts.Sumpfkraut.Menus;
using GUC.Scripts.Sumpfkraut.Networking;
using GUC.Scripts.Sumpfkraut.Networking.MessageHandler;
using GUC.Types;
using GUC.WorldObjects;
using RP_Server_Scripts.Autofac;
using RP_Shared_Script;
using ExitMenu = GUC.Scripts.Arena.Menus.ExitMenu;
using LoginMenu = GUC.Scripts.Menus.LoginMenu;
using MainMenu = GUC.Scripts.Menus.MainMenu;
using StatusMenu = GUC.Scripts.Arena.Menus.StatusMenu;

namespace GUC.Scripts
{
    public class GUCScripts : ScriptInterface
    {
        public static bool Ingame = false;

        private IContainer _DiContainer;
        private readonly ContainerBuilder _DiBuilder;

        public GUCScripts()
        {
            var allConstructorFinder = new AllConstructorFinder();

            //Use the autofac dependency injection container to build the main objects.
            ContainerBuilder builder = new ContainerBuilder();
            builder.RegisterType<ScriptClient>().AsSelf().SingleInstance();
            builder.RegisterType<ArenaControl>().AsSelf().SingleInstance();
            builder.RegisterType<PacketWriterFactory>().As<IPacketWriterFactory>().SingleInstance();
            builder.RegisterType<ScriptMessageSender>().AsSelf().SingleInstance();
            builder.RegisterType<Chat>().AsSelf().SingleInstance();
            builder.RegisterType<GameState>().AsSelf().SingleInstance();
            builder.RegisterType<LoginPacketWriter>().AsSelf().SingleInstance();
            builder.RegisterType<Login.Login>().AsSelf().SingleInstance();
            builder.RegisterType<GucLoggerFactory>().As<ILoggerFactory>().SingleInstance();
            builder.RegisterType<CharacterList>().AsSelf().SingleInstance().FindConstructorsWith(allConstructorFinder);
            builder.RegisterType<LeaveGameSender>().AsSelf().SingleInstance();
            builder.RegisterType<CharListRequestSender>().AsSelf().SingleInstance();
            builder.RegisterType<AccountCreationMessageWriter>().AsSelf().SingleInstance();
            builder.RegisterType<AccountCreation>().AsSelf().SingleInstance();
            builder.RegisterType<CharacterCreation>().AsSelf().SingleInstance();
            builder.RegisterType<NpcDefList>().AsSelf().SingleInstance();
            builder.RegisterType<CharacterVisualsReader>().AsSelf().SingleInstance();
            


            //Message handling
            builder.RegisterType<ScriptMessageHandlerSelector>().As<IScriptMessageHandlerSelector>().SingleInstance();
            builder.RegisterType<LoginDeniedMessageHandler>().As<IScriptMessageHandler>().AsSelf().SingleInstance();
            builder.RegisterType<LoginAcknowledgedMessageHandler>().As<IScriptMessageHandler>().AsSelf().SingleInstance();
            builder.RegisterType<LogoutAcknowledgeMessageHandler>().As<IScriptMessageHandler>().AsSelf().SingleInstance();
            builder.RegisterType<AccountCreationResultMessageHandler>().As<IScriptMessageHandler>().AsSelf().SingleInstance();
            builder.RegisterType<CharacterCreationResultMessageHandler>().As<IScriptMessageHandler>().AsSelf().SingleInstance();
            builder.RegisterType<CharacterListResultMessageHandler>().As<IScriptMessageHandler>().AsSelf().SingleInstance();

            //GUI Menus
            builder.RegisterType<CharCreationMenu>().AsSelf().As<IClosableMenu>().SingleInstance();
            builder.RegisterType<MainMenu>().AsSelf().As<IClosableMenu>().SingleInstance();
            builder.RegisterType<ChatMenu>().AsSelf().As<IClosableMenu>().SingleInstance();
            builder.RegisterType<ExitMenu>().AsSelf().As<IClosableMenu>().SingleInstance();
            builder.RegisterType<StatusMenu>().AsSelf().As<IClosableMenu>().SingleInstance();
            builder.RegisterType<LoginMenu>().AsSelf().As<IClosableMenu>().SingleInstance();
            builder.RegisterType<WaitScreen>().As<IClosableMenu>().AsSelf().SingleInstance();
            builder.RegisterType<InGameMenu>().AsSelf().As<IClosableMenu>().SingleInstance();
            builder.RegisterType<AccountCreationMenu>().AsSelf().As<IClosableMenu>().SingleInstance();
            builder.RegisterType<CharacterSelectionMenu>().AsSelf().As<IClosableMenu>().SingleInstance();


            //Register event wiring classes used to wire the events between GUI views without circular references(and other bad stuff).
            builder.RegisterType<MainMenuEventWiring>().AsSelf().SingleInstance().AutoActivate();
            builder.RegisterType<ExitMenuEventWiring>().AsSelf().SingleInstance().AutoActivate();
            builder.RegisterType<LoginMenuWiring>().AsSelf().SingleInstance().AutoActivate();
            builder.RegisterType<InGameMenuWiring>().AsSelf().SingleInstance().AutoActivate();
            builder.RegisterType<CharacterLoadingEventWiring>().AsSelf().SingleInstance().AutoActivate();
            builder.RegisterType<AccountCreationMenuEventWiring>().AsSelf().SingleInstance().AutoActivate();
            builder.RegisterType<CharCreationMenuEventWiring>().AsSelf().SingleInstance().AutoActivate();
            builder.RegisterType<CharacterSelectionMenuEventWiring>().AsSelf().SingleInstance().AutoActivate();

            
            //Shared
            builder.RegisterType<GermanNameValidator>().As<ICharacterNameValidator>().SingleInstance();



            _DiBuilder = builder;
        }



        public static event Action<long> OnUpdate;
        public void Update(long ticks)
        {
            GUCMenu.UpdateMenus(ticks);
            InputControl.UpdateControls(ticks);
            OnUpdate?.Invoke(ticks);
            CheckMusic();
            CheckPosition();
        }

        SoundInstance _MenuTheme = null;
        public static event Action OnOutgame;
        public void StartOutgame()
        {
            var theme = new SoundDefinition("INSTALLER_LOOP.WAV");
            theme.zSFX.IsFixed = true;
            theme.zSFX.Volume = 0.5f;
            theme.zSFX.SetLooping(true);
            _MenuTheme = SoundHandler.PlaySound(theme, 0.5f);

            if (_DiContainer == null)
            {
                _DiContainer = _DiBuilder.Build();
            }
            _DiContainer.Resolve<LoginMenu>().Open();
            OnIngame += () => _DiContainer.Resolve<ScriptClient>().SendJoinGameMessage();

            OnOutgame?.Invoke();
            Logger.Log("Outgame started.");


            InputControl.Active = _DiContainer.Resolve<ArenaControl>();


            Logger.Log("SumpfkrautOnline ClientScripts loaded!");
        }

        public static event Action OnIngame;
        public void StartIngame()
        {
            // stop oCAniCtrl_Human::_Stand from canceling the s_bowaim animation
            WinApi.Process.Write(0x006B7772, 0xEB, 0x69);

            // remove SetTorchAni
            WinApi.Process.Write(0x0073B410, 0xC2, 0x08, 0x00);
            Gothic.Objects.oCNpcFocus.SetFocusMode(1);

            Ingame = true;

            OnIngame?.Invoke();
            Logger.Log("Ingame started.");
        }

        void CheckMusic()
        {
            if (Ingame && _Client.IsCharacter && !_Client.Character.IsDead)
            {
                if (_Client.Character.TeamID >= 0)
                {
                    bool enemyCloseBy = false;
                    var heroPos = _Client.Character.GetPosition();
                    int distance = SoundHandler.CurrentMusicType == SoundHandler.MusicType.Fight ? 3000 : 1000;
                    WorldObjects.World.Current.ForEachVobPredicate(v =>
                    {
                        if (Cast.Try(v.ScriptObject, out NPCInst npc) && !npc.IsDead
                            && npc.TeamID != -1 && npc.TeamID != _Client.Character.TeamID)
                        {
                            if (heroPos.GetDistance(npc.GetPosition()) < distance)
                            {
                                enemyCloseBy = true;
                                return false;
                            }
                        }
                        return true;
                    });

                    SoundHandler.CurrentMusicType = enemyCloseBy ? SoundHandler.MusicType.Fight : SoundHandler.MusicType.Normal;
                    return;
                }
            }
            SoundHandler.CurrentMusicType = SoundHandler.MusicType.Normal;
        }

        Vec3f _LastValidPos = Vec3f.Null;
        bool _DoneUncon = false;
        readonly Utilities.LockTimer _SwimTimer = new Utilities.LockTimer(3000);

        void DoUnconstuff(NPCInst hero)
        {
            if (hero?.Environment == null || hero?.BaseInst?.gAI==null)
            {
                return;
            }

            var env = hero.Environment;
            if (env.WaterLevel < 0.2f)
            {
                if (env.InAir) return;
                var gAi = hero.BaseInst.gAI;
                if (!gAi.CheckEnoughSpaceMoveForward(false)) return;
                if (!gAi.CheckEnoughSpaceMoveBackward(false)) return;
                if (!gAi.CheckEnoughSpaceMoveLeft(false)) return;
                if (!gAi.CheckEnoughSpaceMoveRight(false)) return;

                _LastValidPos = hero.GetPosition();
            }
            else
            {
                if (!hero.IsUnconscious && env.WaterLevel > 0.3f && _SwimTimer.IsReady)
                {
                    ScreenScrollText.AddText("Du kannst ja gar nicht schwimmen!?!");
                }

                if (!hero.IsUnconscious)
                    _DoneUncon = false;

                if (!_DoneUncon && hero.IsUnconscious)
                {
                    hero.BaseInst.SetPhysics(false);
                    var rb = WinApi.Process.ReadInt(hero.BaseInst.gVob.Address + 224);
                    using (var vec = Vec3f.Null.CreateGVec())
                        WinApi.Process.THISCALL<WinApi.NullReturnCall>(rb, 0x5B66D0, vec);
                    Vec3f.Null.SetGVec(hero.BaseInst.gAI.Velocity);
                    hero.SetPosition(_LastValidPos);
                    _DoneUncon = true;
                }
            }
        }

        void CheckPosition()
        {
            var hero = _Client.Character;
            if (hero != null && !hero.IsDead)
            {
                DoUnconstuff(hero);
            }
        }

        public static event Action OnWorldEnter;
        public void FirstWorldRender()
        {
            if (_MenuTheme != null)
            {
                SoundHandler.StopSound(_MenuTheme);
                _MenuTheme = null;
            }

            OnWorldEnter?.Invoke();
        }

        private ScriptClient _Client;



        public Network.GameClient CreateClient()
        {
            //throw new Exception("Why should the base script need to create a script client?");
            ////return new ScriptClient().BaseClient; //;
            /// 
            if (_DiContainer == null)
            {
                _DiContainer = _DiBuilder.Build();
            }

            _Client = _DiContainer.Resolve<ScriptClient>();
            return _Client.BaseClient;
        }

        public Animations.AniJob CreateAniJob()
        {
            return new ScriptAniJob().BaseAniJob;
        }

        public Animations.Animation CreateAnimation()
        {
            return new ScriptAni().BaseAni;
        }

        public Animations.Overlay CreateOverlay()
        {
            return new ScriptOverlay().BaseOverlay;
        }

        public Models.ModelInstance CreateModelInstance()
        {
            return new ModelDef().BaseDef;
        }

        public World CreateWorld()
        {
            return new WorldInst().BaseWorld;
        }

        public BaseVob CreateVob(byte type)
        {
            BaseVobInst vob;
            switch ((VobType)type)
            {
                case VobType.Vob:
                    vob = new VobInst();
                    break;
                case VobType.Mob:
                    vob = new MobInst();
                    break;
                case VobType.Item:
                    vob = new ItemInst();
                    break;
                case VobType.NPC:
                    vob = new NPCInst();
                    break;
                case VobType.Projectile:
                    vob = new ProjInst();
                    break;
                default:
                    throw new Exception("Unsupported VobType: " + (VobType)type);
            }
            return vob.BaseInst;
        }


        public WorldObjects.Instances.BaseVobInstance CreateInstance(byte type)
        {
            BaseVobDef def;
            switch ((VobType)type)
            {
                case VobType.Vob:
                    def = new VobDef();
                    break;
                case VobType.Mob:
                    def = new MobDef();
                    break;
                case VobType.NPC:
                    def = new NPCDef();
                    break;
                case VobType.Item:
                    def = new ItemDef();
                    break;
                case VobType.Projectile:
                    def = new ProjDef();
                    break;
                default:
                    throw new Exception("Unsupported VobType: " + (VobType)type);
            }

            return def.BaseDef;
        }

        public GuideCmd CreateGuideCommand(byte type)
        {
            switch ((CommandType)type)
            {
                case CommandType.GoToPos:
                    return new GoToPosCommand();
                case CommandType.GoToVob:
                    return new GoToVobCommand();
                case CommandType.GoToVobLookAt:
                    return new GoToVobLookAtCommand();
                default:
                    throw new Exception("Unsupported guide command type: " + type);
            }
        }
    }
}
