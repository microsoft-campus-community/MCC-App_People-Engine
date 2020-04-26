using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CampusCommunity.Infrastructure.Configuration;
using Microsoft.CampusCommunity.Infrastructure.Entities;
using Microsoft.CampusCommunity.Infrastructure.Entities.Dto;
using Microsoft.CampusCommunity.Infrastructure.Enums;
using Microsoft.CampusCommunity.Infrastructure.Helpers;
using Microsoft.CampusCommunity.Infrastructure.Interfaces;

namespace Microsoft.CampusCommunity.Api.Controllers
{
    /// <summary>
    ///     Controller for campus operations
    /// </summary>
    [Authorize]
    [ApiController]
    [Route("api/hubs")]
    public class CampusController : ControllerBase
    {
        private readonly ICampusControllerService _service;
        private readonly IMccAuthorizationService _authorizationService;

        /// <summary>
        ///     Default constructor
        /// </summary>
        /// <param name="service"></param>
        /// <param name="authorizationService"></param>
        public CampusController(ICampusControllerService service, IMccAuthorizationService authorizationService)
        {
            _service = service;
            _authorizationService = authorizationService;
        }

        /// <summary>
        ///     Get all campus.
        ///     Requirement: <see cref="PolicyNames.GermanLeads"/>
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("campus")]
        [Authorize(Policy = PolicyNames.GermanLeads)]
        public Task<IEnumerable<Campus>> GetAll()
        {
            return _service.GetAll();
        }

        /// <summary>
        ///     Get all campus for a specific hub
        ///     Requirement: <see cref="PolicyNames.HubLeads"/> (Hub Lead need to be hub lead of hub)
        /// </summary>
        /// <param name="hubId">HubId - NOT Hub Aad Group Id</param>
        /// <returns></returns>
        [HttpGet]
        [Route("{hubId}/campus")]
        [Authorize(Policy = PolicyNames.HubLeads)]
        public async Task<IEnumerable<Campus>> GetAllCampusForHub(
            [FromRoute] Guid hubId
        )
        {
            await _authorizationService.CheckAuthorizationRequirement(User,
                new[]
                {
                    new AuthorizationRequirement(AuthorizationRequirementType.IsGermanLead, Guid.Empty),
                    new AuthorizationRequirement(AuthorizationRequirementType.IsHubLeadForHub, hubId)
                });
            return await _service.GetAllCampusForHub(hubId);
        }

        /// <summary>
        ///     Get my campus
        ///     Requirement: <see cref="PolicyNames.Community"/>
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("campus/my")]
        [Authorize(Policy = PolicyNames.Community)]
        public Task<Campus> GetMyCampus()
        {
            var userId = AuthenticationHelper.GetUserIdFromToken(User);
            return _service.GetMyCampus(userId);
        }

        /// <summary>
        ///     Get campus by id
        ///     Requirement: <see cref="PolicyNames.Community"/> (members and campus leads can only get their campus. Hub Leads can get their campus (plural))
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("{hubId}/campus/{campusId}")]
        [Authorize(Policy = PolicyNames.Community)]
        public async Task<Campus> GetById(Guid hubId, Guid campusId)
        {
            await _authorizationService.CheckAuthorizationRequirement(User,
                new[]
                {
                    new AuthorizationRequirement(AuthorizationRequirementType.IsGermanLead, Guid.Empty),
                    new AuthorizationRequirement(AuthorizationRequirementType.IsCampusLeadForCampus, campusId),
                    new AuthorizationRequirement(AuthorizationRequirementType.IsCampusMember, campusId),
                    new AuthorizationRequirement(AuthorizationRequirementType.IsHubLeadForCampus, campusId),
                });
            return await _service.GetById(campusId);
        }

        /// <summary>
        ///     Get all users for a campus
        ///     Requirement: <see cref="PolicyNames.Community"/> (members and campus leads can only get their campus. Hub Leads can get their campus)
        /// </summary>
        /// <param name="hubId">Id of the hub of the campus</param>
        /// <param name="campusId">Id of the campus</param>
        /// <param name="scope"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("{hubId}/campus/{campusId}/users")]
        [Authorize(Policy = PolicyNames.Community)]
        public async Task<IEnumerable<BasicUser>> GetUsers(
            [FromRoute] Guid hubId,
            [FromRoute] Guid campusId,
            [FromQuery(Name = "scope")] UserScope scope = UserScope.Basic
        )
        {
            await _authorizationService.CheckAuthorizationRequirement(User,
                new[]
                {
                    new AuthorizationRequirement(AuthorizationRequirementType.IsGermanLead, Guid.Empty),
                    new AuthorizationRequirement(AuthorizationRequirementType.IsCampusLeadForCampus, campusId),
                    new AuthorizationRequirement(AuthorizationRequirementType.IsCampusMember, campusId),
                    new AuthorizationRequirement(AuthorizationRequirementType.IsHubLeadForCampus, campusId),
                });
            return await _service.GetUsers(campusId, scope);
        }

        /// <summary>
        ///     Create a new campus
        ///     Requirement: <see cref="PolicyNames.HubLeads"/>
        /// </summary>
        /// <param name="hubId"></param>
        /// <param name="campus"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Policy = PolicyNames.HubLeads)]
        [Route("{hubId}/campus")]
        public async Task<Campus> CreateCampus(
            [FromRoute] Guid hubId,
            [FromBody] Campus campus
        )
        {
            await _authorizationService.CheckAuthorizationRequirement(User,
                new[]
                {
                    new AuthorizationRequirement(AuthorizationRequirementType.IsGermanLead, Guid.Empty),
                    new AuthorizationRequirement(AuthorizationRequirementType.IsHubLeadForHub, hubId),
                });

            return await _service.CreateCampus(AuthenticationHelper.GetUserIdFromToken(User), hubId, campus, ModelState.IsValid);
        }

        /// <summary>
        ///     Change campus lead for a hub
        ///     Requirement: <see cref="PolicyNames.HubLeads"/>
        /// </summary>
        /// <param name="campusId"></param>
        /// <param name="hubId"></param>
        /// <param name="newLeadId"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("{hubId}/campus/{campusId}/lead")]
        [Authorize(Policy = PolicyNames.HubLeads)]
        public Task DefineCampusLead(
            [FromRoute] Guid campusId,
            [FromRoute] Guid hubId,
            [FromQuery] Guid newLeadId
        )
        {
            return _service.DefineCampusLead(AuthenticationHelper.GetUserIdFromToken(User), campusId, newLeadId);
        }

        /// <summary>
        ///     Delete campus
        ///     Requirement: <see cref="PolicyNames.GermanLeads"/>
        /// </summary>
        /// <returns></returns>
        [HttpDelete]
        [Route("{hubId}/campus/{campusId}")]
        [Authorize(Policy = PolicyNames.GermanLeads)]
        public async Task<IActionResult> Delete([FromRoute] Guid hubId, Guid campusId)
        {
            await _service.Delete(campusId);
            return NoContent();
        }
    }
}