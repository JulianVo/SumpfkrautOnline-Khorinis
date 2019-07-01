using System;

namespace RP_Server_Scripts.Logging
{
    public interface ILoggerFactory
    {
        ILogger GetLogger(string loggerName);

        ILogger GetLogger(Type type);
    }
}
