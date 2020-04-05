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
        private readonly IGraphCampusService _graphService;
        private readonly IGraphUserService _graphUserService;
        private readonly AuthorizationConfiguration _authConfig;

        /// <summary>
        ///     Default constructor
        /// </summary>
        /// <param name="graphService"></param>
        /// <param name="authConfig"></param>
        /// <param name="graphUserService"></param>
        public CampusController(IGraphCampusService graphService, AuthorizationConfiguration authConfig,
            IGraphUserService graphUserService)
        {
            _graphService = graphService;
            _authConfig = authConfig;
            _graphUserService = graphUserService;
        }

        /// <summary>
        ///     Get all campus.
        ///     Requirement: CampusLeads
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("campus")]
        [Authorize(Policy = PolicyNames.GermanLeads)]
        public Task<IEnumerable<Campus>> GetAll()
        {
            return _graphService.GetAllCampus();
        }

        /// <summary>
        ///     Get all campus for a specific hub
        ///     Requirement: HubLeads
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
            return _graphService.GetAllCampusForHub(hubId);
        }

        /// <summary>
        ///     Get my campus
        ///     Requirement: All
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("campus/my")]
        [Authorize(Policy = PolicyNames.Community)]
        public Task<Hub> GetMyHub()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        ///     Get campus by id
        ///     Requirement: CampusLeads
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("{hubId}/campus/{campusId}")]
        [Authorize(Policy = PolicyNames.CampusLeads)]
        public Task<Hub> GetMyHub(Guid hubId, Guid campusId)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        ///     Get all users for a campus
        ///     Requirement: CampusLeads
        /// </summary>
        /// <param name="hubId">Id of the hub of the campus</param>
        /// <param name="campusId">Id of the campus</param>
        /// <param name="scope"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("{hubId}/campus/{campusId}/users")]
        [Authorize(Policy = PolicyNames.CampusLeads)]
        public Task<IEnumerable<BasicUser>> GetUsers(
            [FromRoute] Guid hubId,
            [FromRoute] Guid campusId,
            [FromQuery(Name = "scope")] UserScope scope = UserScope.Basic
        )
        {
            // Authorize Campus
            User.ConfirmGroupMembership(campusId, _authConfig.CampusLeadsAccessGroup);
            return _graphService.GetCampusUsers(campusId);
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
            if (!ModelState.IsValid) throw new MccBadRequestException();
            return _graphService.CreateCampus(campus);
        }

        /// <summary>
        ///     Change campus lead for a hub
        /// </summary>
        /// <param name="campusId"></param>
        /// <param name="hubId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("{hubId}/campus/{campusId}/lead")]
        [Authorize(Policy = PolicyNames.HubLeads)]
        public Task DefineCampusLead(
            [FromRoute] Guid campusId,
            [FromRoute] Guid hubId,
            [FromQuery] Guid userId
        )
        {
            return _graphUserService.DefineCampusLead(userId, campusId);
        }

        /// <summary>
        ///     Delete campus
        ///     Requirement: GermanLeads
        /// </summary>
        /// <returns></returns>
        [HttpDelete]
        [Route("{hubId}/campus/{campusId}")]
        [Authorize(Policy = PolicyNames.GermanLeads)]
        public Task Delete([FromRoute] Guid hubId, Guid campusId)
        {
            throw new NotImplementedException();
        }
    }
}