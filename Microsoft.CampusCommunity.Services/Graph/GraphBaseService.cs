using System;
using Microsoft.CampusCommunity.Infrastructure.Configuration;
using Microsoft.CampusCommunity.Infrastructure.Exceptions;
using Microsoft.CampusCommunity.Infrastructure.Interfaces;
using Microsoft.Graph;
using Microsoft.Graph.Auth;
using Microsoft.Identity.Client;

namespace Microsoft.CampusCommunity.Services.Graph
{
    public class GraphBaseService : IGraphBaseService
    {
        private IConfidentialClientApplication _msalClient;
        public GraphServiceClient Client { get; private set; }
        public GraphClientConfiguration Configuration { get; }
        public AuthorizationConfiguration AuthorizationConfiguration { get; }


        public GraphBaseService(GraphClientConfiguration configuration, AuthorizationConfiguration authorizationConfiguration)
        {
            Configuration = configuration;
            AuthorizationConfiguration = authorizationConfiguration;
            BuildGraphClient();
        }

        private void BuildGraphClient()
        {
            // check if configuration contains client secret
            if (string.IsNullOrWhiteSpace(Configuration.ClientSecret))
            {
                throw new MccBadConfigurationException("Graph API client secret is not configured");
            }

            _msalClient = ConfidentialClientApplicationBuilder.Create(Configuration.ClientId)
                .WithClientSecret(Configuration.ClientSecret)
                .WithAuthority(new Uri(Configuration.Authority))
                .Build();
            
            var authProvider = new ClientCredentialProvider(_msalClient);
            Client = new GraphServiceClient(authProvider);
        }
    }
}