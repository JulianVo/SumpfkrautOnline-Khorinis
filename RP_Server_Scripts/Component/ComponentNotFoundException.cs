﻿using System;
using System.Runtime.Serialization;

namespace RP_Server_Scripts.Component
{
    public class ComponentNotFoundException:Exception
    {
        public ComponentNotFoundException()
        {
        }

        public ComponentNotFoundException(string message) : base(message)
        {
        }

        public ComponentNotFoundException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected ComponentNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
