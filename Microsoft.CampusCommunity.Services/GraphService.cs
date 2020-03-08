using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.CampusCommunity.Infrastructure.Entities;
using Microsoft.CampusCommunity.Infrastructure.Entities.Dto;
using Microsoft.CampusCommunity.Infrastructure.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Graph;
using Microsoft.Graph.Auth;
using Microsoft.Identity.Client;


namespace Microsoft.CampusCommunity.Services
{
    public class GraphService : IGraphService
    {
        private readonly GraphClientConfiguration _configuration;
        private IConfidentialClientApplication _msalClient;
        private GraphServiceClient _graphClient;

        public GraphService(GraphClientConfiguration configuration)
        {
            _configuration = configuration;
            BuildGraphClient();
        }

        private void BuildGraphClient()
        {
            _msalClient = ConfidentialClientApplicationBuilder.Create(_configuration.ClientId)
                .WithClientSecret(_configuration.ClientSecret)
                .WithAuthority(new Uri(_configuration.Authority))
                .Build();
        }

        private async Task<string> Authenticate()
        {
            //string[] scopes = new string[] { _configuration.Scope };
            string[] scopes = new string[] { "https://graph.microsoft.com/.default" };

            AuthenticationResult authResult = null;
            try
            {
                authResult = await _msalClient.AcquireTokenForClient(scopes).ExecuteAsync();
            }
            catch (MsalUiRequiredException e)
            {
                // The application doesn't have sufficient permissions.
                // - Did you declare enough app permissions during app creation?
                // - Did the tenant admin grant permissions to the application?
                Console.WriteLine(e);
                throw;
            }
            catch (MsalServiceException e)
            {
                // AADSTS70011
                // Invalid scope. The scope has to be of the form "https://resourceurl/.default"
                // Mitigation: this is a dev issue. Change the scope to be as expected
                Console.WriteLine(e);
                throw;
            }
            Console.WriteLine(authResult.AccessToken);
            Console.WriteLine(authResult.IdToken);

            var authProvider = new ClientCredentialProvider(_msalClient);
            _graphClient = new GraphServiceClient(authProvider);



            return authResult.AccessToken;
        }

        private IEnumerable<BasicUser> MapBasicUsers(IEnumerable<User> users)
        {
            return users.Select(BasicUser.FromGraphUser).ToList();
        }


        public async Task<IEnumerable<BasicUser>> GetUsers()
        {
            var token = await Authenticate();
            var users = await _graphClient.Users.Request().GetAsync();
            return MapBasicUsers(users);
        }
    }
}