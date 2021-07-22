using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Confluent.Kafka;
using ExpectedObjects;
using Machine.Specifications;
using NetStreams.Configuration;
using NetStreams.Specs.Infrastructure;
using NetStreams.Specs.Infrastructure.Extensions;
using NetStreams.Specs.Infrastructure.Mocks;
using NetStreams.Specs.Infrastructure.Models;
using NetStreams.Specs.Infrastructure.Services;

namespace NetStreams.Specs.Specifications.Integration
{
    internal class ConsumeSpecs
    {
        [Subject("Malformed Message")]
        class when_consuming_a_malformed_message
        {
            static readonly string _sourceTopic = $"malform.{Guid.NewGuid()}";
            static readonly string _destinationTopic = $"malform.{Guid.NewGuid()}";
            static readonly List<TestMessage> _actualMessages = new();
            static TestMessage _expectedMessage;
            static TestProducerService<string, TestMessage> _testMessageProducer;
            static MockLog _mockLogger;

            static string _expectedLogMessage;

            Establish context = () =>
            {
                new TopicService().CreateAll(_sourceTopic, _destinationTopic);

                var stringProducer = new TestProducerService<string, string>(_sourceTopic);
                _testMessageProducer = new TestProducerService<string, TestMessage>(_sourceTopic);

                stringProducer.Produce(Guid.NewGuid().ToString(), "{ malformed }");

                _mockLogger = new MockLog();
                new NetStreamBuilder<string, TestMessage>(cfg =>
                    {
                        cfg.BootstrapServers = "localhost:9092";
                        cfg.ConsumerGroup = Guid.NewGuid().ToString();
                        cfg.AutoOffsetReset = AutoOffsetReset.Earliest;
                        cfg.ConfigureLogging(logging => logging.AddLogger(_mockLogger));
                    })
                    .Stream(_sourceTopic)
                    .ToTopic<string, TestMessage>(_destinationTopic)
                    .Build()
                    .StartAsync(CancellationToken.None);

                _expectedLogMessage =
                    $"A malformed message was encountered on topic { _sourceTopic}. Skipping message. Skipping offset 0 on partition 0";

                _expectedMessage = new TestMessage { Description = "hello" };

                DefaultBuilder.New<string, TestMessage>()
                    .Stream(_destinationTopic)
                    .Handle(context => _actualMessages.Add(context.Message))
                    .Build()
                    .StartAsync(CancellationToken.None);
            };

            Because of = () =>
                _testMessageProducer.ProduceAsync(_expectedMessage.Id, _expectedMessage).BlockUntil(() => _actualMessages.Count == 1).Await();

            It should_log_skip_message_information = () => _mockLogger.ShouldContain(_expectedLogMessage);

            It should_skip_malformed_message =
                () => _expectedMessage.ToExpectedObject().ShouldMatch(_actualMessages[0]);
        }

        [Subject("Auto Commit")]
        class when_consuming_a_message_with_auto_commit_enabled
        {
            static readonly string _sourceTopic = $"ac.{Guid.NewGuid()}";
            static readonly string _destinationTopic = $"ac.{Guid.NewGuid()}";
            static TestProducerService<string, TestMessage> _producer;
            static CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
            static long? _resumedOffset;
            static long? _initialOffset;

            Establish context = () =>
            {
                new TopicService().CreateAll(_sourceTopic, _destinationTopic);

                _producer = new TestProducerService<string, TestMessage>(_sourceTopic);

                var firstTestMessage = new TestMessage();
                var consumerGroup = Guid.NewGuid().ToString();
                var stream = new NetStreamBuilder<string, TestMessage>(cfg =>
                {
                    cfg.BootstrapServers = "localhost:9092";
                    cfg.ConsumerGroup = consumerGroup;
                    cfg.AutoOffsetReset = AutoOffsetReset.Earliest;
                    cfg.DeliveryMode = DeliveryMode.At_Least_Once;
                })
                .Stream(_sourceTopic)
                .Handle(context => _initialOffset = context.Offset)
                .ToTopic<string, TestMessage>(_destinationTopic)
                .Build();

                var streamTask = stream.StartAsync(_cancellationTokenSource.Token);

                _producer.ProduceAsync(Guid.NewGuid().ToString(), firstTestMessage).BlockUntil(() => _initialOffset != null).Await();

                stream.Stop();

                streamTask.BlockUntil(() => streamTask.Status == TaskStatus.RanToCompletion).Await();

                new NetStreamBuilder<string, TestMessage>(cfg =>
                {
                    cfg.BootstrapServers = "localhost:9092";
                    cfg.ConsumerGroup = consumerGroup;
                    cfg.AutoOffsetReset = AutoOffsetReset.Earliest;
                    cfg.DeliveryMode = DeliveryMode.At_Least_Once;
                })
                .Stream(_sourceTopic)
                .Handle(context => _resumedOffset = context.Offset)
                .ToTopic<string, TestMessage>(_destinationTopic)
                .Build()
                .StartAsync(CancellationToken.None);
            };

            Because of = () => _producer.ProduceAsync(Guid.NewGuid().ToString(), new TestMessage()).BlockUntil(() => _resumedOffset != null).Await();

            It should_commit_the_offset = () => _resumedOffset.ShouldBeGreaterThan(_initialOffset);
        }

        [Subject("Consume Failure")]
        class when_consuming_a_message_and_configured_to_continue_on_error
        {
            static readonly string _sourceTopic = $"f.{Guid.NewGuid()}";
            static readonly string _destinationTopic = $"f.{Guid.NewGuid()}";
            static TestProducerService<string, TestMessage> _producer;
            static List<TestMessage> _consumedMessages = new List<TestMessage>();
            Establish context = () =>
            {
                new TopicService().CreateAll(_sourceTopic, _destinationTopic);

                _producer = new TestProducerService<string, TestMessage>(_sourceTopic);

                new NetStreamBuilder<string, TestMessage>(cfg =>
                {
                    cfg.BootstrapServers = "localhost:9092";
                    cfg.ConsumerGroup = Guid.NewGuid().ToString();
                    cfg.AutoOffsetReset = AutoOffsetReset.Earliest;
                    cfg.ContinueOnError = true;
                })
                .Stream(_sourceTopic)
                .Handle(context =>
                {
                    _consumedMessages.Add(context.Message);
                    throw new Exception("Bang!");
                })
                .ToTopic<string, TestMessage>(_destinationTopic)
                .Build()
                .StartAsync(CancellationToken.None);

                _producer.Produce(Guid.NewGuid().ToString(), new TestMessage());
            };

            Because of = () => _producer.ProduceAsync(Guid.NewGuid().ToString(), new TestMessage()).BlockUntil(() => _consumedMessages.Count == 2).Await();

            It should_continue_processing_the_next_message = () => _consumedMessages.Count.ShouldEqual(2);
        }

        [Subject("Consume Failure")]
        class when_consuming_a_message_and_configured_to_stop_on_error
        {
            static readonly string _sourceTopic = $"f.{Guid.NewGuid()}";
            static readonly string _destinationTopic = $"f.{Guid.NewGuid()}";
            static TestProducerService<string, TestMessage> _producer;
            static INetStream _stream;
            static List<TestMessage> _consumedMessages = new List<TestMessage>();
            static Task _streamTask;

            Establish context = () =>
            {
                new TopicService().CreateAll(_sourceTopic, _destinationTopic);

                _producer = new TestProducerService<string, TestMessage>(_sourceTopic);
                _stream = new NetStreamBuilder<string, TestMessage>(cfg =>
                        {
                            cfg.BootstrapServers = "localhost:9092";
                            cfg.ConsumerGroup = Guid.NewGuid().ToString();
                            cfg.AutoOffsetReset = AutoOffsetReset.Earliest;
                            cfg.ContinueOnError = false;
                        })
                        .Stream(_sourceTopic)
                        .Handle(context =>
                        {
                            _consumedMessages.Add(context.Message);
                            throw new Exception("Bang!");
                        })
                        .ToTopic<string, TestMessage>(_destinationTopic)
                        .Build();

                _producer.Produce(Guid.NewGuid().ToString(), new TestMessage());
                _producer.Produce(Guid.NewGuid().ToString(), new TestMessage());

                _streamTask = _stream.StartAsync(CancellationToken.None);
            };

            Because of = () => _streamTask.Wait();

            It should_not_continue_processing_the_next_message = () => _consumedMessages.Count.ShouldEqual(1);

            It should_stop_the_stream = () => _stream.Status.ShouldEqual(NetStreamStatus.Stopped);

            It should_complete_the_stream_task = () => _streamTask.Status.ShouldEqual(TaskStatus.RanToCompletion);
        }

        [Subject("Consume")]
        class when_starting_a_stopped_stream
        {
            static readonly string _sourceTopic = $"r.{Guid.NewGuid()}";
            static readonly string _destinationTopic = $"r.{Guid.NewGuid()}";
            static TestProducerService<string, TestMessage> _producer;
            static INetStream _stream;
            static List<TestMessage> _consumedMessages = new List<TestMessage>();
            static Task _streamTask;

            Establish context = () =>
            {
                new TopicService().CreateAll(_sourceTopic, _destinationTopic);

                _producer = new TestProducerService<string, TestMessage>(_sourceTopic);

                var firstTestMessage = new TestMessage();

                var stream = DefaultBuilder.New<string, TestMessage>()
                .Stream(_sourceTopic)
                .Handle(context => _consumedMessages.Add(context.Message))
                .ToTopic<string, TestMessage>(_destinationTopic)
                .Build();

                var streamTask = stream.StartAsync(CancellationToken.None);

                _producer.ProduceAsync(Guid.NewGuid().ToString(), firstTestMessage).BlockUntil(() => _consumedMessages.Count == 1).Await();

                stream.Stop();

                streamTask.BlockUntil(() => streamTask.Status == TaskStatus.RanToCompletion).Await();

                stream.StartAsync(CancellationToken.None);

            };
            Because of = () => _producer.ProduceAsync(Guid.NewGuid().ToString(), new TestMessage()).BlockUntil(() => _consumedMessages.Count == 2).Await();

            It should_resume_consuming_messages = () => _consumedMessages.Count.ShouldEqual(2);
        }
    }
}