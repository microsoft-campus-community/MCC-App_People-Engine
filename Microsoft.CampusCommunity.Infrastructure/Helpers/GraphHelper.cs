using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CampusCommunity.Infrastructure.Entities;
using Microsoft.CampusCommunity.Infrastructure.Entities.Dto;
using Microsoft.Graph;

namespace Microsoft.CampusCommunity.Infrastructure.Helpers
{
    public static class GraphHelper
    {
        public static IEnumerable<BasicUser> MapBasicUsers(IEnumerable<User> users)
        {
            return users.Select(BasicUser.FromGraphUser).ToList();
        }

        public static IEnumerable<FullUser> MapFullUsers(IEnumerable<User> users,
            AuthorizationGroupMembers groupMembers)
        {
            var result = new List<FullUser>();
            foreach (var user in users)
            {
                var fullUser = FullUser.FromGraphUser(user);
                fullUser.IsCampusLead = groupMembers.CampusLeads.Any(cl => cl == fullUser.Id);
                fullUser.IsHubLead = groupMembers.HubLeads.Any(hl => hl == fullUser.Id);
                fullUser.IsAdmin = groupMembers.Admins.Any(a => a == fullUser.Id);
                result.Add(fullUser);
            }

            return result;
        }

        public static FullUser MapFullUser(User user,
            AuthorizationGroupMembers groupMembers)
        {
            var fullUser = FullUser.FromGraphUser(user);
            fullUser.IsCampusLead = groupMembers.CampusLeads.Any(cl => cl == fullUser.Id);
            fullUser.IsHubLead = groupMembers.HubLeads.Any(hl => hl == fullUser.Id);
            fullUser.IsAdmin = groupMembers.Admins.Any(a => a == fullUser.Id);
            return fullUser;
        }

        public static string CreateMailForUser(NewUser user)
        {
            // remove everything after space
            var firstName = user.FirstName.Split(" ")[0];
            var lastName = user.LastName.Split(" ")[0];
            return $"{firstName}.{lastName}";
        }
    }
}