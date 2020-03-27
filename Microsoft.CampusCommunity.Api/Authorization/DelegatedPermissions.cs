using System.Linq;

namespace Microsoft.CampusCommunity.Api.Authorization
{
    internal static class DelegatedPermissions
    {
        public const string ReadUsers = "Default.ReadWrite";

        public static string[] All => typeof(DelegatedPermissions)
            .GetFields()
            .Where(f => f.Name != nameof(All))
            .Select(f => f.GetValue(null) as string)
            .ToArray();
    }
}