using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.CampusCommunity.Infrastructure.Entities.Dto;
using Microsoft.CampusCommunity.Infrastructure.Exceptions;
using Microsoft.CampusCommunity.Infrastructure.Interfaces;
using Newtonsoft.Json;

namespace Microsoft.CampusCommunity.Infrastructure.Middlewares
{
    public class ExceptionHandlingMiddleware : IMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IAppInsightsService _appInsightsService;

        public ExceptionHandlingMiddleware(RequestDelegate nextDelegate, IAppInsightsService service)
        {
            _next = nextDelegate;
            _appInsightsService = service;
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            // global try catch to catch all exceptions
            try
            {
                await _next(context);
            }
            catch (Exception exception)
            {
                // create guid that will be used as a reference Id for app insights tracking
                var appInsightsTrackingId = Guid.NewGuid();
                await HandleExceptions(context, exception, appInsightsTrackingId);
                LogExceptions(context, appInsightsTrackingId, exception);
            }
        }

        private void LogExceptions(HttpContext context, Guid appInsightsTrackingId, Exception e)
        {
            _appInsightsService.TrackException(context, e, appInsightsTrackingId);
        }

        private Task HandleExceptions(HttpContext context, Exception exception, Guid appInsightsTrackingId)
        {
            var message = exception.Message;
            var trace = exception.StackTrace;

            var errorCode = exception switch
            {
                MccNotAuthenticatedException _ => (int) HttpStatusCode.Unauthorized,
                MccNotAuthorizedException _ => (int) HttpStatusCode.Forbidden,
                MccBadRequestException _ => (int) HttpStatusCode.BadRequest,
                MccNotFoundException _ => (int) HttpStatusCode.NotFound,
                MccExceptionBase _ => (int) HttpStatusCode.InternalServerError,
                _ => (int) HttpStatusCode.InternalServerError
            };

            var responseBody = WriteException(errorCode, message, trace, appInsightsTrackingId);
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = errorCode;
            return context.Response.WriteAsync(responseBody);
        }

        /// <summary>
        /// Serializes an exception so that it can be returned
        /// </summary>
        /// <param name="errorCode"></param>
        /// <param name="message"></param>
        /// <param name="trace"></param>
        /// <param name="appInsightsTrackingId"></param>
        /// <returns></returns>
        private string WriteException(int errorCode, string message, string trace, Guid appInsightsTrackingId)
        {
            var errorWrapper = new ExceptionClientWrapper
            {
                ErrorCode = errorCode,
                ExceptionMessage = message,
                ExceptionTrace = trace,
                ExceptionAppInsightsId = appInsightsTrackingId
            };

            var serializedError = JsonConvert.SerializeObject(errorWrapper);
            return serializedError;
        }
    }
}