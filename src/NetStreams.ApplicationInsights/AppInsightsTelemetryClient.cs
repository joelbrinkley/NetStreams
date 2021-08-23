using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;
using NetStreams.Serialization;
using NetStreams.Telemetry;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace NetStreams.ApplicationInsights
{
    public class AppInsightsTelemetryClient : INetStreamTelemetryClient
    {
        TelemetryClient _telemetryClient;
        readonly IJsonSerializer _jsonSerializer;

        public AppInsightsTelemetryClient(TelemetryClient telemetryClient, IJsonSerializer jsonSerializer)
        {
            _telemetryClient = telemetryClient;
            _jsonSerializer = jsonSerializer;
        }

        public Task SendAsync(NetStreamsTelemetryEvent telemetryEvent, CancellationToken token)
        {
            var @event = new EventTelemetry()
            {
                Name = telemetryEvent.EventName
            };

            foreach (var prop in telemetryEvent.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                @event.Properties.Add(prop.Name, _jsonSerializer.Serialize(prop.GetValue(telemetryEvent)));
            }

            _telemetryClient.TrackEvent(@event);

            return Task.CompletedTask;
        }
    }
}
