using ExpectedObjects;
using Machine.Specifications;
using NetStreams.Internal;
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

        internal void ShouldContainAtleastOne<T>(ExpectedObject expectedTelemetryEvent)
        {
            var actual = TelemetryEvents.FirstOrDefault(x => x.EventName == typeof(T).Name);
            expectedTelemetryEvent.ShouldMatch(actual);
        }

        internal void VerifyHeartBeatEvents(int expectedNumberOfHeartBeats, TimeSpan expectedDuration)
        {
            var events = TelemetryEvents.Where(x => x.EventName == typeof(StreamHeartBeat).Name).OrderBy(o => o.OccurredOn).ToList();

            for (var i = 1; i < events.Count; i++)
            {
                var previous = events[i - 1];
                var current = events[i];

                TimeSpan actual_duration = current.OccurredOn - previous.OccurredOn;

                actual_duration.ShouldBeCloseTo(expectedDuration, TimeSpan.FromMilliseconds(20));
            }

            events.Count.ShouldBeGreaterThanOrEqualTo(expectedNumberOfHeartBeats);


        }
    }
}
