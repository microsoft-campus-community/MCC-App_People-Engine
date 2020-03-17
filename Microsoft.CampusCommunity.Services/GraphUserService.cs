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
    public class GraphUserService : IGraphUserService
    {
		public const string UserJobTitleCampusLead = "Campus Lead";
		public const string UserJobTitleMember = "Member";

		private IGraphService _graphService;
		private IGraphGroupService _graphGroupService;
		private IAppInsightsService _appInsightsService;

        public GraphUserService(IGraphService graphService, IGraphGroupService graphGroupService, IAppInsightsService appInsightsService, AuthorizationConfiguration authorizationConfiguration)
        {
            _graphService = graphService;
			_appInsightsService = appInsightsService;
			_graphGroupService = graphGroupService;
        }

		public async Task<IEnumerable<BasicUser>> GetAllUsers()
		{
			var users = await _graphService.Client.Users.Request().GetAsync();
            return MapBasicUsers(users);		
		}

		public async Task<BasicUser> GetCurrentUser(Guid userId)
		{
			var user = await _graphService.Client.Users[userId.ToString()].Request().GetAsync();
			return BasicUser.FromGraphUser(user);
		}

		public async Task<User> GetLeadForCampus(string campusName) {
			var queryOptions = new List<QueryOption>
			{
				new QueryOption("$filter", $@"jobTitle eq '{UserJobTitleCampusLead}' AND companyName eq '{campusName}'")
			};
		
			var userResult = await _graphService.Client.Users.Request(queryOptions).GetAsync();
			if (userResult.Count != 1) 
				throw new ApplicationException($"Unable to find a user with job title lead and member of campus with name {campusName}");
			return userResult[0];
		}

		public async Task<BasicUser> CreateUser(NewUser user, Guid campusId)
		{
			// get campus and lead for user
			var campus = await _graphGroupService.GetGroupById(campusId);

			var graphUser = CreateGraphUser(user, campus);
			await _graphService.Client.Users.Request().AddAsync(graphUser);

			// add user to the corresponding groups


			// add new user as additional direct to lead
			var lead = await GetLeadForCampus(campus.Name);
			await AssignManager(graphUser, lead.Id);

			throw new NotImplementedException();
		}

		public async Task DefineCampusLead(Guid userId, Guid campusId)
		{
			// get the user and campus
			var user = await FindById(userId);
			var campus = await _graphGroupService.GetGroupById(campusId);

			// make sure the user has the correc job title and campus assigned
			user.JobTitle = UserJobTitleCampusLead;
			user.Department = campus.Id.ToString();
			user.CompanyName = campus.Name;

			// add the campus lead to the campusLeads group
			await _graphGroupService.AddUserToGroup(user, _)

			// change the manager of all members of the group to the new campus lead
			var campusMembers = await _graphGroupService.GetGroupMembers(campusId);

			// where() -> don't change the manager of the lead itself.
			foreach(var member in campusMembers.Where(m => m.Id != user.Id)) {
				try {
					await AssignManager(member, user.Id);
				} catch(Exception e) {
					_appInsightsService.TrackException(null, new Exception($"Could not assign manager {user.Id} to user {user.Id} ({user.MailNickname}).", e), Guid.Empty);
				}
			}
		}

		public Task AssignManager(User user, string managerId) {
			return _graphService
				.Client
				.Users[user.Id]
				.Manager
				.Reference
				.Request()
				.PutAsync(managerId); 
		}

		private static IEnumerable<BasicUser> MapBasicUsers(IEnumerable<User> users)
        {
            return users.Select(BasicUser.FromGraphUser).ToList();
        }

		private async Task<User> FindByAlias(string alias)
		{
			var queryOptions = new List<QueryOption>
			{
				new QueryOption("$filter", $@"mailNickname eq '{alias}'")
			};
		
			var userResult = await _graphService.Client.Users.Request(queryOptions).GetAsync();
			if (userResult.Count != 1) 
				throw new ApplicationException($"Unable to find a user with the alias {alias}");
			return userResult[0];
		}

		private async Task<User> FindById(Guid userId)
		{
			var userResult = await _graphService.Client.Users[userId.ToString()].Request().GetAsync();
			if (userResult == null) 
				throw new ApplicationException($"Unable to find a user with the id {userId}");
			return userResult;
		}

		

		private static User CreateGraphUser(NewUser newUser, MccGroup campus) {
			var user = new User()
			{
				HireDate = DateTime.UtcNow,
				UserPrincipalName = newUser.Email,
				JobTitle = UserJobTitleMember,
				GivenName = newUser.FirstName,
				DisplayName = $"{newUser.FirstName} {newUser.LastName}",
				Department = campus.Id.ToString(),
				CompanyName = campus.Name,
				Surname = newUser.LastName
			};
			return user;
		}


	}
}