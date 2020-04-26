using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.CampusCommunity.Api.Extensions;
using Microsoft.CampusCommunity.Api.Helpers;
using Microsoft.CampusCommunity.Infrastructure.Configuration;
using Microsoft.CampusCommunity.Infrastructure.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace Microsoft.CampusCommunity.Api
{
    /// <summary>
    /// Default startup class
    /// </summary>
    public class Startup
    {
        /// <summary>
        /// Default startup class constructor
        /// </summary>
        /// <param name="configuration"></param>
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        /// <summary>
        /// Configuration of app
        /// </summary>
        public IConfiguration Configuration { get; }

        /// <summary>
        ///     This method gets called by the runtime. Use this method to add services to the container.
        /// </summary>
        /// <param name="services"></param>
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDependencies(Configuration);
            services.AddControllers()
                .AddJsonOptions(options =>
                    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));
        }


        /// <summary>
        ///     This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        /// </summary>
        /// <param name="app"></param>
        /// <param name="env"></param>
        /// <param name="authenticationOptions"></param>
        /// <param name="configuration"></param>
        /// <param name="appInsightsService"></param>
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env,
            IOptions<AadAuthenticationConfiguration> authenticationOptions, IConfiguration configuration, IAppInsightsService appInsightsService)
        {
            bool isDevEnv = env.IsDevelopment() || env.EnvironmentName.StartsWith("Development");
            bool useDevExceptionPage = configuration.GetValue<bool>("useDeveloperExceptionPage");
            if (useDevExceptionPage)
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionMiddleware(appInsightsService);
            }

            app.UseHttpsRedirection();
            app.UseSwagger();
            app.UseSwaggerUI(o =>
            {
                o.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
                o.OAuthClientId(authenticationOptions.Value.ClientId);
                o.DisplayRequestDuration();
                o.InjectStylesheet($"/css/{configuration.GetSection("SwaggerConfiguration")["CssFile"]}");
            });

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();
            app.UseStaticFiles();

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });

            var syncDataOnStartup = configuration.GetValue<bool>("syncDataOnStartup");
            DatabaseSeeder.Seed(app, migrate: true, seedDevData: false, syncDataOnStartup);
        }
    }
}