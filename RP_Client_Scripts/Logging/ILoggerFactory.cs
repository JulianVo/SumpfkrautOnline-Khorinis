using System;

namespace GUC.Scripts.Logging
{
    public interface ILoggerFactory
    {
        ILogger GetLogger(string loggerName);

        ILogger GetLogger(Type type);
    }
}
