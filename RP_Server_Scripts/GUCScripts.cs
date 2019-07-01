using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Autofac;
using Autofac.Core;
using GUC.Animations;
using GUC.Log;
using GUC.Models;
using GUC.Network;
using GUC.Scripting;
using GUC.Types;
using GUC.WorldObjects;
using GUC.WorldObjects.Instances;
using GUC.WorldObjects.VobGuiding;
using RP_Server_Scripts.Chat;
using RP_Server_Scripts.Client;
using RP_Server_Scripts.Definitions;
using RP_Server_Scripts.Logging;
using RP_Server_Scripts.Network;
using RP_Server_Scripts.Network.ScriptMessages;
using RP_Server_Scripts.RP;
using RP_Server_Scripts.Threading;
using RP_Server_Scripts.Visuals;
using RP_Server_Scripts.VobSystem.Definitions;
using RP_Server_Scripts.WorldSystem;
using RP_Shared_Script;
using Module = Autofac.Module;

namespace RP_Server_Scripts
{
    public class GUCScripts : ScriptInterface
    {
        public const float BiggestNPCRadius = 150; // improveme
        public const float SmallestNPCRadius = 40;

        private readonly IClientFactory _ClientFactory;
        private readonly ClientList _ClientList = new ClientList();


        public GUCScripts()
        {



            Logger.Log("######## Initalise SumpfkrautOnline ServerScripts #########");



            var builder = new ContainerBuilder();

            //Config
            builder.RegisterInstance(RpConfig.Default).AsSelf().SingleInstance();



            builder.RegisterType<MainThreadDispatcher>().As<IMainThreadDispatcher>().SingleInstance();

            builder.RegisterAssemblyTypes(Assembly.GetAssembly(GetType()))
                .Where(t => typeof(IDefBuilder).IsAssignableFrom(t))
                .As<IDefBuilder>();
            builder.RegisterType<ScriptInitialization>().As<IStartable>().SingleInstance();
            builder.RegisterInstance(_ClientList).AsSelf().SingleInstance();
            builder.RegisterType<Chat.Chat>().AsImplementedInterfaces().AsSelf().SingleInstance();

            //Script message handler
            builder.RegisterType<ChatScriptMessageHandler>().As<IScriptMessageHandler>().SingleInstance();
            builder.RegisterType<ScriptMessageHandlerSelector>().As<IScriptMessageHandlerSelector>().SingleInstance();
            builder.RegisterType<SpectateScriptMessageHandler>().As<IScriptMessageHandler>().SingleInstance();
            builder.RegisterType<LeaveGameMessageHandler>().As<IScriptMessageHandler>().SingleInstance();

            builder.RegisterType<ClientFactory>().As<IClientFactory>().SingleInstance();
            builder.RegisterType<PacketWriterFactory>().As<IPacketWriterFactory>().SingleInstance();
            builder.RegisterType<WorldList>().AsSelf().SingleInstance();
            builder.RegisterType<BaseDefFactory>().As<IBaseDefFactory>().SingleInstance();
            builder.RegisterType<VobDefRegistration>().As<IVobDefRegistration>().SingleInstance();
            builder.RegisterType<NpcDefList>().AsImplementedInterfaces().SingleInstance();
            builder.RegisterType<VobDefList>().AsImplementedInterfaces().SingleInstance();
            builder.RegisterType<ChatCommandController>().AsSelf().SingleInstance().AutoActivate();
            builder.RegisterType<GucLoggerFactory>().As<ILoggerFactory>().SingleInstance();
            builder.RegisterType<HelpChatCommand>().As<IChatCommand>().SingleInstance().PropertiesAutowired(PropertyWiringOptions.AllowCircularDependencies);
            builder.RegisterType<ServerOptionsProvider>().AsSelf().SingleInstance();

            builder.RegisterType<ItemDefList>().As<IItemDefList>().As<IItemDefRegistration>().SingleInstance();

            //Shared
            builder.RegisterType<GermanNameValidator>().As<ICharacterNameValidator>().SingleInstance();


            //Load the module assemblies
            List<Assembly> moduleAssemblies = new List<Assembly>();


            string databaseModule = Path.Combine(Path.GetDirectoryName(new Uri(this.GetType().Assembly.CodeBase).AbsolutePath) ?? throw new InvalidOperationException(), "RP_Server_Scripts.Database.dll");
            moduleAssemblies.Add(Assembly.LoadFrom(databaseModule));

            string authenticationModulePath = Path.Combine(Path.GetDirectoryName(new Uri(this.GetType().Assembly.CodeBase).AbsolutePath) ?? throw new InvalidOperationException(), "RP_Server_Scripts.Authentication.dll");
            moduleAssemblies.Add(Assembly.LoadFrom(authenticationModulePath));

            string characterModule = Path.Combine(Path.GetDirectoryName(new Uri(this.GetType().Assembly.CodeBase).AbsolutePath) ?? throw new InvalidOperationException(), "RP_Server_Scripts.Character.dll");
            moduleAssemblies.Add(Assembly.LoadFrom(characterModule));

            foreach (var moduleAssembly in moduleAssemblies)
            {
                foreach (var typeInfo in moduleAssembly.DefinedTypes.Where(t => t.IsAssignableTo<Module>()))
                {
                    builder.RegisterModule((IModule)Activator.CreateInstance(typeInfo));
                }
            }


            //Built the DI container.
            var container = builder.Build();



            _ClientFactory = container.Resolve<IClientFactory>();

            _ClientList.ClientConnected += (sender, args) => { };


            CreateTestWorld();




            Logger.Log("######################## Finished #########################");

        }


        public GameClient CreateClient()
        {
            var client = _ClientFactory.Create();
            _ClientList.RegisterClient(client);

            //return new ArenaClient().BaseClient; //new ScriptClient().BaseClient;
            return client.BaseClient;
        }

        public AniJob CreateAniJob()
        {
            return new ScriptAniJob().BaseAniJob;
        }

        public Animation CreateAnimation()
        {
            return new ScriptAni().BaseAni;
        }

        public Overlay CreateOverlay()
        {
            return new ScriptOverlay().BaseOverlay;
        }

        public ModelInstance CreateModelInstance()
        {
            return new ModelDef().BaseDef;
        }

        public World CreateWorld()
        {
            //No idea what to return here. The server should never create the world. That part of the scripts job.
            return new WorldInst(new Guid().ToString()).BaseWorld;
        }

        public BaseVob CreateVob(byte type)
        {
            //BaseVobInst vob;
            //switch ((VobType)type)
            //{
            //    case VobType.Vob:
            //        vob = new VobInst();
            //        break;
            //    case VobType.Mob:
            //        vob = new MobInst();
            //        break;
            //    case VobType.Item:
            //        vob = new ItemInst();
            //        break;
            //    case VobType.NPC:
            //        vob = new NpcInst();
            //        break;
            //    case VobType.Projectile:
            //        vob = new ProjInst();
            //        break;
            //    default:
            //        throw new Exception("Unsupported VobType: " + (VobType)type);
            //}

            //return vob.BaseInst;
            return null;
        }


        public BaseVobInstance CreateInstance(byte type)
        {
            //BaseVobDef def;
            //switch ((VobType)type)
            //{
            //    case VobType.Vob:
            //        def = new VobDef();
            //        break;
            //    case VobType.Mob:
            //        def = new MobDef();
            //        break;
            //    case VobType.NPC:
            //        def = new NpcDef();
            //        break;
            //    case VobType.Item:
            //        def = new ItemDef();
            //        break;
            //    case VobType.Projectile:
            //        def = new ProjDef();
            //        break;
            //    default:
            //        throw new Exception("Unsupported VobType: " + (VobType)type);
            //}

            //return def.BaseDef;
            return null;
        }

        public GuideCmd CreateGuideCommand(byte type)
        {
            return null;
        }

        public TargetCmd GetTestCmd(BaseVob target)
        {
            return null;
        }




        private void CreateTestWorld()
        {
            var world = new WorldInst(RpConfig.Default.DefaultSpawnWorld);
            world.Create();
            world.Clock.SetTime(new WorldTime(0, 8), 15.0f);
            world.Clock.Stop();
            WorldInst.List.Add(world);
        }
    }
}