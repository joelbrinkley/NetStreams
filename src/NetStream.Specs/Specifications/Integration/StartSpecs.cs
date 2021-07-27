using Machine.Specifications;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using NetStreams.Specs.Infrastructure;
using NetStreams.Specs.Infrastructure.Extensions;
using NetStreams.Specs.Infrastructure.Models;
using NetStreams.Specs.Infrastructure.Services;

namespace NetStreams.Specs.Specifications.Integration
{
    class StartSpecs
    {
        [Subject("Start")]
        class when_a_stream_is_started
        {
            static string _sourceTopic = $"start.{Guid.NewGuid()}";
            static TestProducerService<string, TestMessage> _producerService;
            static List<TestMessage> _actualMessages = new List<TestMessage>();
            static List<TestMessage> _expectedMessages = new List<TestMessage>();

            Establish context = () =>
            {
                new TopicService().CreateDefaultTopic(_sourceTopic);

                _producerService = new TestProducerService<string, TestMessage>(_sourceTopic);

                DefaultBuilder.New<string, TestMessage>()
                                    .Stream(_sourceTopic)
                                    .Handle(context => _actualMessages.Add(context.Message))
                                    .Build()
                                    .StartAsync(CancellationToken.None);

                _expectedMessages.Add(new TestMessage() { Description = "hello" });
                _expectedMessages.Add(new TestMessage() { Description = "world" });
            };

            Because of = () => Task.Run(() => _expectedMessages.ForEach(x => _producerService.Produce(x.Id, x))).BlockUntil(() => _actualMessages.Count == _expectedMessages.Count).Await();

            It should_consume_messages = () => _expectedMessages.Count.ShouldEqual(_actualMessages.Count);
        }

        [Subject("Start")]
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
