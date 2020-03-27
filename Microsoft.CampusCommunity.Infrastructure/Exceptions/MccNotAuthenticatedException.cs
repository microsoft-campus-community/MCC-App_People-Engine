using System;
using System.Runtime.Serialization;

namespace Microsoft.CampusCommunity.Infrastructure.Exceptions
{
    public class MccNotAuthenticatedException : MccExceptionBase
    {
        public MccNotAuthenticatedException()
        {
        }

        protected MccNotAuthenticatedException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public MccNotAuthenticatedException(string message) : base(message)
        {
        }

        public MccNotAuthenticatedException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}