using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Graph;

namespace Microsoft.CampusCommunity.Infrastructure.Entities
{
    public class AuthorizationGroupMembers
    {
        public List<Guid> CampusLeads { get; }
        public List<Guid> HubLeads { get; }
        public List<Guid> Admins { get; }

        public AuthorizationGroupMembers(IEnumerable<User> campusLeads, IEnumerable<User> hubLeads, IEnumerable<User> admins)
        {
            CampusLeads = campusLeads.Select(u => Guid.Parse(u.Id)).ToList();
            HubLeads = hubLeads.Select(u => Guid.Parse(u.Id)).ToList(); ;
            Admins = admins.Select(u => Guid.Parse(u.Id)).ToList(); ;
        }
    }
}