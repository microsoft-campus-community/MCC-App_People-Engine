using System;
using Microsoft.CampusCommunity.Infrastructure.Configuration;
using Microsoft.CampusCommunity.Infrastructure.Exceptions;
using Microsoft.CampusCommunity.Infrastructure.Interfaces;
using Microsoft.Graph;
using Microsoft.Graph.Auth;
using Microsoft.Identity.Client;

namespace Microsoft.CampusCommunity.Services
{
    public class GraphService : IGraphService
    {
        private readonly GraphClientConfiguration _configuration;
        private IConfidentialClientApplication _msalClient;
        public GraphServiceClient Client { get; private set; }

        public GraphService(GraphClientConfiguration configuration)
        {
            _configuration = configuration;
            BuildGraphClient();
        }

        private void BuildGraphClient()
        {
            // check if configuration contains client secret
            if (string.IsNullOrWhiteSpace(_configuration.ClientSecret))
            {
                throw new MccBadConfigurationException("Graph API client secret is not configured");
            }

            _msalClient = ConfidentialClientApplicationBuilder.Create(_configuration.ClientId)
                .WithClientSecret(_configuration.ClientSecret)
                .WithAuthority(new Uri(_configuration.Authority))
                .Build();
            
            var authProvider = new ClientCredentialProvider(_msalClient);
            Client = new GraphServiceClient(authProvider);
        }
    }
}