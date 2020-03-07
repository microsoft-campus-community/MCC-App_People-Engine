using System;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Authentication.AzureAD.UI;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Diagnostics;
using Microsoft.CampusCommunity.Api.Authorization;
using Microsoft.CampusCommunity.Api.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Microsoft.Extensions.PlatformAbstractions;

namespace Microsoft.CampusCommunity.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }
        private string _authenticationSettingsSectionName = "AzureAd";

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var authenticationOptions = Configuration.GetSection(_authenticationSettingsSectionName)
                .Get<ConfigurationAuthenticationOptions>();
            AddAuthentication(services, authenticationOptions);
            AddSwagger(services, authenticationOptions);

            
            services.AddControllers();
        }

        private void AddAuthentication(IServiceCollection services, ConfigurationAuthenticationOptions authenticationOptions)
        {
            services.Configure<ConfigurationAuthenticationOptions>(Configuration.GetSection(_authenticationSettingsSectionName));
            services.AddAuthentication(AzureADDefaults.BearerAuthenticationScheme)
                .AddAzureADBearer(options => Configuration.Bind(_authenticationSettingsSectionName, options));
        }

        private static void AddSwagger(IServiceCollection services, ConfigurationAuthenticationOptions authenticationOptions)
        {
            services.AddSwaggerGen(o =>
            {
                o.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Microsoft Campus Community API",
                    Description = "API for the Microsoft Campus Community Management API Backend",
                    Version = "1.0",
                });

                o.AddSecurityDefinition("aad-jwt", new OpenApiSecurityScheme()
                {
                    Type = SecuritySchemeType.OAuth2,
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
                //o.OperationFilter<OAuthSecurityRequirementOperationFilter>();

                // Include XML comments to documentation
                string xmlDocFilePath = Path.Combine(PlatformServices.Default.Application.ApplicationBasePath, "api.xml");
                o.IncludeXmlComments(xmlDocFilePath);
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IOptions<ConfigurationAuthenticationOptions> authenticationOptions)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();
            app.UseSwagger();
            app.UseSwaggerUI(o =>
            {
                o.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
                o.OAuthClientId(authenticationOptions.Value.ClientId);
            });

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            //app.UseCors(policy => policy
            //    .AllowAnyOrigin()
            //    .AllowAnyMethod()
            //    .AllowAnyHeader()
            //    .AllowCredentials());
        }
    }
}
