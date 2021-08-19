using ExpectedObjects;
using NetStreams.Configuration;
using NetStreams.Internal;
using NetStreams.Telemetry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetStreams.Specs.Infrastructure.Mothers
{
    class ExpectedTelemetryEventMother
    {
        internal static Dictionary<Type, ExpectedObject> GetExpectedEvents<TKey, TMessage>(string topic, NetStreamConfiguration<TKey, TMessage> configuration)
        {
            return new Dictionary<Type, ExpectedObject>
            {
                {
                    typeof(StreamStarted),
                    new {
                        Id = Expect.NotDefault<Guid>(),
                        OccurredOn = Expect.NotDefault<DateTimeOffset>(),
                        EventName = typeof(StreamStarted).FullName,
                        StreamProcessorName = "TestProcessor",
                        Source = topic,
                        Configuration = configuration
                    }.ToExpectedObject()
                },
                {
                    typeof(StreamStopped),
                    new {
                        Id = Expect.NotDefault<Guid>(),
                        OccurredOn = Expect.NotDefault<DateTimeOffset>(),
                        EventName = typeof(StreamStopped).FullName,
                        StreamProcessorName = "TestProcessor"
                    }.ToExpectedObject()
                 },
                {
                    typeof(MessageProcessingStarted<TKey, TMessage>),
                    new {
                        Id = Expect.NotDefault<Guid>(),
                        OccurredOn = Expect.NotDefault<DateTimeOffset>(),
                        EventName = typeof(MessageProcessingStarted<TKey, TMessage>).FullName,
                        StreamProcessorName = "TestProcessor"
                    }.ToExpectedObject()
                },
                {
                    typeof(MessageProcessingCompleted<TKey, TMessage>),
                    new {
                        Id = Expect.NotDefault<Guid>(),
                        OccurredOn = Expect.NotDefault<DateTimeOffset>(),
                        EventName = typeof(MessageProcessingCompleted<TKey, TMessage>).FullName,
                        StreamProcessorName = "TestProcessor"
                    }.ToExpectedObject()
                }
            };
        }
    }
}
