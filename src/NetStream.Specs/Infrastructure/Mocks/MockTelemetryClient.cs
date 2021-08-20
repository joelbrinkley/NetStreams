using ExpectedObjects;
using Machine.Specifications;
using NetStreams.Telemetry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NetStreams.Specs.Infrastructure.Mocks
{
    internal class MockTelemetryClient : INetStreamTelemetryClient
    {
        public List<NetStreamsTelemetryEvent> TelemetryEvents = new List<NetStreamsTelemetryEvent>();

        public Task SendAsync(NetStreamsTelemetryEvent telemetryEvent, CancellationToken token)
        {
            TelemetryEvents.Add(telemetryEvent);
            return Task.CompletedTask;
        }

        public void ShouldContainOnlyOne<T>(ExpectedObject expectedTelemetryEvent)
        {
            var actual = TelemetryEvents.SingleOrDefault(x => x.EventName == typeof(T).Name);

            expectedTelemetryEvent.ShouldMatch(actual);
        }
    }
}
