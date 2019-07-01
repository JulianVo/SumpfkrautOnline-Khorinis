using System;
using System.Collections.Generic;

namespace GUC.Scripts.Logging
{
    internal class GucLoggerFactory : ILoggerFactory
    {
        private readonly  Dictionary<string,ILogger> _Loggers = new Dictionary<string, ILogger>();
        private  readonly  object _Lock = new object();

        public ILogger GetLogger(string loggerName)
        {
            if (string.IsNullOrWhiteSpace(loggerName))
            {
                throw new ArgumentException(@"Value cannot be null or whitespace.", nameof(loggerName));
            }

            lock (_Lock)
            {
                if (_Loggers.TryGetValue(loggerName.ToUpperInvariant(), out ILogger logger))
                {
                    return logger;
                }

                logger = new GucLogger(loggerName);
                _Loggers.Add(loggerName.ToUpperInvariant(), logger);

                return logger;
            }
        }

        public ILogger GetLogger(Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            return GetLogger(type.Name);
        }
    }
}