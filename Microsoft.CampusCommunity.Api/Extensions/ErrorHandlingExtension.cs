using Microsoft.AspNetCore.Builder;
using Microsoft.CampusCommunity.Infrastructure.Interfaces;
using Microsoft.CampusCommunity.Infrastructure.Middleware;

namespace Microsoft.CampusCommunity.Api.Extensions
{
    internal static class ErrorHandlingMiddlewareExtensions
    {
        public static IApplicationBuilder UseExceptionMiddleware(this IApplicationBuilder builder, IAppInsightsService appInsightsService)
        {
            var middleware = new ExceptionHandlingMiddleware(appInsightsService);
            return builder.UseExceptionHandler(appError =>
            {
                appError.Run(async context => { await middleware.InvokeAsync(context); });
            });
        }
    }
}