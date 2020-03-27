using System;
using Microsoft.AspNetCore.Http;

namespace Microsoft.CampusCommunity.Infrastructure.Interfaces
{
    public interface IAppInsightsService
    {
        void TrackException(HttpContext context, Exception exception, Guid appInsightsTrackingId);
        void TrackEvent(string eventName, string eventMessage);
        void TrackMetric(string metric, double value);

        void TrackMetric(string metric, int value);
        // Task TrackRequest();
    }
}