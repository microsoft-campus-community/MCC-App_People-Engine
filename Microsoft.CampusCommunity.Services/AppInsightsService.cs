using System;
using System.Collections.Generic;
using Microsoft.ApplicationInsights;
using Microsoft.AspNetCore.Http;
using Microsoft.CampusCommunity.Infrastructure.Helpers;
using Microsoft.CampusCommunity.Infrastructure.Interfaces;
using Microsoft.Extensions.Configuration;

namespace Microsoft.CampusCommunity.Services
{
    public class AppInsightsService : IAppInsightsService
    {
        private TelemetryClient _telemetry;
        private const string AppInsightsInstrumentationKeyConfigurationKey = "AppInsightsInstrumentationKey";

        public AppInsightsService(IConfiguration configuration, TelemetryClient telemetry)
        {
            // _telemetry = new TelemetryClient(new ApplicationInsights.Extensibility.TelemetryConfiguration {
            // 	InstrumentationKey = GetInstrumentationKey(configuration)
            // });
            _telemetry = telemetry;
        }

        private string GetInstrumentationKey(IConfiguration configuration)
        {
            return configuration[AppInsightsInstrumentationKeyConfigurationKey];
        }

        public void TrackEvent(string eventName, string eventMessage)
        {
            _telemetry.TrackEvent(eventName, new Dictionary<string, string>() {{"message", eventMessage}});
        }

        public void TrackException(HttpContext context, Exception exception, Guid appInsightsTrackingId)
        {
            string userId = "?";
            string requestPath = "?";
            string method = "?";
            if (context != null)
            {
                userId = AuthenticationHelper.GetUserIdFromToken(context.User).ToString();
                requestPath = context.Request.Path;
                method = context.Request.Method;
            }


            var properties = new Dictionary<string, string>()
            {
                {"trackingId", appInsightsTrackingId.ToString()},
                {"userId", userId},
                {"request", requestPath},
                {"method", method}
            };
            _telemetry.TrackException(exception, properties);
        }

        public void TrackMetric(string metric, double value)
        {
            _telemetry.GetMetric(metric).TrackValue(value);
        }

        public void TrackMetric(string metric, int value)
        {
            _telemetry.GetMetric(metric).TrackValue(value);
        }

        // public void TrackRequest(HttpContext context)
        // {
        // 	_telemetry.TrackRequest()
        // }
    }
}