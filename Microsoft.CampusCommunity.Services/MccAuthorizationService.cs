using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.CampusCommunity.Infrastructure.Configuration;
using Microsoft.CampusCommunity.Infrastructure.Entities;
using Microsoft.CampusCommunity.Infrastructure.Exceptions;
using Microsoft.CampusCommunity.Infrastructure.Helpers;
using Microsoft.CampusCommunity.Infrastructure.Interfaces;
using Campus = Microsoft.CampusCommunity.Infrastructure.Entities.Db.Campus;

namespace Microsoft.CampusCommunity.Services
{
    /// <summary>
    /// Primary service to handle authorization in controller logic
    /// </summary>
    public class MccAuthorizationService : IMccAuthorizationService
    {
        private readonly AuthorizationConfiguration _authorizationConfiguration;
        private readonly IDbService<Infrastructure.Entities.Db.Campus> _campusDbService;

        public MccAuthorizationService(AuthorizationConfiguration authorizationConfiguration, IDbService<Campus> campusDbService)
        {
            _authorizationConfiguration = authorizationConfiguration;
            _campusDbService = campusDbService;
        }


        public async Task CheckAuthorizationRequirement(ClaimsPrincipal user, AuthorizationRequirement requirement)
        {
            var requirementMet = await IsAuthorizedForRequirement(requirement, user);
            if (!requirementMet)
            {
                var userId = AuthenticationHelper.GetUserIdFromToken(user);
                throw new MccNotAuthorizedException(
                    $"User {userId} does not meet requirement with type {requirement.Type:g} and Id requirement {requirement.Id}.");
            }
        }

        public async Task CheckAuthorizationRequirement(ClaimsPrincipal user, IEnumerable<AuthorizationRequirement> requirements)
        {
            var requirementsArray = requirements.ToArray();
            foreach (var requirement in requirementsArray)
            {
                var requirementMet = await IsAuthorizedForRequirement(requirement, user);
                
                // we only need one requirement that is met
                if (requirementMet)
                    return;
            }

            // if no requirement is met that means that the user is not authorized.
            throw new MccNotAuthorizedException(requirementsArray);
        }


        private async Task<bool> IsAuthorizedForRequirement(AuthorizationRequirement requirement, ClaimsPrincipal user)
        {
            switch (requirement.Type)
            {
                case AuthorizationRequirementType.None:
                    return true;

                case AuthorizationRequirementType.GeneralGroupMembership:
                case AuthorizationRequirementType.IsCampusMember:
                    return user.HasGroupId(requirement.Id);

                case AuthorizationRequirementType.IsGeneralCampusLead:
                    return user.IsCampusLead(_authorizationConfiguration);

                case AuthorizationRequirementType.IsGeneralHubLead:
                    return user.IsHubLead(_authorizationConfiguration);

                case AuthorizationRequirementType.IsGermanLead:
                    return user.HasGroupId(_authorizationConfiguration.GermanLeadsGroupId);

                case AuthorizationRequirementType.IsCampusLeadForCampus:
                case AuthorizationRequirementType.IsCampusLeadForHub:

                    // is Campus Lead?
                    if (!user.IsCampusLead(_authorizationConfiguration))
                        return false;

                    // user is campus lead so check if it is his campus/hub group
                    return user.HasGroupId(requirement.Id);


                case AuthorizationRequirementType.IsHubLeadForCampus:
                    // is hub lead
                    if (!user.IsHubLead(_authorizationConfiguration))
                        return false;

                    // user is hub lead so let's check if he is also a hub lead for the campus
                    var campus = await _campusDbService.GetById(requirement.Id);
                    var campusHubId = campus.Hub.Id;
                    return user.HasGroupId(campusHubId);

                case AuthorizationRequirementType.IsHubLeadForHub:
                    // is hub lead
                    if (!user.IsHubLead(_authorizationConfiguration))
                        return false;

                    // user is part of hub group
                    return user.HasGroupId(requirement.Id);

                default:
                    throw new MccBadConfigurationException($"Requirement {requirement.Type} is not valid");
            }
        }
    }
}