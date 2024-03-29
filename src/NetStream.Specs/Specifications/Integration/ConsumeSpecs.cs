﻿
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Confluent.Kafka;
using ExpectedObjects;
using Machine.Specifications;
using NetStreams.Configuration;
using NetStreams.Specs.Infrastructure.Extensions;
using NetStreams.Specs.Infrastructure.Mocks;
using NetStreams.Specs.Infrastructure.Models;
using NetStreams.Specs.Infrastructure.Mothers;
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
            static INetStream _stream;

            static string _expectedLogMessage;

            Establish context = () =>
            {
                new TopicService().CreateAll(_sourceTopic, _destinationTopic);

                var stringProducer = TestProducerMother.New<string, string>(_sourceTopic);
                _testMessageProducer = TestProducerMother.New<string, TestMessage>(_sourceTopic);

                stringProducer.Produce(Guid.NewGuid().ToString(), "{ malformed }");

                _mockLogger = new MockLog();
                _stream = new NetStreamBuilder<string, TestMessage>(cfg =>
                {
                    cfg.BootstrapServers = "localhost:9092";
                    cfg.ConsumerGroup = Guid.NewGuid().ToString();
                    cfg.AutoOffsetReset = AutoOffsetReset.Earliest;
                    cfg.ConfigureLogging(logging => logging.AddLogger(_mockLogger));
                })
                    .Stream(_sourceTopic)
                    .ToTopic<string, TestMessage>(_destinationTopic)
                    .Build();
                _stream.StartAsync(CancellationToken.None);

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

            Cleanup after = () => _stream.Stop();

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
            static List<long> _offsets = new List<long>();
            static INetStream _stream;

            Establish context = () =>
            {
                new TopicService().CreateAll(_sourceTopic, _destinationTopic);

                _producer = TestProducerMother.New<string, TestMessage>(_sourceTopic);

                var firstTestMessage = new TestMessage();
                var consumerGroup = Guid.NewGuid().ToString();
                _stream = new NetStreamBuilder<string, TestMessage>(cfg =>
                {
                    cfg.BootstrapServers = "localhost:9092";
                    cfg.ConsumerGroup = consumerGroup;
                    cfg.AutoOffsetReset = AutoOffsetReset.Earliest;
                    cfg.DeliveryMode = DeliveryMode.At_Least_Once;
                })
                .Stream(_sourceTopic)
                .Handle(context => _offsets.Add(context.Offset))
                .ToTopic<string, TestMessage>(_destinationTopic)
                .Build();

                var streamTask = _stream.StartAsync(_cancellationTokenSource.Token);

                _producer.ProduceAsync(Guid.NewGuid().ToString(), firstTestMessage).BlockUntil(() => _offsets.Count == 1).Await();

                _stream.Stop();

                streamTask.BlockUntil(() => streamTask.Status == TaskStatus.RanToCompletion).Await();

                _stream.StartAsync(CancellationToken.None);
            };

            Because of = () => _producer.ProduceAsync(Guid.NewGuid().ToString(), new TestMessage()).BlockUntil(() => _offsets.Count == 2).Await();

            Cleanup after = () => _stream.Stop();

            It should_commit_the_offset = () => _offsets[0].ShouldBeLessThan(_offsets[1]);
        }

        [Subject("Consume Failure")]
        class when_consuming_a_message_and_configured_to_continue_on_error
        {
            static readonly string _sourceTopic = $"f.{Guid.NewGuid()}";
            static readonly string _destinationTopic = $"f.{Guid.NewGuid()}";
            static TestProducerService<string, TestMessage> _producer;
            static List<TestMessage> _consumedMessages = new List<TestMessage>();
            static INetStream _stream;

            Establish context = () =>
            {
                new TopicService().CreateAll(_sourceTopic, _destinationTopic);

                _producer = TestProducerMother.New<string, TestMessage>(_sourceTopic);

                _stream = new NetStreamBuilder<string, TestMessage>(cfg =>
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
                .Build();
                _stream.StartAsync(CancellationToken.None);

                _producer.Produce(Guid.NewGuid().ToString(), new TestMessage());
            };

            Because of = () => _producer.ProduceAsync(Guid.NewGuid().ToString(), new TestMessage()).BlockUntil(() => _consumedMessages.Count == 2).Await();

            Cleanup after = () => _stream.Stop();

            It should_continue_processing_the_next_message = () => _consumedMessages.Count.ShouldEqual(2);
        }

        [Subject("Consume Failure")]
        class when_consuming_a_message_and_configured_to_retry_on_error
        {
            static readonly string _sourceTopic = $"ri.{Guid.NewGuid()}";
            static readonly string _destinationTopic = $"ri.{Guid.NewGuid()}";
            static TestProducerService<string, TestMessage> _producer;
            static INetStream _stream;
            static List<long> _consumedOffsets = new List<long>();

            Establish context = () =>
            {
                new TopicService().CreateAll(_sourceTopic, _destinationTopic);

                _producer = TestProducerMother.New<string, TestMessage>(_sourceTopic);

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
                            _consumedOffsets.Add(context.Offset);
                            throw new Exception("Bang!");
                        })
                        .ToTopic<string, TestMessage>(_destinationTopic)
                        .Build();

                _stream.StartAsync(CancellationToken.None);

                _producer.Produce(Guid.NewGuid().ToString(), new TestMessage());
            };

            Because of = () => _producer.ProduceAsync(Guid.NewGuid().ToString(), new TestMessage()).BlockUntil(() => _consumedOffsets.Count > 3).Await();

            Cleanup after = () => _stream.Stop();

            It should_retry_the_same_offset = () => _consumedOffsets.TrueForAll(x => x == 0);
        }
    }
}