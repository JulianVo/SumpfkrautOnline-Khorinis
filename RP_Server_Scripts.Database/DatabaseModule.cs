using System;
using Autofac;
using RP_Server_Scripts.Autofac;
using RP_Server_Scripts.Database.Account;
using RP_Server_Scripts.Database.Character;

namespace RP_Server_Scripts.Database
{
    public class DatabaseModule:Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            var allConstructorFinder = new AllConstructorFinder();

            builder.RegisterType<DatabaseInitialization>().As<IStartable>().SingleInstance().FindConstructorsWith(allConstructorFinder).AutoActivate();
            builder.RegisterType<AuthenticationContextFactory>().As<IAuthenticationContextFactory>().SingleInstance().FindConstructorsWith(allConstructorFinder);
            builder.RegisterType<CharacterManagementContextFactory>().As<ICharacterManagementContextFactory>().SingleInstance().FindConstructorsWith(allConstructorFinder);
        }
    }
}
