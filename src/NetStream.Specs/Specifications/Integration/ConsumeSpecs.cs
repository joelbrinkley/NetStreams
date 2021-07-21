using System;
using System.Collections.Generic;
using System.Threading;
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
            static INetStreamBuilder<string, TestMessage> _streamBuilder;
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

                _expectedMessage = new TestMessage {Description = "hello"};

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
                new NetStreamBuilder<string, TestMessage>(cfg =>
                {
                    cfg.BootstrapServers = "localhost:9092";
                    cfg.ConsumerGroup = consumerGroup;
                    cfg.AutoOffsetReset = AutoOffsetReset.Earliest;
                    cfg.DeliveryMode = DeliveryMode.At_Least_Once;
                })
                .Stream(_sourceTopic)
                .Handle(context => _initialOffset = context.Offset)
                .ToTopic<string, TestMessage>(_destinationTopic)
                .Build()
                .StartAsync(_cancellationTokenSource.Token);

                _producer.ProduceAsync(Guid.NewGuid().ToString(), firstTestMessage).BlockUntil(() => _initialOffset != null).Await();

                Thread.Sleep(1000);
                _cancellationTokenSource.Cancel();
                Thread.Sleep(1000);

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

    }
}