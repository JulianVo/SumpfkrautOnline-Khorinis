using System;
using Autofac;
using RP_Server_Scripts.Authentication;
using RP_Server_Scripts.Autofac;
using RP_Server_Scripts.Character.MessageHandler;
using RP_Server_Scripts.Character.MessageHandler.InformationWriter;
using RP_Server_Scripts.Character.Transaction;
using RP_Server_Scripts.Network;

namespace RP_Server_Scripts.Character
{
    public sealed class CharacterModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            var allConstructorFinder = new AllConstructorFinder();

            builder.RegisterType<CharacterVisualsWriter>().AsSelf().SingleInstance().FindConstructorsWith(allConstructorFinder);
            builder.RegisterType<DefaultSpawnPointProvider>().As<ISpawnPointProvider>().SingleInstance().FindConstructorsWith(allConstructorFinder);
            builder.RegisterType<CharacterTemplateSelector>().As<ICharacterTemplateSelector>().SingleInstance().FindConstructorsWith(allConstructorFinder);
            builder.RegisterType<CharacterBuilder>().AsSelf().SingleInstance().FindConstructorsWith(allConstructorFinder);

            //Transactions(Encapsulations of database code heavy actions that can be triggered via the CharacterService)
            builder.RegisterType<CreateHumanPlayerCharacterTransaction>().AsSelf().SingleInstance().FindConstructorsWith(allConstructorFinder);
            builder.RegisterType<CheckCharacterExistsTransaction>().AsSelf().SingleInstance().FindConstructorsWith(allConstructorFinder);
            builder.RegisterType<AddCharacterOwnerShipTransaction>().AsSelf().SingleInstance().FindConstructorsWith(allConstructorFinder);
            builder.RegisterType<GetAccountOwnedCharactersTransaction>().AsSelf().SingleInstance().FindConstructorsWith(allConstructorFinder);
            builder.RegisterType<SetAccountActiveCharacterTransaction>().AsSelf().SingleInstance().FindConstructorsWith(allConstructorFinder);
            builder.RegisterType<GetAccountActiveCharacterTransaction>().AsSelf().SingleInstance().FindConstructorsWith(allConstructorFinder);
            builder.RegisterType<GetCharacterOwnershipsCountTransaction>().AsSelf().SingleInstance().FindConstructorsWith(allConstructorFinder);

            //Register the service. This is a somewhat more complicated than because of the possible circular references with the transaction classes.
            builder.Register(context => new CharacterService()
            {
            }).OnActivated(args =>
            {
                args.Instance.AddCharacterOwnerShipTransaction =
                    args.Context.Resolve<AddCharacterOwnerShipTransaction>();
                args.Instance.GetAccountOwnedCharactersTransaction =
                    args.Context.Resolve<GetAccountOwnedCharactersTransaction>();
                args.Instance.CharacterExistsTransaction = args.Context.Resolve<CheckCharacterExistsTransaction>();
                args.Instance.CreateHumanPlayerCharacterTransaction =
                    args.Context.Resolve<CreateHumanPlayerCharacterTransaction>();
                args.Instance.SetAccountActiveCharacterTransaction =
                    args.Context.Resolve<SetAccountActiveCharacterTransaction>();
                args.Instance.GetAccountActiveCharacterTransaction =
                    args.Context.Resolve<GetAccountActiveCharacterTransaction>();
                args.Instance.GetCharacterOwnershipsCountTransaction =
                    args.Context.Resolve<GetCharacterOwnershipsCountTransaction>();
                args.Instance.AuthenticationService =
                    args.Context.Resolve<AuthenticationService>();
                //Initialize the service.
                args.Instance.Init();

            }).AsSelf()
                .SingleInstance();


            //Message Handling
            builder.RegisterType<CharacterCreationMessageHandler>().As<IScriptMessageHandler>().SingleInstance().FindConstructorsWith(allConstructorFinder);
            builder.RegisterType<RequestCharacterListMessageHandler>().As<IScriptMessageHandler>().SingleInstance().FindConstructorsWith(allConstructorFinder);
            builder.RegisterType<JoinGameMessageHandler>().As<IScriptMessageHandler>().SingleInstance().FindConstructorsWith(allConstructorFinder);
            builder.RegisterType<LeaveGameMessageHandler>().As<IScriptMessageHandler>().SingleInstance().FindConstructorsWith(allConstructorFinder);

            //Database initialization
            builder.RegisterType<CharacterItemsInitialization>().AsSelf().SingleInstance().AutoActivate().FindConstructorsWith(allConstructorFinder);

            //Other initializations
            builder.RegisterType<AccountLogoutHandling>().AsSelf().SingleInstance().AutoActivate();
        }
    }
}
