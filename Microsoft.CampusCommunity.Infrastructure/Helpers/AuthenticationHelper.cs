using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.CampusCommunity.Infrastructure.Exceptions;

namespace Microsoft.CampusCommunity.Infrastructure.Helpers
{
    public static class AuthenticationHelper
    {
        public const string OIdClaimName = "http://schemas.microsoft.com/identity/claims/objectidentifier";

        /// <summary>
        /// Get the user id from the token claims
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public static Guid GetUserIdFromToken(ClaimsPrincipal user)
        {
            if (user == null || !user.HasClaim(c => c.Type == OIdClaimName))
                throw new MccNotAuthenticatedException();

            string subClaim;
            try
            {
                subClaim = user.FindFirst(c => c.Type == OIdClaimName).Value;
            }
            catch (Exception e)
            {
                throw new MccNotAuthenticatedException("Could not get sub claim from token", e);
            }

            if (string.IsNullOrWhiteSpace(subClaim)) throw new MccNotAuthenticatedException();

            if (Guid.TryParse(subClaim, out var userId)) return userId;


            throw new MccNotAuthenticatedException($"Could not parse userId: {subClaim}");
        }

        /// <summary>
        /// Get all aad and office groups from token
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
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
                if (Guid.TryParse(groupClaim, out var groupId))
                    groupIds.Add(groupId);

            return groupIds;
        }

        public static bool HasDaemonAppRole(this ClaimsPrincipal user)
        {
            if (user == null || !user.HasClaim(c => c.Type == ClaimTypes.Role))
                return false;

            return user.FindAll(c => c.Type == ClaimTypes.Role).Any(c => c.Value == "DaemonAppRole");
        }


        // FROM: https://www.ryadel.com/en/c-sharp-random-password-generator-asp-net-core-mvc/
        /// <summary>
        ///     Generates a Random Password
        ///     respecting the given strength requirements.
        /// </summary>
        /// <param name="opts">
        ///     A valid PasswordOptions object
        ///     containing the password strength requirements.
        /// </param>
        /// <returns>A random password</returns>
        public static string GenerateRandomPassword(PasswordOptions opts = null)
        {
            if (opts == null)
                opts = new PasswordOptions()
                {
                    RequiredLength = 8,
                    RequiredUniqueChars = 4,
                    RequireDigit = true,
                    RequireLowercase = true,
                    RequireNonAlphanumeric = true,
                    RequireUppercase = true
                };

            string[] randomChars = new[]
            {
                "ABCDEFGHJKLMNOPQRSTUVWXYZ", // uppercase 
                "abcdefghijkmnopqrstuvwxyz", // lowercase
                "0123456789", // digits
                "!@$?_-" // non-alphanumeric
            };
            Random rand = new Random(Environment.TickCount);
            List<char> chars = new List<char>();

            if (opts.RequireUppercase)
                chars.Insert(rand.Next(0, chars.Count),
                    randomChars[0][rand.Next(0, randomChars[0].Length)]);

            if (opts.RequireLowercase)
                chars.Insert(rand.Next(0, chars.Count),
                    randomChars[1][rand.Next(0, randomChars[1].Length)]);

            if (opts.RequireDigit)
                chars.Insert(rand.Next(0, chars.Count),
                    randomChars[2][rand.Next(0, randomChars[2].Length)]);

            if (opts.RequireNonAlphanumeric)
                chars.Insert(rand.Next(0, chars.Count),
                    randomChars[3][rand.Next(0, randomChars[3].Length)]);

            for (int i = chars.Count;
                i < opts.RequiredLength
                || chars.Distinct().Count() < opts.RequiredUniqueChars;
                i++)
            {
                string rcs = randomChars[rand.Next(0, randomChars.Length)];
                chars.Insert(rand.Next(0, chars.Count),
                    rcs[rand.Next(0, rcs.Length)]);
            }

            return new string(chars.ToArray());
        }
    }
}