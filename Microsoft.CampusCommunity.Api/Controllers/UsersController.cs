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
    /// Controller for user operations
    /// </summary>
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IGraphUserService _graphService;
        private readonly AuthorizationConfiguration _authConfig;

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="graphService"></param>
        /// <param name="authConfig"></param>
        public UsersController(IGraphUserService graphService, AuthorizationConfiguration authConfig)
        {
            _graphService = graphService;
            _authConfig = authConfig;
        }

        /// <summary>
        /// Get all MCC users
        /// </summary>
        /// <param name="scope"></param>
        /// <returns></returns>
        [HttpGet]
        [Authorize(Policy = PolicyNames.GermanLeads)]
        public Task<IEnumerable<BasicUser>> Get(
            [FromQuery(Name = "scope")] UserScope scope = UserScope.Basic)
        {
            return _graphService.GetAllUsers();
        }

        /// <summary>
        /// Gets the current user
        /// </summary>
        /// <param name="scope"></param>
        /// <returns></returns>
        [HttpGet]
        [Authorize(Policy = PolicyNames.General)]
        [Route("current")]
        public Task<BasicUser> GetCurrentUser(
            [FromQuery(Name = "scope")] UserScope scope = UserScope.Basic
        )
        {
            return _graphService.GetCurrentUser(AuthenticationHelper.GetUserIdFromToken(User));
        }

        /// <summary>
        /// Create a new user
        /// </summary>
        /// <param name="campusId">Campus Id of the new user to be created under</param>
        /// <param name="user"></param>
        /// <returns></returns>
        /// <exception cref="MccBadRequestException"></exception>
        [HttpPost]
        [Authorize(Policy = PolicyNames.CampusLeads)]
        public Task<BasicUser> CreateUser(
            [FromRoute] Guid campusId,
            [FromBody] NewUser user
        )
        {
            user.CampusId = campusId;
            if (!ModelState.IsValid) throw new MccBadRequestException();

            User.ConfirmGroupMembership(campusId, _authConfig.CampusLeadsGroupId);
            return _graphService.CreateUser(user, campusId);
        }
    }
}