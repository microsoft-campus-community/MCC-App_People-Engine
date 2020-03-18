using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using Microsoft.CampusCommunity.Infrastructure.Exceptions;

namespace Microsoft.CampusCommunity.Infrastructure.Helpers
{
    public static class AuthorizationHelper
    {
        public static void ConfirmGroupMembership(this ClaimsPrincipal user, Guid groupId, IEnumerable<Guid> except)
        {
            var groupIds = AuthenticationHelper.GetAaDGroups(user);

			// if the user is a member of the "excepts" group, he doesn't need to be a member of the "groupId" group
			// the reason for this is that someone from the development group or the german lead group doesn't need to
			// be a member of every campus group for instance
			if(except != null && groupIds.Intersect(except).Count() > 0) {
				return;
			}

			if(!groupIds.Contains(groupId)){
				// user not authorized
				var userId = AuthenticationHelper.GetUserIdFromToken(user);
				throw new MccNotAuthenticatedException($"User {userId} is not authorized to access group {groupId}");
			}
        }

		public static void ConfirmGroupMembership(this ClaimsPrincipal user, Guid groupId, Guid exceptGroup)
        {
            user.ConfirmGroupMembership(groupId, new[]{exceptGroup});
        }
    }
}
