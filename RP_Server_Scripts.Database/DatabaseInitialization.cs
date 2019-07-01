using System;
using Autofac;
using RP_Server_Scripts.Database.Properties;
using RP_Server_Scripts.Logging;

namespace RP_Server_Scripts.Database
{
    internal class DatabaseInitialization : IStartable
    {
        private readonly ILogger _Log;

        public DatabaseInitialization(ILoggerFactory loggerFactory)
        {
            if (loggerFactory == null)
            {
                throw new ArgumentNullException(nameof(loggerFactory));
            }

            _Log = loggerFactory.GetLogger(GetType());
        }


        public void Start()
        {
            using (var context = new GlobalContext())
            {
                _Log.Info(context.Database.CreateIfNotExists()
                    ? Resources.Msg_DatabaseWasCreated
                    : Resources.Msg_DatabaseFound);
            }
        }
    }
}
