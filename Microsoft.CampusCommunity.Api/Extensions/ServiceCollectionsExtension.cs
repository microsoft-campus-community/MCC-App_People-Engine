﻿using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.AzureAD.UI;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.CampusCommunity.Api.Authorization;
using Microsoft.CampusCommunity.DataAccess;
using Microsoft.CampusCommunity.Infrastructure.Configuration;
using Microsoft.CampusCommunity.Infrastructure.Entities;
using Microsoft.CampusCommunity.Infrastructure.Interfaces;
using Microsoft.CampusCommunity.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.PlatformAbstractions;
using Microsoft.OpenApi.Models;

namespace Microsoft.CampusCommunity.Api.Extensions
{
    /// <summary>
    /// Class to setup DI and other settings used in Startup.cs
    /// </summary>
    public static class ServiceCollectionsExtension
    {
        private const string AuthenticationSettingsSectionName = "AzureAd";
        private const string GraphAuthenticationSettingsSectionName = "Graph";
        private const string AuthorizationSettingsSectionName = "Authorization";

        /// <summary>
        /// Adds all necessary dependencies, initializes DI for services and setup authentication and swagger
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public static IServiceCollection AddDependencies(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<IConfiguration>(configuration);
            
            var authenticationOptions = configuration.GetSection(AuthenticationSettingsSectionName)
                .Get<AadAuthenticationConfiguration>();
            services.Configure<AadAuthenticationConfiguration>(configuration.GetSection(AuthenticationSettingsSectionName));


            services.AddAuthentication(authenticationOptions);
            services.AddAuthorization(configuration);
            services.AddContext(configuration);
            services.AddSwagger(authenticationOptions);
            services.AddMccServices(configuration);

            return services;
        }

        private static IServiceCollection AddAuthentication(this IServiceCollection services, AadAuthenticationConfiguration authenticationOptions)
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
                options.Authority += "/v2.0"; // the token issuer is https://login.microsoftonline.com/TENANT_ID/v2.0 but the default scheme uses https://login.microsoftonline.com/TENANT_ID
                options.SaveToken = true;
            });

            return services;
        }

        private static IServiceCollection AddAuthorization(this IServiceCollection services,
            IConfiguration configuration)
        {
            // Create config
            var authorizationConfigSection = configuration.GetSection(AuthorizationSettingsSectionName);
            var authConfig = new AuthorizationConfiguration(
                authorizationConfigSection["AllCompanyGroup"],
                authorizationConfigSection["CampusLeadsGroup"],
                authorizationConfigSection["GermanLeadsGroup"],
                authorizationConfigSection["HubLeadsGroup"],
                authorizationConfigSection["InternalDevelopmentGroup"]
                );

            var developmentPolicyGroups = new GroupMembershipRequirement(new []{authConfig.InternalDevelopmentGroupId});
            var germanLeadsPolicyGroups = new GroupMembershipRequirement(new[] { authConfig.GermanLeadsGroupId });
            var hubLeadsGroup = new GroupMembershipRequirement(new[] { authConfig.InternalDevelopmentGroupId, authConfig.GermanLeadsGroupId, authConfig.HubLeadsGroupId });
            var campusLeadsGroup = new GroupMembershipRequirement(new[] { authConfig.InternalDevelopmentGroupId, authConfig.GermanLeadsGroupId, authConfig.HubLeadsGroupId, authConfig.CampusLeadsGroupId });
            var generalGroup = new GroupMembershipRequirement(new[] { authConfig.InternalDevelopmentGroupId, authConfig.GermanLeadsGroupId, authConfig.HubLeadsGroupId, authConfig.CampusLeadsGroupId, authConfig.AllCompanyGroupId });

            services.AddAuthorization(options =>
            {
                options.AddPolicy(PolicyNames.DevelopmentInternal, policy => { policy.Requirements.Add(developmentPolicyGroups); });
                options.AddPolicy(PolicyNames.GermanLeads, policy => { policy.Requirements.Add(germanLeadsPolicyGroups); });
                options.AddPolicy(PolicyNames.HubLeads, policy => { policy.Requirements.Add(hubLeadsGroup); });
                options.AddPolicy(PolicyNames.CampusLeads, policy => { policy.Requirements.Add(campusLeadsGroup); });
                options.AddPolicy(PolicyNames.General, policy => { policy.Requirements.Add(generalGroup); });
            });

            services.AddSingleton<IAuthorizationHandler, GroupMembershipPolicyHandler>();
			services.AddSingleton<AuthorizationConfiguration>(authConfig);

            return services;
        }

        private static IServiceCollection AddContext(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("connectionString");
            services.AddDbContext<MccContext>(options => { options.UseSqlServer(connectionString); });
            return services;
        }

        private static IServiceCollection AddSwagger(this IServiceCollection services, AadAuthenticationConfiguration authenticationOptions)
        {
            services.AddSwaggerGen(o =>
            {
                o.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Microsoft Campus Community API",
                    Description = "API for the Microsoft Campus Community Management API Backend",
                    Version = "1.0",
                    Contact = new OpenApiContact() { Email = "info@campus-community.org", Name = "Microsoft Campus Community"}
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
                string xmlDocFilePath = Path.Combine(PlatformServices.Default.Application.ApplicationBasePath, "api.xml");
                o.IncludeXmlComments(xmlDocFilePath);
            });
            return services;
        }

        private static IServiceCollection AddMccServices(this IServiceCollection services, IConfiguration configuration)
        {
            var graphConfigSection = configuration.GetSection(GraphAuthenticationSettingsSectionName);
            var graphConfig = graphConfigSection.Get<GraphClientConfiguration>();
            services.AddSingleton<GraphClientConfiguration>(graphConfig);


            services.AddScoped<IGraphService, GraphService>();

            return services;
        }
    }
}
