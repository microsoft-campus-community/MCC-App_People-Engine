using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Microsoft.CampusCommunity.DataAccess;
using Microsoft.CampusCommunity.Infrastructure.Entities;
using Microsoft.CampusCommunity.Infrastructure.Interfaces;
using Microsoft.CampusCommunity.Services;
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

            var graphConfigSection = configuration.GetSection("Graph");
            var graphConfig = graphConfigSection.Get<GraphClientConfiguration>();
            services.AddSingleton<GraphClientConfiguration>(graphConfig);
            services.AddScoped<IGraphService, GraphService>();

            

            return services;

        }
    }
}
