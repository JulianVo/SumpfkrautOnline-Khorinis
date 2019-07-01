using System;
using Autofac;
using RP_Server_Scripts.Authentication.ChatCommands;
using RP_Server_Scripts.Authentication.MessageHandler;
using RP_Server_Scripts.Autofac;
using RP_Server_Scripts.Chat;
using RP_Server_Scripts.Logging;
using RP_Server_Scripts.Network;
using RP_Shared_Script;
using Module = Autofac.Module;

namespace RP_Server_Scripts.Authentication
{
    /// <summary>
    /// Autofac module of the Authentication module.
    /// </summary>
    public sealed class AuthenticationModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            var allConstructorFinder = new AllConstructorFinder();

            builder.RegisterType<AuthenticationService>().AsSelf().SingleInstance().FindConstructorsWith(allConstructorFinder);
            builder.RegisterType<BCrypPasswordService>().As<IPasswordService>().SingleInstance().FindConstructorsWith(allConstructorFinder);
            builder.RegisterType<LoginStateChatCommand>().As<IChatCommand>().SingleInstance().FindConstructorsWith(allConstructorFinder);
            builder.RegisterType<RegisterChatCommand>().As<IChatCommand>().SingleInstance().FindConstructorsWith(allConstructorFinder);
            builder.RegisterType<LogOutChatCommand>().As<IChatCommand>().SingleInstance().FindConstructorsWith(allConstructorFinder);
            builder.RegisterType<LoginMessageHandler>().As<IScriptMessageHandler>().SingleInstance().FindConstructorsWith(allConstructorFinder);
            builder.RegisterType<LogoutMessageHandler>().As<IScriptMessageHandler>().SingleInstance().FindConstructorsWith(allConstructorFinder);
            builder.RegisterType<CreateAccountMessageHandler>().As<IScriptMessageHandler>().SingleInstance().FindConstructorsWith(allConstructorFinder);
            

            //Add a decorator to all script message handlers that do not handle login messages. The decorator automatically blocks all non login/create messages
            // from clients that are not yet logged in. Never trust the client...
            builder.RegisterDecorator<IScriptMessageHandler>((context, parameters, instance) =>
            {
                if (instance.SupportedMessage != ScriptMessages.Login && instance.SupportedMessage != ScriptMessages.CreateAccount)
                {
                    return new AuthenticatedMessageHandlerDecorator(instance, context.Resolve<AuthenticationService>(),
                        context.Resolve<ILoggerFactory>());
                }
                else
                {
                    return instance;
                }
            });
        }
    }
}
