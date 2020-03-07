using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using Microsoft.CampusCommunity.Infrastructure.Exceptions;

namespace Microsoft.CampusCommunity.Infrastructure.Helpers
{
    public static class AuthenticationHelper
    {
        public static Guid GetUserIdFromToken(ClaimsPrincipal user)
        {
            if (user == null || !user.HasClaim(c => c.Type == "sub"))
                throw new MccNotAuthenticatedException();

            string subClaim;
            try
            {
                subClaim = user.FindFirst(c => c.Type == "sub").Value;
            }
            catch (Exception e)
            {
                throw new MccNotAuthenticatedException("Could not get sub claim from token", e);
            }

            if (string.IsNullOrWhiteSpace(subClaim)) throw new MccNotAuthenticatedException();

            if (Guid.TryParse(subClaim, out var userId)) return userId;


            throw new MccNotAuthenticatedException($"Could not parse userId: {subClaim}");
        }

        public static IEnumerable<Guid> GetAaDGroups(ClaimsPrincipal user)
        {
            if (user == null || !user.HasClaim(c => c.Type == "groups"))
                return Array.Empty<Guid>();

            IEnumerable<string> groupClaims;
            try
            {
                groupClaims = user.FindAll(c => c.Type == "groups").Select(c => c.Value);
            }
            catch (Exception e)
            {
                throw new MccNotAuthenticatedException("Could not get group claim from token", e);
            }

            // convert to guids
            var groupIds = new List<Guid>();
            foreach (var groupClaim in groupClaims)
            {
                if (Guid.TryParse(groupClaim, out var groupId))
                    groupIds.Add(groupId);
            }

            return groupIds;

        }
    }
}
