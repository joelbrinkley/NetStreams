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
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using It = Machine.Specifications.It;

namespace NetStreams.Specs.Specifications.Component
{
    class NetStreamTelemetrySpecs
    {
        [Subject("Telemetry")]
        class when_the_starting_a_stream
        {
            private static MockTelemetryClient _mockTelemetryClient;
            private static NetStream<string, TestMessage> _netStream;
            private static ExpectedObject _expectedTelemetryEvent;

            Establish context = () =>
            {
                var mockConsumer = new Mock<IConsumer<string, TestMessage>>();

                mockConsumer.Setup(x => x.Consume(Parameter.IsAny<int>()))
                    .Returns((ConsumeResult<string, TestMessage>)null);

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

                _expectedTelemetryEvent = new
                {
                    Id = Expect.NotDefault<Guid>(),
                    OccurredOn = Expect.NotDefault<DateTimeOffset>(),
                    EventName = typeof(StreamStarted).FullName,
                    StreamProcessorName = "TestProcessor",
                    Source = topic,
                    Configuration = configuration
                }.ToExpectedObject();
            };

            Because of = () => _netStream.StartAsync(CancellationToken.None);

            It should_send_stream_started = () => _mockTelemetryClient.ShouldContainOnlyOne<StreamStarted>(_expectedTelemetryEvent);
        }

        [Subject("Telemetry")]
        class when_the_stopping_a_straem
        {
            private static MockTelemetryClient _mockTelemetryClient;
            private static NetStream<string, TestMessage> _netStream;
            private static ExpectedObject _expectedTelemetryEvent;

            Establish context = () =>
            {
                var mockConsumer = new Mock<IConsumer<string, TestMessage>>();

                mockConsumer.Setup(x => x.Consume(Parameter.IsAny<int>()))
                    .Returns((ConsumeResult<string, TestMessage>)null);

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

                _expectedTelemetryEvent = new
                {
                    Id = Expect.NotDefault<Guid>(),
                    OccurredOn = Expect.NotDefault<DateTimeOffset>(),
                    EventName = typeof(StreamStopped).FullName,
                    StreamProcessorName = "TestProcessor"
                }.ToExpectedObject();

                _netStream.StartAsync(CancellationToken.None);
            };

            Because of = () => _netStream.StopAsync(CancellationToken.None).Await();

            It should_send_stream_stopped_event = () => _mockTelemetryClient.ShouldContainOnlyOne<StreamStopped>(_expectedTelemetryEvent);
        }
    }
}
