using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.CampusCommunity.Infrastructure.Entities.Dto;
using Microsoft.CampusCommunity.Infrastructure.Exceptions;
using Microsoft.CampusCommunity.Infrastructure.Interfaces;
using Newtonsoft.Json;

namespace Microsoft.CampusCommunity.Infrastructure.Middlewares {
	public class ExceptionHandlingMiddleware : IMiddleware
	{
		private readonly RequestDelegate _next;
		private readonly IAppInsightsService _appInsightsService;

		public ExceptionHandlingMiddleware(RequestDelegate NextDelegate, IAppInsightsService service) {
			_next = NextDelegate;
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
			int errorCode = -1;
			string message = exception.Message;
			string trace = exception.StackTrace;

			switch (exception) {
				case MccNotAuthenticatedException _:
					errorCode = (int)HttpStatusCode.Unauthorized;
					break;
				default:
					errorCode = (int)HttpStatusCode.InternalServerError;
					break;
			}

			var responseBody = WriteException(errorCode, message, trace, appInsightsTrackingId);
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)errorCode;
            return context.Response.WriteAsync(responseBody);
		}

		private string WriteException(int errorCode, string message, string trace, Guid appInsightsTrackingId)
		{
			var errorWrapper = new ExceptionClientWrapper{
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