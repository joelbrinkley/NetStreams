using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;
using NetStreams.Telemetry;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NetStreams.ApplicationInsights
{
    public class AppInsightsTelemetryClient : INetStreamTelemetryClient
    {
        TelemetryClient _telemetryClient;

        public AppInsightsTelemetryClient(TelemetryClient telemetryClient)
        {
            _telemetryClient = telemetryClient;
        }

        public Task SendAsync(NetStreamsTelemetryEvent telemetryEvent, CancellationToken token)
        {
            _telemetryClient.TrackEvent(new EventTelemetry()
            {
                Name = telemetryEvent.EventName
            });

            return Task.CompletedTask;
        }
    }
}
