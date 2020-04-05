using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.CampusCommunity.Infrastructure.Entities.Dto;
using Microsoft.CampusCommunity.Infrastructure.Exceptions;
using Microsoft.CampusCommunity.Infrastructure.Interfaces;
using Campus = Microsoft.CampusCommunity.Infrastructure.Entities.Db.Campus;

namespace Microsoft.CampusCommunity.Services.Controller
{
    public class HubControllerService : IHubControllerService
    {
        private readonly IDbService<Infrastructure.Entities.Db.Hub> _hubDbService;
        private readonly IDbService<Campus> _campusDbService;
        private readonly IGraphGroupService _graphGroupService;
        private readonly IGraphUserService _graphUserService;

        public HubControllerService(IDbService<Infrastructure.Entities.Db.Hub> hubDbService, IDbService<Campus> campusDbService, IGraphGroupService graphGroupService, IGraphUserService graphUserService)
        {
            _hubDbService = hubDbService;
            _campusDbService = campusDbService;
            _graphGroupService = graphGroupService;
            _graphUserService = graphUserService;
        }

        #region Implementation of IHubControllerService

        /// <inheritdoc />
        public async Task<IEnumerable<Hub>> GetAll()
        {
            var dbHubs = await _hubDbService.GetAll();
            return dbHubs.Select(Hub.FromDb);
        }

        /// <inheritdoc />
        public async Task<Hub> GetMyHub(Guid userId)
        {
            var campusId = await _graphUserService.GetCampusIdForUser(userId);
            if (campusId == Guid.Empty)
                throw new MccNotFoundException($"Could not find campus for user with id {userId}");

            var campus = await _campusDbService.GetById(campusId);
            return Hub.FromDb(campus.Hub);
        }

        /// <inheritdoc />
        public async Task<Hub> GetHubById(Guid userId, bool isCampusLead, Guid id)
        {
            // Perform additional checks if user is campus lead
            if (isCampusLead)
            {
                var hub = await GetMyHub(userId);
                if (hub.Id == id)
                    return hub;

                // campus lead requested other hub
                throw new MccNotAuthorizedException($"User {userId} is only authorized to access hub {hub.Name}.");
            }

            return Hub.FromDb(await _hubDbService.GetById(id));
        }

        /// <inheritdoc />
        public async Task<Hub> Create(Guid userId, Hub entity, bool modelState)
        {
            // create aad group
            var hubGroup = await _graphGroupService.CreateGroup(entity.Name, userId);

            var newHub = new Infrastructure.Entities.Db.Hub(entity.Name, entity.Lead, hubGroup.Id, userId);
            return Hub.FromDb(await _hubDbService.Create(newHub, modelState));
        }

        /// <inheritdoc />
        public async Task<Hub> Update(Guid userId, Hub entity, bool modelState)
        {
            var hub = await _hubDbService.GetById(entity.Id);

            // update
            hub.Name = entity.Name;
            hub.ModifiedBy = userId;
            await _hubDbService.Update(hub);
            return Hub.FromDb(hub);
        }

        /// <inheritdoc />
        public async Task<Hub> ChangeHubLead(Guid userId, Guid id, Guid newLead)
        {
            var hub = await _hubDbService.GetById(id);
            await _graphGroupService.ChangeGroupOwner(hub.AadGroupId, newLead);

            hub.Lead = newLead;
            hub.ModifiedBy = userId;

            await _hubDbService.Update(hub);

            // change managers
            var campusLeads = hub.Campus.Select(c => c.Lead);
            await _graphUserService.DefineHubLead(newLead, campusLeads, hub.Id);

            return Hub.FromDb(hub);
        }

        /// <inheritdoc />
        public Task Delete(Guid id)
        {
            return _hubDbService.Delete(id);
        }

        #endregion
    }
}