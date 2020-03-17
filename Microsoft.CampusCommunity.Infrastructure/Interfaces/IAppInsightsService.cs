using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Microsoft.CampusCommunity.Infrastructure.Interfaces {
	public interface IAppInsightsService {
		Task TrackException(HttpContext context, Exception exception, Guid appInsightsTrackingId);
		Task TrackEvent(string eventMessage);
		Task TrackMetric(string metric, double value);
		Task TrackMetric(string metric, int value);
		Task TrackRequest();
	}
}