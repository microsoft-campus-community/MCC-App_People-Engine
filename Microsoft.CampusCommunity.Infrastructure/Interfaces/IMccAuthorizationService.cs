using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.CampusCommunity.Infrastructure.Entities;

namespace Microsoft.CampusCommunity.Infrastructure.Interfaces
{
    public interface IMccAuthorizationService
    {
        /// <summary>
        /// Checks if a user is allowed to access a certain endpoint.
        /// If the user is not allowed to method will throw an exception. If nothing happens, the user can perform this operation
        /// </summary>
        /// <param name="requirement"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        public Task CheckAuthorizationRequirement(ClaimsPrincipal user, AuthorizationRequirement requirement);
        public Task CheckAuthorizationRequirement(ClaimsPrincipal user, IEnumerable<AuthorizationRequirement> requirements);

    }
}