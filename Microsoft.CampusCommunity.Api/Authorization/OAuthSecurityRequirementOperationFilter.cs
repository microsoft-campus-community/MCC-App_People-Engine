using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.CampusCommunity.Infrastructure.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Microsoft.CampusCommunity.Api.Authorization
{
    /// <summary>
    ///     Tells swagger when authentication is needed
    /// </summary>
    internal class OAuthSecurityRequirementOperationFilter : IOperationFilter
    {
        private readonly string _appIdUri;

        public OAuthSecurityRequirementOperationFilter(IOptions<AadAuthenticationConfiguration> authenticationOptions)
        {
            _appIdUri = authenticationOptions.Value.ApplicationIdUri;
        }

        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            // Check for authorize attribute
            if (context.MethodInfo.DeclaringType != null)
            {
                var authAttributes = context.MethodInfo.DeclaringType.GetCustomAttributes(true)
                    .Union(context.MethodInfo.GetCustomAttributes(true))
                    .OfType<AuthorizeAttribute>();

                // no auth attributes -> no need to authenticate request
                if (!authAttributes.Any()) return;
            }
            else
            {
                // could not get context
                return;
            }

            // Add security requirement to operation
            operation.Responses.Add("401", new OpenApiResponse {Description = "Unauthorized"});
            operation.Security = new List<OpenApiSecurityRequirement>()
            {
                new OpenApiSecurityRequirement()
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            },
                            Scheme = "oauth2",
                            Name = "Bearer",
                            In = ParameterLocation.Header,
                        },
                        new List<string>()
                    }
                }
            };
        }
    }
}