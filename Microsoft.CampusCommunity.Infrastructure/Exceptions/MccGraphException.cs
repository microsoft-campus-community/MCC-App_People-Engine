using System;
using System.Runtime.Serialization;

namespace Microsoft.CampusCommunity.Infrastructure.Exceptions
{
    public class MccGraphException : MccExceptionBase
    {
        public MccGraphException()
        {
        }

        protected MccGraphException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public MccGraphException(string message) : base(message)
        {
        }

        public MccGraphException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}