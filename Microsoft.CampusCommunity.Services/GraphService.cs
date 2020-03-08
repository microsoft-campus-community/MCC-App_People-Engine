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
            var authProvider = new ClientCredentialProvider(_msalClient);
            _graphClient = new GraphServiceClient(authProvider);
        }
        
        private IEnumerable<BasicUser> MapBasicUsers(IEnumerable<User> users)
        {
            return users.Select(BasicUser.FromGraphUser).ToList();
        }


        public async Task<IEnumerable<BasicUser>> GetUsers()
        {
            var users = await _graphClient.Users.Request().GetAsync();
            return MapBasicUsers(users);
        }
    }
}