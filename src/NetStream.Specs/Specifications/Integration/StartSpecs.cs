using Machine.Specifications;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
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
            static INetStream<string, TestMessage> _stream;
            static TestProducerService<string, TestMessage> _producerService;
            static List<TestMessage> _actualMessages = new List<TestMessage>();
            static List<TestMessage> _expectedMessages = new List<TestMessage>();

            Establish context = () =>
            {
                new TopicService().CreateDefaultTopic(_sourceTopic);

                _producerService = new TestProducerService<string, TestMessage>(_sourceTopic);

                var builder = new NetStreamBuilder(cfg =>
               {
                   cfg.BootstrapServers = "localhost:9092";
                   cfg.ConsumerGroup = $"start.{Guid.NewGuid().ToString()}";
               });

                _stream = builder.Stream<string, TestMessage>(_sourceTopic)
                                 .Handle(context => _actualMessages.Add(context.Message));
                           
                _stream.StartAsync(CancellationToken.None);

                _expectedMessages.Add(new TestMessage() { Description = "hello" });
                _expectedMessages.Add(new TestMessage() { Description = "world" });
            };

            Because of = () => Task.Run(() => _expectedMessages.ForEach(x => _producerService.Produce(x.Key, x))).BlockUntil(() => _actualMessages.Count == _expectedMessages.Count).Await();

            It should_consume_messages = () => _expectedMessages.Count.ShouldEqual(_actualMessages.Count);
        }
    }
}
