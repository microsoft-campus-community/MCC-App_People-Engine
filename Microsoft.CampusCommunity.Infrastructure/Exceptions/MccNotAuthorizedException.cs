using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Microsoft.CampusCommunity.Infrastructure.Exceptions
{
    public class MccNotAuthorizedException : MccExceptionBase
    {
        public MccNotAuthorizedException()
        {
        }

        protected MccNotAuthorizedException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public MccNotAuthorizedException(string message) : base(message)
        {
        }

        public MccNotAuthorizedException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
