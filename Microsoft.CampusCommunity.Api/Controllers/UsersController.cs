using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CampusCommunity.Infrastructure.Configuration;
using Microsoft.CampusCommunity.Infrastructure.Entities;
using Microsoft.CampusCommunity.Infrastructure.Entities.Dto;
using Microsoft.CampusCommunity.Infrastructure.Enums;
using Microsoft.CampusCommunity.Infrastructure.Exceptions;
using Microsoft.CampusCommunity.Infrastructure.Helpers;
using Microsoft.CampusCommunity.Infrastructure.Interfaces;

namespace Microsoft.CampusCommunity.Api.Controllers
{
    /// <summary>
    /// Controller for newUser operations
    /// </summary>
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IUserControllerService _service;
        private readonly AuthorizationConfiguration _authConfig;
        private readonly IMccAuthorizationService _authorizationService;

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="userControllerService"></param>
        /// <param name="authConfig"></param>
        /// <param name="authorizationService"></param>
        public UsersController(IUserControllerService userControllerService, AuthorizationConfiguration authConfig, IMccAuthorizationService authorizationService)
        {
            _service = userControllerService;
            _authConfig = authConfig;
            _authorizationService = authorizationService;
        }

        /// <summary>
        /// Get all MCC users. This will only return users where the "location" tag is not empty.
        /// Requirement: <see cref="PolicyNames.GermanLeads"/>
        /// </summary>
        /// <param name="scope">User scope</param>
        /// <returns>A user</returns>
        /// <response code="200">Request successful</response>
        /// <response code="400">Product has missing/invalid values</response>
        /// <response code="500">Internal Server Error</response>
        [HttpGet]
        [Authorize(Policy = PolicyNames.GermanLeads)]
        public Task<IEnumerable<BasicUser>> Get(
            [FromQuery(Name = "scope")] UserScope scope = UserScope.Basic)
        {
            return _service.GetAll(scope);
        }

        /// <summary>
        /// Gets the current User.
        /// Requirement: <see cref="PolicyNames.Community"/>
        /// </summary>
        /// <param name="scope"></param>
        /// <returns></returns>
        [HttpGet]
        [Authorize(Policy = PolicyNames.Community)]
        [Route("current")]
        public Task<BasicUser> GetCurrentUser(
            [FromQuery(Name = "scope")] UserScope scope = UserScope.Basic
        )
        {
            return _service.GetUserById(AuthenticationHelper.GetUserIdFromToken(User), scope);
        }


        /// <summary>
        /// Gets a user by id
        /// Requirement: <see cref="PolicyNames.CampusLeads"/>
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="scope"></param>
        /// <returns></returns>
        [HttpGet]
        [Authorize(Policy = PolicyNames.CampusLeads)]
        [Route("{userId}")]
        public Task<BasicUser> GetById(
            [FromRoute] Guid userId,
            [FromQuery(Name = "scope")] UserScope scope = UserScope.Basic
        )
        {
            return _service.GetUserById(userId, scope);
        }

        /// <summary>
        /// Create a new new User
        /// Requirement: <see cref="PolicyNames.CampusLeads"/>
        /// </summary>
        /// <param name="newUser"></param>
        /// <returns></returns>
        /// <exception cref="MccBadRequestException"></exception>
        [HttpPost]
        [Authorize(Policy = PolicyNames.CampusLeads)]
        public async Task<BasicUser> CreateUser(
            [FromBody] NewUser newUser
        )
        {
            if (!ModelState.IsValid) throw new MccBadRequestException();
            var campusId = newUser.CampusId;
            if (campusId == Guid.Empty) throw new MccBadRequestException("Campus Id for new newUser is not set");

            await _authorizationService.CheckAuthorizationRequirement(User,
                new[]
                {
                    new AuthorizationRequirement(AuthorizationRequirementType.IsGermanLead, Guid.Empty),
                    new AuthorizationRequirement(AuthorizationRequirementType.IsCampusLeadForCampus, campusId),
                    new AuthorizationRequirement(AuthorizationRequirementType.IsHubLeadForCampus, campusId),
                });

            return await _service.CreateUser(newUser, campusId);
        }
    }
}