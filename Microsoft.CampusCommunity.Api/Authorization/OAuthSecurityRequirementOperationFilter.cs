using Microsoft.AspNetCore.Authorization;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.CampusCommunity.Api.Models;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;

namespace Microsoft.CampusCommunity.Api.Authorization
{
    internal class OAuthSecurityRequirementOperationFilter : IOperationFilter
    {
        private readonly string _appIdUri;
        
        public OAuthSecurityRequirementOperationFilter(IOptions<ConfigurationAuthenticationOptions> authenticationOptions)
        {
            _appIdUri = authenticationOptions.Value.ApplicationIdUri;
        }

        //public void Apply(OpenApiOperation operation, OperationFilterContext context)
        //{
        //    // Get custom attributes on action and controller
        //    object[] attributes = context.ApiDescription.CustomAttributes().ToArray();
        //    if (attributes.OfType<AllowAnonymousAttribute>().Any())
        //    {
        //        // Controller / action allows anonymous calls
        //        return;
        //    }

        //    AuthorizeAttribute[] authorizeAttributes = attributes.OfType<AuthorizeAttribute>().ToArray();
        //    if (authorizeAttributes.Length == 0)
        //    {
        //        return;
        //    }

        //    // Policy name is always an action defined in Actions
        //    // Resolve the actions, from them derive the accepted scopes, get distinct values
        //    string[] scopes = authorizeAttributes
        //        .Select(attr => attr.Policy)
        //        .SelectMany(action => AuthorizedPermissions.DelegatedPermissionsForActions[action])
        //        .Distinct()
        //        .Select(scope => $"{_appIdUri}/{scope}") // Combine scope id with app id URI to form full scope id
        //        .ToArray();

        //    operation.Security.Add(new OpenApiSecurityRequirement
        //    {
        //        {
        //            new OpenApiSecurityScheme
        //            {
        //                Reference = new OpenApiReference
        //                {
        //                    Type = ReferenceType.SecurityScheme,
        //                    Id = "aad-jwt"
        //                },
        //                UnresolvedReference = true
        //            },
        //            scopes
        //        }
        //    });
        //}

        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            // Check for authorize attribute
            var authAttributes = context.MethodInfo.DeclaringType.GetCustomAttributes(true)
                .Union(context.MethodInfo.GetCustomAttributes(true))
                .OfType<AuthorizeAttribute>();

            if (authAttributes.Any())
            {
                operation.Responses.Add("401", new OpenApiResponse { Description = "Unauthorized" });
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
}
