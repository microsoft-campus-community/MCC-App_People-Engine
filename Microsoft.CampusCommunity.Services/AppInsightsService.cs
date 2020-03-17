// using System;
// using System.Threading.Tasks;
// using Microsoft.ApplicationInsights;
// using Microsoft.AspNetCore.Http;
// using Microsoft.CampusCommunity.Infrastructure.Interfaces;
// using Microsoft.Extensions.Configuration;

// namespace Microsoft.CampusCommunity.Services {
// 	public class AppInsightsService : IAppInsightsService
// 	{
// 		private TelemetryClient _telemetry;
// 		private const string AppInsightsInstrumentationKeyConfigurationKey = 'AppInsightsInstrumentationKey'; 

// 		public AppInsightsService(IConfiguration configuration) {
// 			_telemetry = new TelemetryClient()
// 			_telemetry.InstrumentationKey = GetInstrumentationKey(configuration);
// 		}

// 		private string GetInstrumentationKey(IConfiguration configuration) {
// 			return configuration[AppInsightsInstrumentationKeyConfigurationKey];
// 		}

// 		public Task TrackEvent(string eventMessage)
// 		{
// 			throw new NotImplementedException();
// 		}

// 		public Task TrackException(HttpContext context, Exception exception, Guid appInsightsTrackingId)
// 		{
// 			telemetry.TrackException(ex);
// 		}

// 		public Task TrackMetric(string metric, double value)
// 		{
// 			_telemetry.GetMetric(metric).TrackValue(value);
// 		}

// 		public Task TrackMetric(string metric, int value)
// 		{
// 			_telemetry.GetMetric(metric).TrackValue(value);
// 		}

// 		public Task TrackRequest()
// 		{
// 			throw new NotImplementedException();
// 		}
// 	}
// }