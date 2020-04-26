using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.CampusCommunity.Infrastructure.Entities;
using Microsoft.CampusCommunity.Infrastructure.Entities.Dto;
using Microsoft.CampusCommunity.Infrastructure.Exceptions;
using Microsoft.CampusCommunity.Infrastructure.Extensions;
using Microsoft.CampusCommunity.Infrastructure.Helpers;
using Microsoft.CampusCommunity.Infrastructure.Interfaces;
using Microsoft.Graph;

namespace Microsoft.CampusCommunity.Services.Graph
{
    public class GraphGroupService : IGraphGroupService, IGraphCampusService
    {
        private readonly IGraphBaseService _graphService;
        private readonly IAppInsightsService _appInsightsService;

        public const string CampusGroupNamePrefix = "Campus";
        public const string HubGroupNamePrefix = "Hub";

        public GraphGroupService(IGraphBaseService graphService, IAppInsightsService appInsightsService)
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

        public async Task<MccGraphGroup> GetGroupById(Guid campusId)
        {
            var group = await _graphService.Client.Groups[campusId.ToString()].Request().GetAsync();

            return new MccGraphGroup()
            {
                Id = Guid.Parse(group.Id),
                Name = group.DisplayName
            };
        }

        public async Task<IEnumerable<User>> GetGroupMembers(Guid groupId)
        {
            var groupMembers = await _graphService.Client.Groups[groupId.ToString()].Members.Request().GetAsync();

            return groupMembers.OfType<User>().Select(member => member).ToList();
        }

        public async Task<IEnumerable<MccGraphGroup>> GetAllGroups()
        {
            var dirObjects = await _graphService.Client.Groups.Request().GetAsync();
            return dirObjects.Select(MccGraphGroup.FromGraph).ToList();
        }

        public async Task<AuthorizationGroupMembers> GetGroupMembersOfAuthorizationGroups()
        {
            var campusLeadsGroup = _graphService.AuthorizationConfiguration.CampusLeadsGroupId;
            var hubLeadsGroup = _graphService.AuthorizationConfiguration.HubLeadsGroupId;
            var germanLeadsGroup = _graphService.AuthorizationConfiguration.GermanLeadsGroupId;

            var campusLeads = await GetGroupMembers(campusLeadsGroup);
            var hubLeads = await GetGroupMembers(hubLeadsGroup);
            var germanLeads = await GetGroupMembers(germanLeadsGroup);

            return new AuthorizationGroupMembers(campusLeads, hubLeads, germanLeads);
        }

        public Task<IEnumerable<MccGraphGroup>> UserMemberOf(Guid userId)
        {
            return UserMemberOf(userId.ToString());
        }

        /// <inheritdoc />
        public async Task<MccGraphGroup> CreateGroup(string name, Guid owner, string description, bool shouldCreateTeamsTeam)
        {
            // make sure there are no umlaute 
            var mailNickname = name.Replace(' ', '.').RemoveDiacritics();
            var newGroup = new Group()
            {
                DisplayName = name,
                MailEnabled = true,
                GroupTypes = new[] {"Unified"},
                MailNickname = mailNickname,
                SecurityEnabled = true,
                Description = description
            };
            newGroup = await _graphService.Client.Groups.Request().AddAsync(newGroup);

            // add owner
            var ownerUserObject = await _graphService.Client.Users[owner.ToString()].Request().GetAsync();
            try
            {
                await _graphService.Client.Groups[newGroup.Id].Owners.References.Request().AddAsync(ownerUserObject);
            }
            catch (Exception e)
            {
                var exWrapper =
                    new MccGraphException(
                        $"Could not add owner to group {newGroup.DisplayName} and id {newGroup.Id}. Owner: {owner}", e);
                _appInsightsService.TrackException(null, exWrapper, Guid.NewGuid());
                Console.WriteLine("Could not add owner to group.");
            }

            if (shouldCreateTeamsTeam)
            {
                try
                {
                    await CreateTeamsTeam(newGroup.Id);
                }
                catch
                {
                    // Rollback creation
                    await _graphService.Client.Groups[newGroup.Id].Request().DeleteAsync();
                    throw;
                }
            }
                

            return MccGraphGroup.FromGraph(newGroup);
        }

        /// <inheritdoc />
        public async Task ChangeGroupOwner(Guid groupId, Guid newOwner)
        {
            // get user for new owner
            var user = await _graphService.Client.Users[newOwner.ToString()].Request().GetAsync();
            await _graphService.Client.Groups[groupId.ToString()].Owners.References.Request().AddAsync(user);
        }

        public async Task<IEnumerable<MccGraphGroup>> UserMemberOf(string userId)
        {
            IUserMemberOfCollectionWithReferencesPage groupsCollection;
            try
            {
                groupsCollection = await _graphService.Client.Users[userId].MemberOf.Request().GetAsync();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                Console.WriteLine(_graphService.Client.Users[userId].MemberOf.Request().RequestUrl);
                throw;
            }

            if (groupsCollection == null)
                return new List<MccGraphGroup>();


            var result = new List<MccGraphGroup>();
            if (groupsCollection.Count == 0)
                return result;

            foreach (DirectoryObject dirObject in groupsCollection)
                if (dirObject is Group @group)
                {
                    if (!Guid.TryParse(@group.Id, out var groupId))
                    {
                        _appInsightsService.TrackEvent(nameof(UserMemberOf),
                            $"Could not parse guid of group {@group.Id} with name {@group.DisplayName}");
                        continue;
                    }


                    result.Add(new MccGraphGroup()
                        {
                            Name = @group.DisplayName,
                            Id = groupId
                        }
                    );
                }

            return result;
        }

        /// <summary>
        /// Create a new teams. This operation is potentially long running because we might need to wait until the group is synced.
        /// See comment before Task.Delay
        /// </summary>
        /// <param name="groupId"></param>
        /// <returns></returns>
        private async Task CreateTeamsTeam(string groupId)
        {
            var team = new Team()
            {
                GuestSettings = new TeamGuestSettings
                {
                    AllowCreateUpdateChannels = true,
                    AllowDeleteChannels = true,
                    ODataType = null
                },
                MemberSettings = new TeamMemberSettings
                {
                    AllowCreateUpdateChannels = true,
                    ODataType = null
                },
                MessagingSettings = new TeamMessagingSettings
                {
                    AllowUserEditMessages = true,
                    AllowUserDeleteMessages = true,
                    ODataType = null
                },
                FunSettings = new TeamFunSettings
                {
                    AllowGiphy = true,
                    GiphyContentRating = GiphyRatingType.Moderate,
                    ODataType = null
                },
                ODataType = null
            };
            // we have to potentially try this 3 times
            Team newTeam = null;
            for (var i = 0; i < 3; i++)
            {
                try
                { 
                    newTeam = await _graphService.Client.Groups[groupId].Team.Request().PutAsync(team);
                }
                catch (Exception e)
                {
                    _appInsightsService.TrackException(null, e, Guid.NewGuid());
                }

                if (newTeam != null) break;

                // add delay 
                // https://docs.microsoft.com/en-us/graph/api/team-put-teams?view=graph-rest-1.0&tabs=csharp
                // If the group was created less than 15 minutes ago, it's possible for the Create team call to fail with a 404 error code due to replication delays.
                // The recommended pattern is to retry the Create team call three times, with a 10 second delay between calls.
                await Task.Delay(3000);
            }

            if (newTeam == null)
                throw new MccGraphException(
                    $"Even after three consecutive tries, a team could not be created for the group with Id {groupId}");
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
            return dirObjects.Select(dirObject => new Campus(Guid.Empty)
                {Id = Guid.Parse(dirObject.Id), Name = dirObject.DisplayName}).ToList();
        }

        public Task<IEnumerable<Campus>> GetAllCampusForHub(Guid hubId)
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