using System;
using System.Runtime.Serialization;

namespace Microsoft.CampusCommunity.Infrastructure.Exceptions
{
    public class MccExceptionBase : Exception
    {
        public MccExceptionBase()
        {
        }

        protected MccExceptionBase(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public MccExceptionBase(string message) : base(message)
        {
        }

        public MccExceptionBase(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}