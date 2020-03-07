using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.CampusCommunity.Api.Authorization
{
    internal static class ApplicationPermissions
    {
        public const string ReadAllUsers = "Users.Read.All";
        public const string WriteAllUsers = "Users.Write.All";

        public static string[] All => typeof(ApplicationPermissions)
            .GetFields()
            .Where(f => f.Name != nameof(All))
            .Select(f => f.GetValue(null) as string)
            .ToArray();
    }
}
