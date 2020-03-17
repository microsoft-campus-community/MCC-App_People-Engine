using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Microsoft.CampusCommunity.Infrastructure.Exceptions
{
    public class MccBadRequestException : MccExceptionBase
    {
        public MccBadRequestException()
        {
        }

        protected MccBadRequestException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public MccBadRequestException(string message) : base(message)
        {
        }

        public MccBadRequestException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
