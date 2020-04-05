using System;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.AzureAD.UI;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.CampusCommunity.Api.Authorization;
using Microsoft.CampusCommunity.DataAccess;
using Microsoft.CampusCommunity.Infrastructure.Configuration;
using Microsoft.CampusCommunity.Infrastructure.Exceptions;
using Microsoft.CampusCommunity.Infrastructure.Interfaces;
using Microsoft.CampusCommunity.Services;
using Microsoft.CampusCommunity.Services.Graph;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.PlatformAbstractions;
using Microsoft.OpenApi.Models;

namespace Microsoft.CampusCommunity.Api.Extensions
{
    /// <summary>
    ///     Class to setup DI and other settings used in Startup.cs
    /// </summary>
    public static class ServiceCollectionsExtension
    {
        private const string AuthenticationSettingsSectionName = "AzureAd";
        private const string GraphAuthenticationSettingsSectionName = "Graph";
        private const string AuthorizationSettingsSectionName = "AuthorizationGroups";

        /// <summary>
        ///     Adds all necessary dependencies, initializes DI for services and setup authentication and swagger
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public static IServiceCollection AddDependencies(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<IConfiguration>(configuration);

            var authenticationOptions = configuration.GetSection(AuthenticationSettingsSectionName)
                .Get<AadAuthenticationConfiguration>();

            if (!authenticationOptions.IsValid())
                throw new MccBadConfigurationException(
                    $"Could not start application because configuration for section {AuthenticationSettingsSectionName} is not valid and misses values. Configuration: {authenticationOptions.ToString()}");

            services.Configure<AadAuthenticationConfiguration>(
                configuration.GetSection(AuthenticationSettingsSectionName));
            services.AddApplicationInsightsTelemetry();

            services.AddAuthentication(authenticationOptions);
            services.AddAuthorization(configuration);
            services.AddContext(configuration);
            services.AddSwagger(authenticationOptions, configuration);
            services.AddMccServices(configuration);

            return services;
        }

        private static IServiceCollection AddAuthentication(this IServiceCollection services,
            AadAuthenticationConfiguration authenticationOptions)
        {
            services.AddAuthentication(AzureADDefaults.BearerAuthenticationScheme)
                .AddAzureADBearer(options =>
                {
                    options.ClientId = authenticationOptions.ClientId;
                    options.TenantId = authenticationOptions.TenantId;
                    options.Domain = authenticationOptions.Domain;
                    options.Instance = authenticationOptions.Instance;
                });
            services.Configure<JwtBearerOptions>(AzureADDefaults.JwtBearerAuthenticationScheme, options =>
            {
                options.Authority +=
                    "/v2.0"; // the token issuer is https://login.microsoftonline.com/TENANT_ID/v2.0 but the default scheme uses https://login.microsoftonline.com/TENANT_ID
                options.SaveToken = true;
            });

            return services;
        }

        private static IServiceCollection AddAuthorization(this IServiceCollection services,
            IConfiguration configuration)
        {
            // Create config
            var authorizationConfigSection = configuration.GetSection(AuthorizationSettingsSectionName);

            if (string.IsNullOrWhiteSpace(authorizationConfigSection["AllCompanyGroup"]) ||
                string.IsNullOrWhiteSpace(authorizationConfigSection["CampusLeadsGroup"]) ||
                string.IsNullOrWhiteSpace(authorizationConfigSection["GermanLeadsGroup"]) ||
                string.IsNullOrWhiteSpace(authorizationConfigSection["HubLeadsGroup"]) ||
                string.IsNullOrWhiteSpace(authorizationConfigSection["InternalDevelopmentGroup"]))
            {
                var sectionContents = authorizationConfigSection.AsEnumerable();
                var message =
                    $"The authorization configuration section seems to be empty or not configured. Please check settings for section with name {AuthorizationSettingsSectionName}. The following keys and values are present: ";
                message = sectionContents.Aggregate(message, (current, kv) => current + $" - ({kv.Key}): '{kv.Value}'");

                throw new MccBadConfigurationException(message);
            }

            var authConfig = new AuthorizationConfiguration(
                authorizationConfigSection["AllCompanyGroup"],
                authorizationConfigSection["CampusLeadsGroup"],
                authorizationConfigSection["GermanLeadsGroup"],
                authorizationConfigSection["HubLeadsGroup"],
                authorizationConfigSection["InternalDevelopmentGroup"]
            );

            var developmentPolicyGroups = new GroupMembershipRequirement(new[] {authConfig.InternalDevelopmentGroupId});
            var germanLeadsPolicyGroups = new GroupMembershipRequirement(new[] {authConfig.GermanLeadsGroupId});
            var hubLeadsGroup = new GroupMembershipRequirement(new[]
                {authConfig.InternalDevelopmentGroupId, authConfig.GermanLeadsGroupId, authConfig.HubLeadsGroupId});
            var campusLeadsGroup = new GroupMembershipRequirement(new[]
            {
                authConfig.InternalDevelopmentGroupId, authConfig.GermanLeadsGroupId, authConfig.HubLeadsGroupId,
                authConfig.CampusLeadsGroupId
            });
            var generalGroup = new GroupMembershipRequirement(new[]
            {
                authConfig.InternalDevelopmentGroupId, authConfig.GermanLeadsGroupId, authConfig.HubLeadsGroupId,
                authConfig.CampusLeadsGroupId, authConfig.CommunityGroupId
            });

            services.AddAuthorization(options =>
            {
                options.AddPolicy(PolicyNames.DevelopmentInternal,
                    policy => { policy.Requirements.Add(developmentPolicyGroups); });
                options.AddPolicy(PolicyNames.GermanLeads,
                    policy => { policy.Requirements.Add(germanLeadsPolicyGroups); });
                options.AddPolicy(PolicyNames.HubLeads, policy => { policy.Requirements.Add(hubLeadsGroup); });
                options.AddPolicy(PolicyNames.CampusLeads, policy => { policy.Requirements.Add(campusLeadsGroup); });
                options.AddPolicy(PolicyNames.Community, policy => { policy.Requirements.Add(generalGroup); });
            });

            services.AddScoped<IAuthorizationHandler, GroupMembershipPolicyHandler>();
            services.AddSingleton<AuthorizationConfiguration>(authConfig);

            return services;
        }

        private static IServiceCollection AddContext(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("connectionString");
            services.AddDbContext<MccContext>(options => { options.UseSqlServer(connectionString); });
            return services;
        }

        private static IServiceCollection AddSwagger(this IServiceCollection services,
            AadAuthenticationConfiguration authenticationOptions, IConfiguration configuration)
        {
            var swaggerConfigurationSection = configuration.GetSection("SwaggerConfiguration");

            services.AddSwaggerGen(o =>
            {
                o.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = swaggerConfigurationSection["Title"],
                    Description = string.Format(swaggerConfigurationSection["Description"], authenticationOptions.Domain, authenticationOptions.TenantId),
                    Version = "1.0",
                    Contact = new OpenApiContact()
                        {Email = "info@campus-community.org", Name = "Microsoft Campus Community"}
                });

                o.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
                {
                    Type = SecuritySchemeType.OAuth2,
                    Scheme = "MccApiAuth",
                    Flows = new OpenApiOAuthFlows()
                    {
                        // We only define implicit though the UI does support authorization code, client credentials and password grants
                        // We don't use authorization code here because it requires a client secret, which makes this sample more complicated by introducing secret management
                        // Client credentials could work, but not when the UI client id == API client id. We'd need a separate registration and granting app permissions to that. And also needs a secret.
                        // Password grant we don't use because... you shouldn't be using it.
                        Implicit = new OpenApiOAuthFlow
                        {
                            AuthorizationUrl = new Uri(authenticationOptions.AuthorizationUrl),
                            Scopes = DelegatedPermissions.All.ToDictionary(p =>
                                $"{authenticationOptions.ApplicationIdUri}/{p}")
                        }
                    }
                });

                // Add security requirements to operations based on [Authorize] attributes
                o.OperationFilter<OAuthSecurityRequirementOperationFilter>();

                // Include XML comments to documentation
                string xmlDocFilePath =
                    Path.Combine(PlatformServices.Default.Application.ApplicationBasePath, "api.xml");
                o.IncludeXmlComments(xmlDocFilePath);
            });
            return services;
        }

        private static IServiceCollection AddMccServices(this IServiceCollection services, IConfiguration configuration)
        {
            var graphConfigSection = configuration.GetSection(GraphAuthenticationSettingsSectionName);
            var graphConfig = graphConfigSection.Get<GraphClientConfiguration>();
            services.AddSingleton<GraphClientConfiguration>(graphConfig);


            services.AddScoped<IGraphBaseService, GraphBaseService>();
            services.AddScoped<IAppInsightsService, AppInsightsService>();
            services.AddScoped<IGraphCampusService, GraphGroupService>();
            services.AddScoped<IGraphGroupService, GraphGroupService>();
            services.AddScoped<IGraphUserService, GraphUserService>();

            return services;
        }
    }
}