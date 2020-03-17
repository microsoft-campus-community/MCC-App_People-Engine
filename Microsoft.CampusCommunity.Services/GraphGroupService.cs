using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.CampusCommunity.Infrastructure.Configuration;
using Microsoft.CampusCommunity.Infrastructure.Entities;
using Microsoft.CampusCommunity.Infrastructure.Entities.Dto;
using Microsoft.CampusCommunity.Infrastructure.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Graph;
using Microsoft.Graph.Auth;
using Microsoft.Identity.Client;


namespace Microsoft.CampusCommunity.Services
{
    public class GraphGroupService : IGraphGroupService
    {
		private IGraphService _graphService;
		private IAppInsightsService _appInsightsService;

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

		

		public Task<IEnumerable<MccGroup>> UserMemberOf(Guid userId) 
		{
			return UserMemberOf(userId.ToString());
		}

		public async Task<IEnumerable<MccGroup>> UserMemberOf(string userId) 
		{
			IUserMemberOfCollectionWithReferencesPage groupsCollection = await _graphService.Client.Users[userId].MemberOf.Request().GetAsync();
            var result = new List<MccGroup>();
			if (groupsCollection?.Count == 0)
				return result;

			foreach (DirectoryObject dirObject in groupsCollection)
			{
				if (dirObject is Group)
				{
					Group group = dirObject as Group;
					if (!Guid.TryParse(group.Id, out var groupId)) {
						_appInsightsService.TrackEvent($"Could not parse guid of group {group.Id} with name {group.DisplayName}");
						continue;
					}


					result.Add(new MccGroup()
						{
							Name = group.DisplayName,
							Id = groupId
						}
					);
				}
			}

            return result;
		}
		
		


	}
}