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
    [Authorize]
    [ApiController]
    [Route("api/hubs")]
    public class CampusController : ControllerBase
    {
        private readonly IGraphCampusService _graphService;
		private readonly IGraphUserService _graphUserService;
		private readonly AuthorizationConfiguration _authConfig;

        public CampusController(IGraphCampusService graphService, AuthorizationConfiguration authConfig)
        {
            _graphService = graphService;
			_authConfig = authConfig;
        }

        [HttpGet]
		[Route("campus")]
        [Authorize(Policy = PolicyNames.GermanLeads)]
        public Task<IEnumerable<Campus>> GetAll()
        {
            return _graphService.GetAllCampus();
        }

		[HttpGet]
		[Route("{hubId}/campus")]
        [Authorize(Policy = PolicyNames.HubLeads)]
        public Task<IEnumerable<Campus>> GetAllCampusForHub(
			[FromRoute] Guid HubId
		)
        {
            return _graphService.GetAllCampusForhub(HubId);
        }

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
			User.ConfirmGroupMembership(campusId, new[]{_authConfig.GermanLeadsGroupId, _authConfig.InternalDevelopmentGroupId});
			return _graphService.GetCampusUsers(campusId);
		}

		[HttpPost]
		[Authorize(Policy=PolicyNames.GermanLeads)]
		[Route("{hubId}/campus")]
		public Task<Campus> CreateCampus(
			[FromRoute] Guid hubId,
			[FromBody] Campus campus
		) 
		{
			if (!ModelState.IsValid) {
				throw new MccBadRequestException();
			}
			return _graphService.CreateCampus(campus);
		}

		[HttpPut]
		[Route("{hubId}/campus/{campusId}/lead")]
		[Authorize(Policy=PolicyNames.HubLeads)]
		public Task DefineCampusLead(
			[FromRoute] Guid campusId,
			[FromRoute] Guid hubId,
			[FromQuery] Guid userId
		) 
		{
			return _graphUserService.DefineCampusLead(userId, campusId);
		}
    }
}