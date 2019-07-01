using System;
using System.Runtime.Serialization;

namespace RP_Shared_Script
{
    public sealed class ScriptMessageHandlingException : Exception
    {
        public ScriptMessageHandlingException()
        {
        }

        public ScriptMessageHandlingException(string message) : base(message)
        {
        }

        public ScriptMessageHandlingException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected ScriptMessageHandlingException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
