namespace Microsoft.CampusCommunity.Infrastructure.Entities
{
    public enum AuthorizationRequirementType
    {
        GeneralGroupMembership,
        IsCampusMember,
        IsGermanLead,
        IsHubLeadForCampus,
        IsHubLeadForHub,
        IsGeneralHubLead,
        IsGeneralCampusLead,
        IsCampusLeadForCampus,
        IsCampusLeadForHub,
        IsCampusLeadForUser,
        IsHubLeadForUser,
        OwnUser,
        None
    }
}