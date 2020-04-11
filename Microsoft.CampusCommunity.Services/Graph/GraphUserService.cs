using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.CampusCommunity.Infrastructure.Configuration;
using Microsoft.CampusCommunity.Infrastructure.Entities.Dto;
using Microsoft.CampusCommunity.Infrastructure.Exceptions;
using Microsoft.CampusCommunity.Infrastructure.Extensions;
using Microsoft.CampusCommunity.Infrastructure.Helpers;
using Microsoft.CampusCommunity.Infrastructure.Interfaces;
using Microsoft.Graph;

namespace Microsoft.CampusCommunity.Services.Graph
{
    public class GraphUserService : IGraphUserService
    {
        public const string UserJobTitleCampusLead = "Campus Lead";
        public const string UserJobTitleHubLead = "Hub Lead";
        public const string UserJobTitleMember = "Member";

        // TODO: Define in config
        public readonly Guid SkuId = new Guid("314c4481-f395-4525-be8b-2ec4bb1e9d91");

        public static readonly PasswordOptions DefaultPasswordOptions = new PasswordOptions()
        {
            RequiredLength = 15
        };

        // public readonly List<string> DefaultNewUserGroups = new List<string>(){
        // 	"", // everyone
        // 	""
        // };

        private readonly IGraphBaseService _graphService;
        private readonly IGraphGroupService _graphGroupService;
        private readonly IAppInsightsService _appInsightsService;
        private readonly AuthorizationConfiguration _authorizationConfiguration;

        public GraphUserService(IGraphBaseService graphService, IGraphGroupService graphGroupService,
            IAppInsightsService appInsightsService, AuthorizationConfiguration authorizationConfiguration)
        {
            _graphService = graphService;
            _appInsightsService = appInsightsService;
            _graphGroupService = graphGroupService;
            _authorizationConfiguration = authorizationConfiguration;
        }

        public async Task<IEnumerable<BasicUser>> GetAllUsers()
        {
            // TODO: check in the future if office location is supported as a graph filter attribute. Try something like $filter=officeLocation+eq+'Munich'. Currently this is not supported.

            var users = await _graphService.Client.Users.Request().GetAsync();

            // only return users where location is not empty
            var filteredUsers = users.Where(u => !string.IsNullOrWhiteSpace(u.OfficeLocation));

            return GraphHelper.MapBasicUsers(filteredUsers);
        }

        public async Task<BasicUser> GetBasicUserById(Guid userId)
        {
            var user = await GetGraphUserById(userId);
            return BasicUser.FromGraphUser(user);
        }

        public Task<User> GetGraphUserById(Guid userId)
        {
            return _graphService.Client.Users[userId.ToString()].Request().GetAsync();
        }

        public async Task<User> GetLeadForCampus(Guid campusId)
        {
            var queryOptions = new List<QueryOption>
            {
                new QueryOption("$filter", $@"jobTitle eq '{UserJobTitleCampusLead}' AND department eq '{campusId}'")
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

            var graphUser = CreateGraphUser(user, Campus.FromMccGroup(campus));
            var userPassword = graphUser.PasswordProfile.Password.ToSecureString();
            graphUser = await _graphService.Client.Users.Request().AddAsync(graphUser);

            // add user to the corresponding groups
            await _graphGroupService.AddUserToGroup(graphUser, _authorizationConfiguration.CommunityGroupId);
            await _graphGroupService.AddUserToGroup(graphUser, campusId);

            // add licence
            await AssignLicense(graphUser);

            // add new user as additional direct to lead
            var lead = await GetLeadForCampus(campus.Id);
            await AssignManager(graphUser, lead.Id);

            // Send welcome mail
            await SendNewUserWelcomeMail(graphUser, user, lead, userPassword);

            return BasicUser.FromGraphUser(graphUser);
        }

        /// <inheritdoc />
        public async Task<Guid> GetCampusIdForUser(Guid userId)
        {
            var user = await FindById(userId);

            // return campus id if guid can be parsed
            if (Guid.TryParse(user.Department, out var campusId))
                return campusId;
            return Guid.Empty;
        }

        private async Task AssignLicense(User user)
        {
            var addLicenses = new List<AssignedLicense>()
            {
                new AssignedLicense
                {
                    DisabledPlans = new List<Guid>(),
                    SkuId = SkuId
                }
            };

            await _graphService.Client.Users[user.Id]
                .AssignLicense(addLicenses, new List<Guid>())
                .Request()
                .PostAsync();
        }

        public async Task DefineCampusLead(Guid userId, Guid campusId)
        {
            // get the user and campus
            var user = await FindById(userId);
            var campus = await _graphGroupService.GetGroupById(campusId);

            // find existing lead of campus (if possible)
            User existingLead = await GetLeadForCampus(campus.Id);
            if (existingLead != null)
                throw new MccBadRequestException(
                    $"Unable to assign campus lead because there is already an existing lead defined ({existingLead.Mail})");

            // make sure the user has the correct job title and campus assigned
            var userUpdate = new User()
            {
                Id = userId.ToString(),
                JobTitle = UserJobTitleCampusLead,
                Department = campus.Id.ToString(),
                CompanyName = campus.Name
            };
            await _graphService.Client.Users[user.Id].Request().UpdateAsync(userUpdate);

            // add the campus lead to the campusLeads group
            await _graphGroupService.AddUserToGroup(user, _authorizationConfiguration.CampusLeadsGroupId);


            // change the manager of all members of the group to the new campus lead
            var campusMembers = await _graphGroupService.GetGroupMembers(campusId);

            // where() -> don't change the manager of the lead itself.
            foreach (var member in campusMembers.Where(m => m.Id != user.Id))
                try
                {
                    await AssignManager(member, user.Id);
                }
                catch (Exception e)
                {
                    _appInsightsService.TrackException(null,
                        new Exception($"Could not assign manager {user.Id} to user {user.Id} ({user.MailNickname}).",
                            e), Guid.Empty);
                }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="newLead"></param>
        /// <param name="campusLeads">Ids of campus leads under hub</param>
        /// <param name="hubId"></param>
        /// <returns></returns>
        public async Task DefineHubLead(Guid newLead, IEnumerable<Guid> campusLeads, Guid hubId)
        {
            var user = await FindById(newLead);
            var hub = await _graphGroupService.GetGroupById(hubId);
            
            // make sure the user has the correct job title
            var userUpdate = new User()
            {
                Id = newLead.ToString(),
                JobTitle = UserJobTitleHubLead
            };
            await _graphService.Client.Users[user.Id].Request().UpdateAsync(userUpdate);

            // add the campus lead to the campusLeads group
            await _graphGroupService.AddUserToGroup(user, _authorizationConfiguration.HubLeadsGroupId);

            // change the manager of all members of the group to the new campus lead

            // where() -> don't change the manager of the lead itself.
            foreach (var member in campusLeads.Where(m => m.ToString() != user.Id))
                try
                {
                    await AssignManager(member, user.Id);
                }
                catch (Exception e)
                {
                    _appInsightsService.TrackException(null,
                        new Exception($"Could not assign manager {user.Id} to user {user.Id} ({user.MailNickname}).",
                            e), Guid.Empty);
                }
        }

        public Task AssignManager(User user, string managerId)
        {
            return _graphService
                .Client
                .Users[user.Id]
                .Manager
                .Reference
                .Request()
                .PutAsync(managerId);
        }

        public Task AssignManager(Guid userId, string managerId)
        {
            return _graphService
                .Client
                .Users[userId.ToString()]
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

        public async Task SendMail(string subject, string body, User fromUser, string to)
        {
            var message = new Message
            {
                Subject = subject,
                Sender = new Recipient()
                {
                    EmailAddress = new EmailAddress()
                    {
                        Name = fromUser.DisplayName,
                        Address = fromUser.Mail
                    }
                },
                From = new Recipient()
                {
                    EmailAddress = new EmailAddress()
                    {
                        Name = fromUser.DisplayName,
                        Address = fromUser.Mail
                    }
                },
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
                }
            };
            var r = _graphService.Client.Users[fromUser.Id]
                .SendMail(message, false)
                .Request();
            await r.PostAsync();
        }

        private static User CreateGraphUser(NewUser newUser, Campus campus)
        {
            var user = new User()
            {
                //HireDate = DateTime.UtcNow,
                UserPrincipalName = newUser.Email,
                JobTitle = UserJobTitleMember,
                GivenName = newUser.FirstName,
                DisplayName = $"{newUser.FirstName} {newUser.LastName}",
                Department = campus.Id.ToString(),
                CompanyName = campus.Name,
                Surname = newUser.LastName,
                AccountEnabled = true,
                MailNickname = $"{newUser.FirstName}.{newUser.LastName}",
                OfficeLocation = campus.CampusLocation,
                UsageLocation = "DE"
            };

            var generatedPassword = AuthenticationHelper.GenerateRandomPassword(DefaultPasswordOptions);
            user.PasswordProfile = new PasswordProfile()
            {
                ForceChangePasswordNextSignIn = true,
                Password = generatedPassword
            };
            return user;
        }

        // TODO: Refine: Use Flow or something where everyone can change this
        private async Task SendNewUserWelcomeMail(User user, NewUser newUser, User campusLead, SecureString userPassword)
        {
            var body =
                $"Hello {user.DisplayName},\nWelcome to the Microsoft Campus Community! We are very happy to have you as your newest member. Your new user with the address {newUser.Email} is almost ready. We are just finishing a few things here and there. You can already try and login with the following credentials:\n\nUsername: {newUser.Email}\nPassword: {userPassword.ToUnsecureString()}\n\nPlease change the password after you login for the first time. If there are any problems don't hesitate to contact us.\nLet's all have a great time working together.\n\nBest regards\n{campusLead.DisplayName}\n\nPlease not that this mail was generated automatically.";
            await SendMail($"Welcome to the Microsoft Campus Community", body, campusLead, newUser.SecondaryMail);
        }
    }
}