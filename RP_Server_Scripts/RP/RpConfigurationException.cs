using System;
using System.Runtime.Serialization;

namespace RP_Server_Scripts.RP
{
   public class RpConfigurationException:Exception
    {
        public RpConfigurationException()
        {
        }

        public RpConfigurationException(string message) : base(message)
        {
        }

        public RpConfigurationException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected RpConfigurationException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
