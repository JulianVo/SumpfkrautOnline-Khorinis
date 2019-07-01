using System;
using GUC.Log;

namespace RP_Server_Scripts.Logging
{
    internal sealed class GucLogger : ILogger
    {
        private readonly string _LoggerName;

        public GucLogger(string loggerName)
        {
            if (string.IsNullOrWhiteSpace(loggerName))
            {
                throw new ArgumentException(@"Value cannot be null or whitespace.", nameof(loggerName));
            }

            _LoggerName = loggerName;
        }

        public void Debug(string message)
        {
            if (string.IsNullOrWhiteSpace(message))
            {
                throw new ArgumentException(@"Value cannot be null or whitespace.", nameof(message));
            }

            Logger.Log(Logger.LOG_PRINT, message);
        }

        public void Info(string message)
        {
            if (string.IsNullOrWhiteSpace(message))
            {
                throw new ArgumentException(@"Value cannot be null or whitespace.", nameof(message));
            }

            Logger.Log(Logger.LOG_INFO,message);
        }

        public void Warn(string message)
        {
            if (string.IsNullOrWhiteSpace(message))
            {
                throw new ArgumentException(@"Value cannot be null or whitespace.", nameof(message));
            }

            Logger.Log(Logger.LOG_WARNING, message);
        }

        public void Error(string message)
        {
            if (string.IsNullOrWhiteSpace(message))
            {
                throw new ArgumentException(@"Value cannot be null or whitespace.", nameof(message));
            }

            Logger.Log(Logger.LOG_ERROR, message);
        }

        public void Fatal(string message)
        {
            if (string.IsNullOrWhiteSpace(message))
            {
                throw new ArgumentException(@"Value cannot be null or whitespace.", nameof(message));
            }

            Logger.Log(Logger.LOG_ERROR, message);
        }
    }
}
