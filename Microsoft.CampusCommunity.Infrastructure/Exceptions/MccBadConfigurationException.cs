using System;
using System.Runtime.Serialization;

namespace Microsoft.CampusCommunity.Infrastructure.Exceptions
{
    public class MccBadConfigurationException : MccExceptionBase
    {
        /// <inheritdoc />
        public MccBadConfigurationException()
        {
        }

        /// <inheritdoc />
        protected MccBadConfigurationException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        /// <inheritdoc />
        public MccBadConfigurationException(string message) : base(message)
        {
        }

        /// <inheritdoc />
        public MccBadConfigurationException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}