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
    [Route("[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IGraphUserService _graphService;
		private readonly AuthorizationConfiguration _authConfig;

        public UsersController(IGraphUserService graphService, AuthorizationConfiguration authConfig)
        {
            _graphService = graphService;
			_authConfig = authConfig;
        }

        [HttpGet]
        [Authorize(Policy = PolicyNames.GermanLeads)]
        public Task<IEnumerable<BasicUser>> Get(
            [FromQuery(Name = "scope")] UserScope scope = UserScope.Basic)
        {
            return _graphService.GetAllUsers();
        }

		[HttpGet]
		[Authorize(Policy = PolicyNames.CampusLeads)]
		public Task<IEnumerable<BasicUser>> GetCampusUsers(
            [FromRoute] Guid campusId,
			[FromQuery(Name = "scope")] UserScope scope = UserScope.Basic
		) 
		{
			// Authorize Campus
			User.ConfirmGroupMembership(campusId, _authConfig.CampusLeadsGroupId);
			return _graphService.GetCampusUsers(campusId);
		}

		[HttpGet]
		[Authorize(Policy=PolicyNames.General)]
		public Task<BasicUser> GetCurrentUser(
			[FromQuery(Name = "scope")] UserScope scope = UserScope.Basic
		) 
		{
			return _graphService.GetCurrentUser(AuthenticationHelper.GetUserIdFromToken(User));
		}

		[HttpPost]
		[Authorize(Policy=PolicyNames.CampusLeads)]
		public Task<BasicUser> CreateUser(
			[FromRoute] Guid campusId,
			[FromBody] NewUser user
		) 
		{
			user.CampusId = campusId;
			if (!ModelState.IsValid) {
				throw new MccBadRequestException();
			}

			User.ConfirmCampusMembership(campusId, _authConfig.CampusLeadsGroupId);
			return _graphService.CreateUser(user);
		}

		[HttpPut]
		[Authorize(Policy=PolicyNames.GermanLeads)]
		public Task DefineCampusLead(
			[FromRoute] Guid campusId,
			[FromQuery] Guid userId
		) 
		{
			return _graphService.DefineCampusLead(userId, campusId);
		}
    }
}