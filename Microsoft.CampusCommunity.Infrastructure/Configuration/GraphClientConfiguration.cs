namespace Microsoft.CampusCommunity.Infrastructure.Configuration
{
    /// <summary>
    /// Configuration Section class for the graph client configuration
    /// </summary>
    public class GraphClientConfiguration
    {
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string TenantId { get; set; }
        public string Authority => $"https://login.microsoftonline.com/{TenantId}/v2.0";
    }
}