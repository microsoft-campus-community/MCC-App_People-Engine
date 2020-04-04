using System;

namespace Microsoft.CampusCommunity.Infrastructure.Configuration
{
    /// <summary>
    /// Config Section class for Azure Active Directory authentication
    /// </summary>
    public class AadAuthenticationConfiguration
    {
        public string Instance { get; set; }
        public string Domain { get; set; }
        public string DomainProtocol { get; set; }
        public string TenantId { get; set; }
        public string ClientId { get; set; }

        public string AuthorizationUrl => $"https://login.microsoftonline.com/{TenantId}/oauth2/v2.0/authorize";

        public string Authority => $"https://login.microsoftonline.com/{TenantId}/v2.0";
        public string ApplicationIdUri => $"{DomainProtocol}://{Domain}/Api"; //https://campus-community.org/Api || api://4e8978e9-8303-4e00-b180-0e96fc1fe7dc/Api

        /// <summary>
        /// Checks if the configurations contains any values
        /// </summary>
        /// <returns></returns>
        public bool IsValid()
        {
            var hasInstance = !string.IsNullOrWhiteSpace(Instance);
            var hasDomain = !string.IsNullOrWhiteSpace(Domain);
            var hasTenantId = !string.IsNullOrWhiteSpace(TenantId);
            var hasClientId = !string.IsNullOrWhiteSpace(ClientId);

            return hasInstance && hasDomain && hasTenantId && hasClientId;
        }

        public override string ToString()
        {
            return $"Instance: {Instance} - Domain: {Domain} - TenantId: {TenantId} - ClientId: {ClientId}";
        }
    }
}