using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

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
