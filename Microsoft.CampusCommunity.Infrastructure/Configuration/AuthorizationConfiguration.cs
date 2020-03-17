using System;

namespace Microsoft.CampusCommunity.Infrastructure.Configuration
{
    public class AuthorizationConfiguration
    {
        public AuthorizationConfiguration(string allCompanyGroupId, string campusLeadsGroupId, string germanLeadsGroupId, string hubLeadsGroupId, string internalDevelopmentGroupId)
        {
            AllCompanyGroupId = Guid.Parse(allCompanyGroupId);
            CampusLeadsGroupId = Guid.Parse(campusLeadsGroupId);
            GermanLeadsGroupId = Guid.Parse(germanLeadsGroupId);
            HubLeadsGroupId = Guid.Parse(hubLeadsGroupId);
            InternalDevelopmentGroupId = Guid.Parse(internalDevelopmentGroupId);
        }

        public Guid AllCompanyGroupId { get; set; }
        public Guid CampusLeadsGroupId { get; set; }
        public Guid GermanLeadsGroupId { get; set; }
        public Guid HubLeadsGroupId { get; set; }
        public Guid InternalDevelopmentGroupId { get; set; }
    }
}
