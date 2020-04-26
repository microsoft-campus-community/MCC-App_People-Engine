using Microsoft.AspNetCore.Builder;
using Microsoft.CampusCommunity.Infrastructure.Interfaces;
using Microsoft.CampusCommunity.Infrastructure.Middleware;

namespace Microsoft.CampusCommunity.Api.Extensions
{
    internal static class ErrorHandlingMiddlewareExtensions
    {
        public static IApplicationBuilder UseExceptionMiddleware(this IApplicationBuilder builder, IAppInsightsService appInsightsService)
        {
            return builder.UseExceptionHandler(appError =>
            {
                appError.Use(async (context, next) =>
                {
                    var middleware = new ExceptionHandlingMiddleware(appInsightsService);
                    await middleware.InvokeAsync(context, next);
                });
            });
        }
    }
}