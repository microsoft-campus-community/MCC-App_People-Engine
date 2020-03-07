using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

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
