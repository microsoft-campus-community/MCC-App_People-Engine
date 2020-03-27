using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.CampusCommunity.Infrastructure;
using Microsoft.CampusCommunity.Infrastructure.Entities.Dto;
using Microsoft.CampusCommunity.Infrastructure.Interfaces;
using Microsoft.Graph;

namespace Microsoft.CampusCommunity.Services
{
    public class GraphGroupService : IGraphGroupService, IGraphCampusService
    {
        private IGraphService _graphService;
        private IAppInsightsService _appInsightsService;

        public const string CampusGroupNamePrefix = "Campus";
        public const string HubGroupNamePrefix = "Hub";

        public GraphGroupService(IGraphService graphService, IAppInsightsService appInsightsService)
        {
            _graphService = graphService;
            _appInsightsService = appInsightsService;
        }


        //Adds the user to the given group if not already a member of
        public async Task AddUserToGroup(User user, Guid groupId)
        {
            var userGroups = await UserMemberOf(user.Id);
            if (userGroups.Any(g => g.Id == groupId))
                Console.WriteLine("User already belongs to this group");
            else
                await _graphService.Client.Groups[groupId.ToString()].Members.References.Request().AddAsync(user);
        }

        public async Task<MccGroup> GetGroupById(Guid campusId)
        {
            var group = await _graphService.Client.Groups[campusId.ToString()].Request().GetAsync();

            return new MccGroup()
            {
                Id = Guid.Parse(group.Id),
                Name = group.DisplayName
            };
        }

        public async Task<IEnumerable<User>> GetGroupMembers(Guid groupId)
        {
            var groupMembers = await _graphService.Client.Groups[groupId.ToString()].Members.Request().GetAsync();
            var results = new List<User>();
            foreach (var member in groupMembers)
                if (member is User)
                {
                    var userMember = member as User;
                    results.Add(userMember);
                }

            return results;
        }

        public async Task<IEnumerable<MccGroup>> GetAllGroups()
        {
            var dirObjects = await _graphService.Client.Groups.Request().GetAsync();
            var result = new List<MccGroup>();
            foreach (var dirObject in dirObjects)
                result.Add(new MccGroup()
                {
                    Id = Guid.Parse(dirObject.Id),
                    Name = dirObject.DisplayName
                });
            return result;
        }

        public Task<IEnumerable<MccGroup>> UserMemberOf(Guid userId)
        {
            return UserMemberOf(userId.ToString());
        }

        public async Task<IEnumerable<MccGroup>> UserMemberOf(string userId)
        {
            IUserMemberOfCollectionWithReferencesPage groupsCollection =
                await _graphService.Client.Users[userId].MemberOf.Request().GetAsync();
            var result = new List<MccGroup>();
            if (groupsCollection?.Count == 0)
                return result;

            foreach (DirectoryObject dirObject in groupsCollection)
                if (dirObject is Group)
                {
                    Group group = dirObject as Group;
                    if (!Guid.TryParse(@group.Id, out var groupId))
                    {
                        _appInsightsService.TrackEvent(nameof(UserMemberOf),
                            $"Could not parse guid of group {@group.Id} with name {@group.DisplayName}");
                        continue;
                    }


                    result.Add(new MccGroup()
                        {
                            Name = @group.DisplayName,
                            Id = groupId
                        }
                    );
                }

            return result;
        }


        #region Implementation of ICampusService

        public Task<Campus> CreateCampus(Campus campus)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<Campus>> GetAllCampus()
        {
            var queryOptions = new List<QueryOption>
            {
                new QueryOption("$filter", $@"startswith(displayName, '{CampusGroupNamePrefix}')")
            };

            var dirObjects = await _graphService.Client.Groups.Request(queryOptions).GetAsync();
            var result = new List<Campus>();
            foreach (var dirObject in dirObjects)
                result.Add(new Campus()
                {
                    Id = Guid.Parse(dirObject.Id),
                    Name = dirObject.DisplayName
                });
            return result;
        }

        public Task<IEnumerable<Campus>> GetAllCampusForhub(Guid hubId)
        {
            throw new NotImplementedException();
        }

        public Task<Campus> GetCampus(Guid id)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<BasicUser>> GetCampusUsers(Guid campusId)
        {
            var users = await GetGroupMembers(campusId);
            return GraphHelper.MapBasicUsers(users);
        }

        #endregion
    }
}