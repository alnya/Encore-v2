﻿namespace Encore.Domain.Services.Exceptions
{
    using System;
    using System.Runtime.Serialization;
    
    [Serializable]
    public class DomainException : Exception
    {
        public DomainException()
        {
        }

        public DomainException(string message)
            : base(message)
        {
        }

        public DomainException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        public DomainException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
