using System;
using System.Runtime.Serialization;

namespace Microsoft.CampusCommunity.Infrastructure.Exceptions
{
    public class MccNotFoundException : MccExceptionBase
    {
        /// <inheritdoc />
        public MccNotFoundException()
        {
        }

        /// <inheritdoc />
        protected MccNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        /// <inheritdoc />
        public MccNotFoundException(string message) : base(message)
        {
        }

        /// <inheritdoc />
        public MccNotFoundException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}