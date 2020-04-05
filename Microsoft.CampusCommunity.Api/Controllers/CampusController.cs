using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CampusCommunity.Infrastructure.Configuration;
using Microsoft.CampusCommunity.Infrastructure.Entities.Dto;
using Microsoft.CampusCommunity.Infrastructure.Enums;
using Microsoft.CampusCommunity.Infrastructure.Exceptions;
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
        private readonly AuthorizationConfiguration _authConfig;

        /// <summary>
        ///     Default constructor
        /// </summary>
        /// <param name="authConfig"></param>
        /// <param name="service"></param>
        public CampusController(AuthorizationConfiguration authConfig, ICampusControllerService service)
        {
            _authConfig = authConfig;
            _service = service;
        }

        /// <summary>
        ///     Get all campus.
        ///     Requirement: GermanLeads
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
        ///     Requirement: HubLeads (Hub Lead need to be hub lead of hub)
        /// </summary>
        /// <param name="hubId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("{hubId}/campus")]
        [Authorize(Policy = PolicyNames.HubLeads)]
        public Task<IEnumerable<Campus>> GetAllCampusForHub(
            [FromRoute] Guid hubId
        )
        {
            User.ConfirmGroupMembership(hubId, _authConfig.HubLeadsAccessGroup);
            return _service.GetAllCampusForHub(hubId);
        }

        /// <summary>
        ///     Get my campus
        ///     Requirement: All
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
        ///     Requirement: Community (members and campus leads can only get their campus. Hub Leads can get their campus (plural))
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("{hubId}/campus/{campusId}")]
        [Authorize(Policy = PolicyNames.Community)]
        public Task<Campus> GetById(Guid hubId, Guid campusId)
        {
            User.ConfirmGroupMembership(campusId, _authConfig.HubLeadsGroupId);
            return _service.GetById(campusId, User);
        }

        /// <summary>
        ///     Get all users for a campus
        ///     Requirement: Community (members and campus leads can only get their campus. Hub Leads can get their campus)
        /// </summary>
        /// <param name="hubId">Id of the hub of the campus</param>
        /// <param name="campusId">Id of the campus</param>
        /// <param name="scope"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("{hubId}/campus/{campusId}/users")]
        [Authorize(Policy = PolicyNames.Community)]
        public Task<IEnumerable<BasicUser>> GetUsers(
            [FromRoute] Guid hubId,
            [FromRoute] Guid campusId,
            [FromQuery(Name = "scope")] UserScope scope = UserScope.Basic
        )
        {
            User.ConfirmGroupMembership(campusId, _authConfig.HubLeadsGroupId);
            return _service.GetUsers(campusId, User, scope);
        }

        /// <summary>
        ///     Create a new campus
        ///     Requirement: GermanLeads
        /// </summary>
        /// <param name="hubId"></param>
        /// <param name="campus"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Policy = PolicyNames.GermanLeads)]
        [Route("{hubId}/campus")]
        public Task<Campus> CreateCampus(
            [FromRoute] Guid hubId,
            [FromBody] Campus campus
        )
        {
            return _service.CreateCampus(User, hubId, campus, ModelState.IsValid);
        }

        /// <summary>
        ///     Change campus lead for a hub
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
            return _service.DefineCampusLead(User, campusId, newLeadId);
        }

        /// <summary>
        ///     Delete campus
        ///     Requirement: GermanLeads
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