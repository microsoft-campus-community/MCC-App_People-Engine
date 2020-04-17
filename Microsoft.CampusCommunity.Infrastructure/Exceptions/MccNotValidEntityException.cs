using System;
using System.Runtime.Serialization;

namespace Microsoft.CampusCommunity.Infrastructure.Exceptions
{
    public class MccNotValidEntityException : MccExceptionBase
    {
        /// <inheritdoc />
        public MccNotValidEntityException()
        {
        }

        /// <inheritdoc />
        protected MccNotValidEntityException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        /// <inheritdoc />
        public MccNotValidEntityException(string message) : base(message)
        {
        }

        /// <inheritdoc />
        public MccNotValidEntityException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}