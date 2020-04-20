using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Microsoft.CampusCommunity.Infrastructure.Entities;

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

        public MccNotAuthorizedException(IEnumerable<AuthorizationRequirement> unmetRequirements) 
            : base($"User does not meet at least one of the following requirements: {string.Join("\n", unmetRequirements)}")
        {
        }

        public MccNotAuthorizedException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}