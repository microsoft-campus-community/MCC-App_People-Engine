using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Microsoft.CampusCommunity.DataAccess;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

namespace Microsoft.CampusCommunity.Api.Extensions
{
    public static class ServiceCollectionsExtension
    {
        public static IServiceCollection AddDependencies(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<IConfiguration>(configuration);

            // add db context
            var connectionString = configuration.GetConnectionString("connectionString");
            services.AddDbContext<MccContext>(options => { options.UseSqlServer(connectionString); });

            // add swagger
            //services.AddSwaggerGen(setup =>
            //{
            //    setup.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme()
            //    {
            //        Type = SecuritySchemeType.OAuth2,
            //        Flows = new OpenApiOAuthFlows()
            //        {
            //            Implicit = new OpenApiOAuthFlow()
            //            {
            //                TokenUrl = new Uri($"{servicePrinciples.Api.Instance}/{servicePrinciples.Api.TenantId}/oauth2/v2.0/token"),
            //                AuthorizationUrl = new Uri($"{servicePrinciples.Api.Instance}/{servicePrinciples.Api.TenantId}/oauth2/v2.0/authorize"),
            //                Scopes =
            //                {
            //                    {servicePrinciples.Api.Scope, servicePrinciples.Api.ScopeDescription }
            //                }
            //            }
            //        }
            //    });
            //})

            return services;

        }
    }
}
