namespace Microsoft.CampusCommunity.Infrastructure.Configuration
{
    public class AadAuthenticationConfiguration
    {
        public string Instance { get; set; }
        public string Domain { get; set; }
        public string TenantId { get; set; }
        public string ClientId { get; set; }

        public string AuthorizationUrl => $"https://login.microsoftonline.com/{TenantId}/oauth2/v2.0/authorize";

        public string Authority => $"https://login.microsoftonline.com/{TenantId}/v2.0";
        public string ApplicationIdUri => $"https://{Domain}/Api"; //https://campus-community.org/Api

		public bool IsValid() {
			var hasInstance = !string.IsNullOrWhiteSpace(Instance); 
			var hasDomain = !string.IsNullOrWhiteSpace(Domain); 
			var hasTenantId = !string.IsNullOrWhiteSpace(TenantId); 
			var hasClientId = !string.IsNullOrWhiteSpace(ClientId); 

			return hasInstance && hasDomain && hasTenantId && hasClientId;
		}

		public override string ToString() {
			return $"Instance: {Instance} - Domain: {Domain} - TenantId: {TenantId} - ClientId: {ClientId}";
		}
    }
}
