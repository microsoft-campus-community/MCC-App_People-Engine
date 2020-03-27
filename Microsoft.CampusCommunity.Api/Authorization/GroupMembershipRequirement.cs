using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;

namespace Microsoft.CampusCommunity.Api.Authorization
{
    /// <summary>
    /// Model for group membership requirement
    /// </summary>
    public class GroupMembershipRequirement : IAuthorizationRequirement
    {
        /// <summary>
        /// Needs to be member of one of these groups
        /// </summary>
        public IEnumerable<Guid> GroupMemberships { get; }

        /// <summary>
        /// Generic constructor
        /// </summary>
        /// <param name="groupMemberships"></param>
        public GroupMembershipRequirement(IEnumerable<Guid> groupMemberships)
        {
            GroupMemberships = groupMemberships;
        }
    }
}