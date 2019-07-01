using System;
using System.Runtime.Serialization;

namespace RP_Server_Scripts.Database
{
    public class DatabaseRequestException : Exception
    {
        public DatabaseRequestException()
        {
        }

        public DatabaseRequestException(string message) : base(message)
        {
        }

        public DatabaseRequestException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected DatabaseRequestException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
