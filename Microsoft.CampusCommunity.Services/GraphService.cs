using System;
using Microsoft.CampusCommunity.Infrastructure.Configuration;
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
        public GraphServiceClient GraphClient { get; private set; }

        protected GraphService(GraphClientConfiguration configuration)
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
            GraphClient = new GraphServiceClient(authProvider);
        }
    }
}