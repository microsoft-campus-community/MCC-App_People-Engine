using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.CampusCommunity.Infrastructure.Helpers;
using Microsoft.CampusCommunity.Infrastructure.Interfaces;

namespace Microsoft.CampusCommunity.Api.Authorization
{   
    /// <summary>
    /// Dynamic Policy Handler that decides whether or not a user has the correct group membership to access a resource
    /// </summary>
    public class GroupMembershipPolicyHandler : AuthorizationHandler<GroupMembershipRequirement>
    {
        private readonly IGraphGroupService _graphService;

        public GroupMembershipPolicyHandler(IGraphGroupService graphService)
        {
            _graphService = graphService;
        }

        /// <summary>
        /// Implementation of AuthorizationHandler. Sets context.Succeed in case of met group requirement
        /// </summary>
        /// <param name="context"></param>
        /// <param name="requirement"></param>
        /// <returns></returns>
        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context,
            GroupMembershipRequirement requirement)
        {
            // give full permissions if daemon app role
            if (context.User.HasDaemonAppRole())
            {
                context.Succeed(requirement);
                return;
            }

            IList<Guid> groups;
            try
            {
                groups = AuthenticationHelper.GetAaDGroups(context.User).ToList();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return;
            }
            // if the user is part of 6 or more groups, they won't be encoded in the token anymore. In this case we need to query the graph directly to get group memberships
            // TODO: check for claim hasGroups = true -> this will tell us that the user has groups but they are not part of the token
            if (groups.Count == 0)
            {
                var mccGroups = await _graphService.UserMemberOf(AuthenticationHelper.GetUserIdFromToken(context.User));
                groups = mccGroups.Select(g => g.Id).ToList();

                // add those groups to the user claims so that other group auth tests can be performed
                var claims = mccGroups.Select(g => new Claim("groups", g.Id.ToString()));
                context.User.AddIdentity(new ClaimsIdentity(claims));
            }

            // does the user have at least one of the necessary group memberships?
            var matches = groups.Intersect(requirement.GroupMemberships).Count();
            if (matches > 0) context.Succeed(requirement);
        }
    }
}