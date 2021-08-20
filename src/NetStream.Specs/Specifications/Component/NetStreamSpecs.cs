﻿using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
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
using It = Machine.Specifications.It;

namespace NetStreams.Specs.Specifications.Component
{
    internal class NetStreamSpecs
    {
        [Subject("Start")]
        class when_the_start_task_is_canceled
        {
            static CancellationTokenSource _tokenSource;
            static Task _startTask;

            Establish context = () =>
            {
                _tokenSource = new CancellationTokenSource();

                var mockConsumer = new Mock<IConsumer<string, TestMessage>>();

                mockConsumer.Setup(x => x.Consume(Parameter.IsAny<int>()))
                    .Returns((ConsumeResult<string, TestMessage>)null);

                var netStream = new NetStream<string, TestMessage>(
                    Guid.NewGuid().ToString(),
                    new NetStreamConfiguration<string, TestMessage>(),
                    mockConsumer.Object,
                    new NullTopicCreator(),
                    new MockLog());

                _startTask = netStream.StartAsync(_tokenSource.Token);
            };

            Because of = () =>
                Task.Run(() => _tokenSource.Cancel()).BlockUntil(() => _startTask.Status == TaskStatus.RanToCompletion)
                    .Await();

            It should_run_to_completion = () => _startTask.Status.ShouldEqual(TaskStatus.RanToCompletion);
        }

        [Subject("Start")]
        class when_starting_a_stream
        {
            static MockTelemetryClient _mockTelemetryClient;
            static NetStream<string, TestMessage> _netStream;
            static ExpectedObject _expectedStartEvent;

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
                    new MockLog(),
                    _mockTelemetryClient,
                    null,
                    null,
                    "TestProcessor");

                _expectedStartEvent = new
                {
                    Id = Expect.NotDefault<Guid>(),
                    OccurredOn = Expect.NotDefault<DateTimeOffset>(),
                    EventName = typeof(StreamStarted).Name,
                    FullName = typeof(StreamStarted).FullName,
                    StreamProcessorName = "TestProcessor",
                    Source = topic,
                    Configuration = configuration
                }.ToExpectedObject();
            };

            Because of = () => _netStream.StartAsync(CancellationToken.None);

            Cleanup after = () => _netStream.StopAsync(CancellationToken.None).Await();

            It should_send_stream_started_event = () => _mockTelemetryClient.ShouldContainOnlyOne<StreamStarted>(_expectedStartEvent);
        }

        [Subject("ErrorHandling")]
        class when_an_error_occurs_while_streaming_with_an_onerror
        {
            static NetStream<string, TestMessage> _stream;
            static CancellationTokenSource _cancellationTokenSource = new();
            static Mock<IConsumer<string, TestMessage>> _mockConsumer;
            private static MockTelemetryClient _mockTelemetryClient;
            static ExpectedObject _expectedException;
            static Exception _actualException;
            static Task _streamTask;
            static ExpectedObject _expectedTelemetryEvent;

            Establish context = () =>
            {
                var exceptionToThrow = new Exception("Boom!");
                _expectedException = exceptionToThrow.ToExpectedObject();

                _mockConsumer = MockConsumer.SetupToThrowError(exceptionToThrow);
                _mockTelemetryClient = new MockTelemetryClient();

                var sourceTopic = Guid.NewGuid().ToString();
                var configuration = new NetStreamConfiguration<string, TestMessage>
                {
                    DeliveryMode = DeliveryMode.At_Least_Once
                };

                _stream = new NetStream<string, TestMessage>(
                    sourceTopic,
                    configuration,
                    _mockConsumer.Object,
                    new NullTopicCreator(),
                    new MockLog(),
                    _mockTelemetryClient,
                    null,
                    ex =>
                    {
                        _actualException = ex;
                        _cancellationTokenSource.Cancel();
                    },
                    "TestProcessor");

                _expectedTelemetryEvent = new
                {
                    Id = Expect.NotDefault<Guid>(),
                    OccurredOn = Expect.NotDefault<DateTimeOffset>(),
                    EventName = typeof(NetStreamExceptionOccurred<string, TestMessage>).Name,
                    FullName = typeof(NetStreamExceptionOccurred<string, TestMessage>).FullName,
                    StreamProcessorName = "TestProcessor",
                    Exception = exceptionToThrow,
                    ConsumeContext = Expect.Null()
                }.ToExpectedObject();

                _streamTask = _stream.StartAsync(_cancellationTokenSource.Token);
            };

            Because of = () => _streamTask.BlockUntil(() => _actualException != null).Await();

            It should_call_on_error_with_exception = () => _expectedException.ShouldMatch(_actualException);

            It should_send_error_telemetry_event = () => _mockTelemetryClient.ShouldContainOnlyOne<NetStreamExceptionOccurred<string, TestMessage>>(_expectedTelemetryEvent);
        }

        [Subject("Consume")]
        class when_consuming_messages
        {
            private static MockTelemetryClient _mockTelemetryClient;
            private static NetStream<string, TestMessage> _netStream;
            private static ExpectedObject _expectedStartEvent;
            private static ExpectedObject _expectedCompletedEvent;
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
                    new MockLog(),
                    _mockTelemetryClient,
                    null,
                    null,
                    "TestProcessor");

                _expectedStartEvent = new
                {
                    Id = Expect.NotDefault<Guid>(),
                    OccurredOn = Expect.NotDefault<DateTimeOffset>(),
                    EventName = typeof(MessageProcessingStarted<string, TestMessage>).Name,
                    FullName = typeof(MessageProcessingStarted<string, TestMessage>).FullName,
                    StreamProcessorName = "TestProcessor"
                }.ToExpectedObject();

                _expectedCompletedEvent = new
                {
                    Id = Expect.NotDefault<Guid>(),
                    OccurredOn = Expect.NotDefault<DateTimeOffset>(),
                    EventName = typeof(MessageProcessingCompleted<string, TestMessage>).Name,
                    FullName = typeof(MessageProcessingCompleted<string, TestMessage>).FullName,
                    StreamProcessorName = "TestProcessor"
                }.ToExpectedObject();

                _netStream.StartAsync(CancellationToken.None);
            };

            Because of = () => Task.CompletedTask.BlockUntil(() => _mockTelemetryClient.TelemetryEvents.Any(x => x.GetType() == typeof(MessageProcessingCompleted<string, TestMessage>))).Await();

            Cleanup after = () => _netStream.StopAsync(CancellationToken.None).Await();

            It should_send_processing_started_event = () => _mockTelemetryClient.ShouldContainOnlyOne<MessageProcessingStarted<string, TestMessage>>(_expectedStartEvent);

            It should_send_processing_completed_event = () => _mockTelemetryClient.ShouldContainOnlyOne<MessageProcessingCompleted<string, TestMessage>>(_expectedCompletedEvent);
        }

        [Subject("Stop")]
        class when_stopping_a_stream
        {
            static MockTelemetryClient _mockTelemetryClient;
            static NetStream<string, TestMessage> _netStream;
            static ExpectedObject _expectedTelemetryEvent;

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
                    new MockLog(),
                    _mockTelemetryClient,
                    null,
                    null,
                    "TestProcessor");

                _expectedTelemetryEvent = new
                {
                    Id = Expect.NotDefault<Guid>(),
                    OccurredOn = Expect.NotDefault<DateTimeOffset>(),
                    EventName = typeof(StreamStopped).Name,
                    FullName = typeof(StreamStopped).FullName,
                    StreamProcessorName = "TestProcessor"
                }.ToExpectedObject();

                var task = _netStream.StartAsync(CancellationToken.None);

                Task.CompletedTask.BlockUntil(() => _mockTelemetryClient.TelemetryEvents.Any(x => x.GetType() == typeof(MessageProcessingCompleted<string, TestMessage>))).Await();
            };

            Because of = () => _netStream.StopAsync(CancellationToken.None).Await();

            It set_status_to_stopped = () => _netStream.Status.ShouldEqual(NetStreamStatus.Stopped);

            It should_send_stream_stopped_telemetry_event = () => _mockTelemetryClient.ShouldContainOnlyOne<StreamStopped>(_expectedTelemetryEvent);
        }
    }
}