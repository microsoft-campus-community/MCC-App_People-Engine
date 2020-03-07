using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.CampusCommunity.Api.Authorization
{
    internal static class DelegatedPermissions
    {
        public const string ReadUsers = "Users.Read";
        public const string WriteUsers = "Users.Write";

        public static string[] All => typeof(DelegatedPermissions)
            .GetFields()
            .Where(f => f.Name != nameof(All))
            .Select(f => f.GetValue(null) as string)
            .ToArray();
    }
}
