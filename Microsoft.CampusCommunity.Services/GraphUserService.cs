using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.CampusCommunity.Infrastructure;
using Microsoft.CampusCommunity.Infrastructure.Configuration;
using Microsoft.CampusCommunity.Infrastructure.Entities;
using Microsoft.CampusCommunity.Infrastructure.Entities.Dto;
using Microsoft.CampusCommunity.Infrastructure.Exceptions;
using Microsoft.CampusCommunity.Infrastructure.Helpers;
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

		// TODO: Define in config
		public readonly Guid SkuId = new Guid("314c4481-f395-4525-be8b-2ec4bb1e9d91");

		public static readonly PasswordOptions DefaultPasswordOptions = new PasswordOptions(){
			RequiredLength = 15
		};

		// public readonly List<string> DefaultNewUserGroups = new List<string>(){
		// 	"", // everyone
		// 	""
		// };

		private IGraphService _graphService;
		private IGraphGroupService _graphGroupService;
		private IAppInsightsService _appInsightsService;
		private AuthorizationConfiguration _authorizationCongfiguration;

        public GraphUserService(IGraphService graphService, IGraphGroupService graphGroupService, IAppInsightsService appInsightsService, AuthorizationConfiguration authorizationConfiguration)
        {
            _graphService = graphService;
			_appInsightsService = appInsightsService;
			_graphGroupService = graphGroupService;
			_authorizationCongfiguration = authorizationConfiguration;
        }

		public async Task<IEnumerable<BasicUser>> GetAllUsers()
		{
			var users = await _graphService.Client.Users.Request().GetAsync();
            return GraphHelper.MapBasicUsers(users);		
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
				return null;
			return userResult[0];
		}

		public async Task<BasicUser> CreateUser(NewUser user, Guid campusId)
		{
			// get campus and lead for user
			var campus = await _graphGroupService.GetGroupById(campusId);

			var graphUser = CreateGraphUser(user, campus);
			await _graphService.Client.Users.Request().AddAsync(graphUser);

			// add user to the corresponding groups
			await _graphGroupService.AddUserToGroup(graphUser, _authorizationCongfiguration.AllCompanyGroupId);
			
			// add licence
			await AssignLicense(graphUser);

			// add new user as additional direct to lead
			var lead = await GetLeadForCampus(campus.Name);
			await AssignManager(graphUser, lead.Id);

			// Send welcome mail
			await SendNewUserWelcomeMail(graphUser, user, lead);

			return BasicUser.FromGraphUser(graphUser);
		}

		private async Task AssignLicense(User user) {
			var addLicenses = new List<AssignedLicense>()
			{
				new AssignedLicense
				{
					DisabledPlans = new List<Guid>(),
					SkuId = SkuId
				}
			};

			var removeLicenses = new List<Guid>();

			await _graphService.Client.Users[user.Id]
				.AssignLicense(addLicenses,removeLicenses)
				.Request()
				.PostAsync();
		}

		public async Task DefineCampusLead(Guid userId, Guid campusId)
		{
			// get the user and campus
			var user = await FindById(userId);
			var campus = await _graphGroupService.GetGroupById(campusId);

			// find existing lead of campus (if possible)
			User existingLead = await GetLeadForCampus(campus.Name);
			if (existingLead != null) {
				throw new MccBadRequestException($"Unable to assign campus lead because there is already an existing lead defined ({existingLead.Mail})");
			}

			// make sure the user has the correc job title and campus assigned
			user.JobTitle = UserJobTitleCampusLead;
			user.Department = campus.Id.ToString();
			user.CompanyName = campus.Name;

			// add the campus lead to the campusLeads group
			await _graphGroupService.AddUserToGroup(user, _authorizationCongfiguration.CampusLeadsGroupId);

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

		public async Task SendMail(string subject, string body, string fromUserId, string to) {
			var message = new Message
			{
				Subject = subject,
				Body = new ItemBody
				{
					ContentType = BodyType.Text,
					Content = body
				},
				ToRecipients = new List<Recipient>()
				{
					new Recipient
					{
						EmailAddress = new EmailAddress
						{
							Address = to
						}
					}
				},
				CcRecipients = new List<Recipient>()
			};
			var saveToSentItems = false;

			await _graphService.Client.Users[fromUserId]
				.SendMail(message, saveToSentItems)
				.Request()
				.PostAsync();
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

			var generatedPassword = AuthenticationHelper.GenerateRandomPassword(DefaultPasswordOptions);
			user.PasswordProfile = new PasswordProfile() {
				ForceChangePasswordNextSignIn = true,
				Password = generatedPassword
			};
			return user;
		}

		// TODO: Refine: Use Flow or something where everyone can change this
		private async Task SendNewUserWelcomeMail(User user, NewUser newUser, User campusLead) {
			var body = $"Hello {user.DisplayName},\nWelcome to the Microsoft Campus Community! We are very happy to have you as your newest member. Your new user with the address {user.Mail} is almost ready. We are just finishing a few things here. You can already try and login with the following credentials:\n\nUsername: {user.Mail}\nPassword: {user.PasswordProfile.Password}\n\nPlease change the password after you login for the first time. If there are any problems don't hesitate to contact us.\nLet's all have a great time working together.";
			await SendMail($"Welcome to the Microsoft Campus Community", body, campusLead.Id, newUser.SecondaryMail);
		}


	}
}