using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.CampusCommunity.Api.Authorization
{
    internal static class AuthorizedPermissions
    {
        /// <summary>
        /// Contains the allowed delegated permissions for each action.
        /// If the caller has one of the allowed ones, they should be allowed
        /// to perform the action.
        /// </summary>
        public static IReadOnlyDictionary<string, string[]> DelegatedPermissionsForActions = new Dictionary<string, string[]>
        {
            [Actions.ReadUsers] = new[] { DelegatedPermissions.ReadUsers },
            [Actions.WriteUsers] = new[] { DelegatedPermissions.WriteUsers }
        };

        /// <summary>
        /// Contains the allowed application permissions for each action.
        /// If the caller has one of the allowed ones, they should be allowed
        /// to perform the action.
        /// </summary>
        public static IReadOnlyDictionary<string, string[]> ApplicationPermissionsForActions = new Dictionary<string, string[]>
        {
            [Actions.ReadUsers] = new[] { ApplicationPermissions.ReadAllUsers },
            [Actions.WriteUsers] = new[] { ApplicationPermissions.WriteAllUsers }
        };
    }
}
