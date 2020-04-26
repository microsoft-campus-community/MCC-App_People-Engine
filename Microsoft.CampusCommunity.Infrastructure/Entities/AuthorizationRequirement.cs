using System;

namespace Microsoft.CampusCommunity.Infrastructure.Entities
{
    public class AuthorizationRequirement
    {
        public AuthorizationRequirementType Type { get; }
        public Guid Id { get; }

        public AuthorizationRequirement(AuthorizationRequirementType type, Guid id)
        {
            Type = type;
            Id = id;
        }

        public AuthorizationRequirement(AuthorizationRequirementType type)
        {
            Type = type;
            Id = Guid.Empty;
        }

        public override string ToString()
        {
            return $"{Type:g}: {Id}";
        }
    }
}