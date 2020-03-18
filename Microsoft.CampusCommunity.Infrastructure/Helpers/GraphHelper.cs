using System.Collections.Generic;
using System.Linq;
using Microsoft.CampusCommunity.Infrastructure.Entities.Dto;
using Microsoft.Graph;

namespace Microsoft.CampusCommunity.Infrastructure {
	public static class GraphHelper {
		public static IEnumerable<BasicUser> MapBasicUsers(IEnumerable<User> users)
        {
            return users.Select(BasicUser.FromGraphUser).ToList();
        }
	}
}