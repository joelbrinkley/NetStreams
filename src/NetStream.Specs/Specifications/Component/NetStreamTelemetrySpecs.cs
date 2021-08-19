using Confluent.Kafka;
using ExpectedObjects;
using Machine.Specifications;
using Moq;
using NetStreams.Configuration;
using NetStreams.Internal;
using NetStreams.Specs.Infrastructure;
using NetStreams.Specs.Infrastructure.Extensions;
using NetStreams.Specs.Infrastructure.Mocks;
using NetStreams.Specs.Infrastructure.Models;
using NetStreams.Specs.Infrastructure.Mothers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using It = Machine.Specifications.It;

namespace NetStreams.Specs.Specifications.Component
{
    class NetStreamTelemetrySpecs
    {
        [Subject("Telemetry")]
        class when_the_consuming_a_message
        {
            private static MockTelemetryClient _mockTelemetryClient;
            private static NetStream<string, TestMessage> _netStream;
            private static Dictionary<Type, ExpectedObject> _expectedTelemetryEvents;

            Establish context = () =>
            {
                var mockConsumer = MockConsumer.SetupToConsumeSingleTestMessage();

                _mockTelemetryClient = new MockTelemetryClient();

                var topic = Guid.NewGuid().ToString();

                var configuration = new NetStreamConfiguration<string, TestMessage>();

                _netStream = new NetStream<string, TestMessage>(
                   topic,
                    configuration,
                    mockConsumer.Object,
                    new NullTopicCreator(),
                    _mockTelemetryClient,
                    null,
                    null,
                    "TestProcessor");

                _expectedTelemetryEvents = ExpectedTelemetryEventMother.GetExpectedEvents(topic, configuration);

                _netStream.StartAsync(CancellationToken.None).BlockUntil(() => _mockTelemetryClient.TelemetryEvents.Count >= 4).Await();
            };

            Because of = () => _netStream.StopAsync(CancellationToken.None).Await();

            It should_send_stream_started_event = () => _mockTelemetryClient.ShouldContainOnlyOne<StreamStarted>(_expectedTelemetryEvents);

            It should_send_stream_stopped_event = () => _mockTelemetryClient.ShouldContainOnlyOne<StreamStopped>(_expectedTelemetryEvents);

            It should_send_processing_started_event = () => _mockTelemetryClient.ShouldContainOnlyOne<MessageProcessingStarted<string, TestMessage>>(_expectedTelemetryEvents);

            It should_send_processing_completed_event = () => _mockTelemetryClient.ShouldContainOnlyOne<MessageProcessingCompleted<string, TestMessage>>(_expectedTelemetryEvents);
        }
    }
}

