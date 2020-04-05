using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using Microsoft.CampusCommunity.Infrastructure.Configuration;
using Microsoft.CampusCommunity.Infrastructure.Exceptions;

namespace Microsoft.CampusCommunity.Infrastructure.Helpers
{
    public static class AuthorizationHelper
    {
        /// <summary>
        /// Make sure that the user is part of a <see cref="groupId"/>. If he is not part of this group he can also be part of the <see cref="except"/> group.
        /// </summary>
        /// <param name="user">User</param>
        /// <param name="groupId">Has to be part of group</param>
        /// <param name="except">Can also be part of one of these groups</param>
        public static void ConfirmGroupMembership(this ClaimsPrincipal user, Guid groupId, IEnumerable<Guid> except)
        {
            var groupIds = AuthenticationHelper.GetAaDGroups(user).ToList();

            // if the user is a member of the "excepts" group, he doesn't need to be a member of the "groupId" group
            // the reason for this is that someone from the development group or the german lead group doesn't need to
            // be a member of every campus group for instance
            if (except != null && groupIds.Intersect(except).Any()) return;

            if (!groupIds.Contains(groupId))
            {
                // user not authorized
                var userId = AuthenticationHelper.GetUserIdFromToken(user);
                throw new MccNotAuthenticatedException($"User {userId} is not authorized to access group {groupId}");
            }
        }

        /// <summary>
        /// Make sure that the user is part of a <see cref="groupId"/>. If he is not part of this group he can also be part of one <see cref="exceptGroup"/> group.
        /// </summary>
        /// <param name="user"></param>
        /// <param name="groupId"></param>
        /// <param name="exceptGroup"></param>
        public static void ConfirmGroupMembership(this ClaimsPrincipal user, Guid groupId, Guid exceptGroup)
        {
            user.ConfirmGroupMembership(groupId, new[] {exceptGroup});
        }

        /// <summary>
        /// Extension method to check if user is MCC member
        /// </summary>
        /// <param name="user"></param>
        /// <param name="authorizationConfiguration"></param>
        /// <returns></returns>
        public static bool IsMccMember(this ClaimsPrincipal user, AuthorizationConfiguration authorizationConfiguration)
        {
            return user.HasGroupId(authorizationConfiguration.CommunityGroupId);
        }

        /// <summary>
        /// Extension method to see if user is campus lead
        /// </summary>
        /// <param name="user"></param>
        /// <param name="authorizationConfiguration"></param>
        /// <returns></returns>
        public static bool IsCampusLead(this ClaimsPrincipal user, AuthorizationConfiguration authorizationConfiguration)
        {
            return user.HasGroupId(authorizationConfiguration.CampusLeadsGroupId);
        }

        /// <summary>
        /// Extension method to see if user is hub lead
        /// </summary>
        /// <param name="user"></param>
        /// <param name="authorizationConfiguration"></param>
        /// <returns></returns>
        public static bool IsHubLead(this ClaimsPrincipal user, AuthorizationConfiguration authorizationConfiguration)
        {
            return user.HasGroupId(authorizationConfiguration.HubLeadsGroupId);
        }

        public static bool HasGroupId(this ClaimsPrincipal user, Guid groupId)
        {
            return AuthenticationHelper.GetAaDGroups(user).Any(g => g == groupId);
        }

    }
}