using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.CampusCommunity.Infrastructure.Configuration;
using Microsoft.CampusCommunity.Infrastructure.Entities.Dto;
using Microsoft.CampusCommunity.Infrastructure.Enums;
using Microsoft.CampusCommunity.Infrastructure.Exceptions;
using Microsoft.CampusCommunity.Infrastructure.Helpers;
using Microsoft.CampusCommunity.Infrastructure.Interfaces;
using Hub = Microsoft.CampusCommunity.Infrastructure.Entities.Db.Hub;

namespace Microsoft.CampusCommunity.Services.Controller
{
    public class CampusControllerService: ICampusControllerService
    {
        private readonly IGraphUserService _graphUserService;
        private readonly IDbService<Infrastructure.Entities.Db.Campus> _campusDbService;
        private readonly IDbService<Hub> _hubDbService;
        private readonly IHubControllerService _hubControllerService;
        private readonly AuthorizationConfiguration _authorizationConfiguration;
        private readonly IGraphGroupService _graphGroupService;


        #region Implementation of ICampusControllerService

        /// <inheritdoc />
        public async Task<IEnumerable<Campus>> GetAll()
        {
            var campus = await _campusDbService.GetAll();
            return campus.Select(Campus.FromDb);
        }

        /// <inheritdoc />
        public async Task<IEnumerable<Campus>> GetAllCampusForHub(Guid hubId)
        {
            var hub = await _hubDbService.GetById(hubId);
            if (hub == null)
            {
                throw new MccNotFoundException($"Could not find hub with id {hubId}");
            }

            return hub.Campus.Select(Campus.FromDb);
        }

        /// <inheritdoc />
        public async Task<Campus> GetMyCampus(Guid userId)
        {
            var campusId = await _graphUserService.GetCampusIdForUser(userId);
            if (campusId == Guid.Empty)
                throw new MccNotFoundException($"Could not find campus for user with id {userId}");

            var campus = await _campusDbService.GetById(campusId);
            return Campus.FromDb(campus);
        }

        /// <inheritdoc />
        public async Task<Campus> GetById(Guid campusId, ClaimsPrincipal user)
        {
            var campus = await _campusDbService.GetById(campusId);
            AuthorizeHubLeadForCampus(campus, user);

            return Campus.FromDb(campus);
        }

        /// <inheritdoc />
        public async Task<IEnumerable<BasicUser>> GetUsers(Guid campusId, ClaimsPrincipal user, UserScope scope)
        {
            var campus = await _campusDbService.GetById(campusId);
            AuthorizeHubLeadForCampus(campus, user);

            var users = await _graphGroupService.GetGroupMembers(campus.AadGroupId);
            return users.Select(BasicUser.FromGraphUser);
        }

        /// <inheritdoc />
        public async Task<Campus> CreateCampus(ClaimsPrincipal user, Guid hubId, Campus campus, bool modelState)
        {
            var userId = AuthenticationHelper.GetUserIdFromToken(user);
            var campusGroup = await _graphGroupService.CreateGroup(campus.Name, campus.Lead);
            var newCampus = new Infrastructure.Entities.Db.Campus(campus.Name, campus.Lead, campusGroup.Id,
                campus.University, userId);
            return Campus.FromDb(await _campusDbService.Create(newCampus, modelState));
        }

        /// <inheritdoc />
        public async Task DefineCampusLead(ClaimsPrincipal user, Guid campusId, Guid newLeadId)
        {
            var campus = await _campusDbService.GetById(campusId);
            await _graphGroupService.ChangeGroupOwner(campus.AadGroupId, newLeadId);

            campus.Lead = newLeadId;
            campus.ModifiedBy = AuthenticationHelper.GetUserIdFromToken(user);

            await _campusDbService.Update(campus);

            // change managers
            await _graphUserService.DefineCampusLead(newLeadId, campus.Id);
        }

        /// <inheritdoc />
        public Task Delete(Guid campusId)
        {
            return _campusDbService.Delete(campusId);
        }

        #endregion

        /// <summary>
        /// This method throws an exception if the user is a hub lead but not for the <see cref="Campus"/>.
        /// </summary>
        /// <param name="campus"></param>
        /// <param name="user"></param>
        private void AuthorizeHubLeadForCampus(Infrastructure.Entities.Db.Campus campus, ClaimsPrincipal user)
        {
            // for all other users this check is already performed
            if (user.IsHubLead(_authorizationConfiguration))
            {
                // check if the campus the hub lead is checking for belongs to their hub
                var hub = campus.Hub;
                if (!user.HasGroupId(hub.AadGroupId))
                {
                    throw new MccNotAuthorizedException($"user {AuthenticationHelper.GetUserIdFromToken(user)} is not authorized to access campus with id {campus.Id}. He is only authorized to access campus within hub {hub.Name}.");
                }
            }
        }
    }
}