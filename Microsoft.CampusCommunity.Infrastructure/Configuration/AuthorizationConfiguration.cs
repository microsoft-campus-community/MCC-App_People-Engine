using System;

namespace Microsoft.CampusCommunity.Infrastructure.Configuration
{
    /// <summary>
    /// Config Section class for group authorization
    /// </summary>
    public class AuthorizationConfiguration
    {
        public AuthorizationConfiguration(string allCompanyGroupId, string campusLeadsGroupId,
            string germanLeadsGroupId, string hubLeadsGroupId, string internalDevelopmentGroupId)
        {
            CommunityGroupId = Guid.Parse(allCompanyGroupId);
            CampusLeadsGroupId = Guid.Parse(campusLeadsGroupId);
            GermanLeadsGroupId = Guid.Parse(germanLeadsGroupId);
            HubLeadsGroupId = Guid.Parse(hubLeadsGroupId);
            InternalDevelopmentGroupId = Guid.Parse(internalDevelopmentGroupId);
        }

        public Guid CommunityGroupId { get; set; }
        public Guid CampusLeadsGroupId { get; set; }
        public Guid GermanLeadsGroupId { get; set; }
        public Guid HubLeadsGroupId { get; set; }
        public Guid InternalDevelopmentGroupId { get; set; }

        public Guid[] CampusLeadsAccessGroup
        {
            get
            {
                return new Guid[]
                {
                    GermanLeadsGroupId,
                    HubLeadsGroupId,
                    InternalDevelopmentGroupId
                };
            }
        }

        public Guid[] HubLeadsAccessGroup
        {
            get
            {
                return new Guid[]
                {
                    GermanLeadsGroupId,
                    InternalDevelopmentGroupId
                };
            }
        }
    }
}