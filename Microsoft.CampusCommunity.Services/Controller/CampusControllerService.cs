using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.CampusCommunity.Infrastructure.Configuration;
using Microsoft.CampusCommunity.Infrastructure.Entities.Dto;
using Microsoft.CampusCommunity.Infrastructure.Enums;
using Microsoft.CampusCommunity.Infrastructure.Exceptions;
using Microsoft.CampusCommunity.Infrastructure.Interfaces;
using Hub = Microsoft.CampusCommunity.Infrastructure.Entities.Db.Hub;

namespace Microsoft.CampusCommunity.Services.Controller
{
    public class CampusControllerService: ICampusControllerService
    {
        private readonly IGraphUserService _graphUserService;
        private readonly IDbService<Infrastructure.Entities.Db.Campus> _campusDbService;
        private readonly IDbService<Hub> _hubDbService;
        private readonly AuthorizationConfiguration _authorizationConfiguration;
        private readonly IGraphGroupService _graphGroupService;

        public CampusControllerService(IGraphUserService graphUserService, IDbService<Infrastructure.Entities.Db.Campus> campusDbService, IDbService<Hub> hubDbService, AuthorizationConfiguration authorizationConfiguration, IGraphGroupService graphGroupService)
        {
            _graphUserService = graphUserService;
            _campusDbService = campusDbService;
            _hubDbService = hubDbService;
            _authorizationConfiguration = authorizationConfiguration;
            _graphGroupService = graphGroupService;
        }


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
        public async Task<Campus> GetById(Guid campusId)
        {
            var campus = await _campusDbService.GetById(campusId);

            return Campus.FromDb(campus);
        }

        /// <inheritdoc />
        public async Task<IEnumerable<BasicUser>> GetUsers(Guid campusId, UserScope scope)
        {
            var campus = await _campusDbService.GetById(campusId);

            var users = await _graphGroupService.GetGroupMembers(campus.AadGroupId);

            if (scope == UserScope.Basic)
                return users.Select(BasicUser.FromGraphUser);

            return await _graphUserService.AddFullScope(users);
        }

        /// <inheritdoc />
        public async Task<Campus> CreateCampus(Guid userId, Guid hubId, Campus campus, bool modelState)
        {
            // find hub
            var hub = await _hubDbService.GetById(hubId);
            
            // find lead
            var lead = await _graphUserService.GetGraphUserById(campus.Lead);

            var campusGroup = await _graphGroupService.CreateGroup(campus.Name, userId, hub.AadGroupId.ToString(), true);

            // add lead to group
            await _graphGroupService.AddUserToGroup(lead, campusGroup.Id);

            // make sure lead has permissions and title
            await _graphUserService.DefineCampusLead(campus.Lead, campusGroup.Id);

            // assign manager
            await _graphUserService.AssignManager(lead, hub.Lead.ToString());


            var newCampus = new Infrastructure.Entities.Db.Campus(campus.Name, campus.Lead, campusGroup.Id,
                campus.University, userId)
            {
                Hub = hub,
                Id = campusGroup.Id
            };

            return Campus.FromDb(await _campusDbService.Create(newCampus, modelState));
        }

        /// <inheritdoc />
        public async Task DefineCampusLead(Guid userId, Guid campusId, Guid newLeadId)
        {
            var campus = await _campusDbService.GetById(campusId);
            await _graphGroupService.ChangeGroupOwner(campus.AadGroupId, newLeadId);

            campus.Lead = newLeadId;
            campus.ModifiedBy = userId;

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
    }
}