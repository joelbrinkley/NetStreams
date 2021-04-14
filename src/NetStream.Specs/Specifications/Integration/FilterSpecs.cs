using Machine.Specifications;
using System;
using System.Collections.Generic;
using System.Threading;
using NetStreams.Specs.Infrastructure.Extensions;
using NetStreams.Specs.Infrastructure.Models;
using NetStreams.Specs.Infrastructure.Services;

namespace NetStreams.Specs.Specifications.Integration
{
    class FilterSpecs
    {
        [Subject("Filter")]
        class when_filtering_a_stream
        {
            static string _sourceTopic = $"filter.{Guid.NewGuid()}";
            static INetStream<string, TestMessage> _stream;
            static List<TestMessage> _expectedHandledMessages = new List<TestMessage>();
            static List<TestMessage> _actualHandledMessages = new List<TestMessage>();
            static TestProducerService<string, TestMessage> _producerService;

            Establish context = () =>
            {
                new TopicService().CreateDefaultTopic(_sourceTopic);

                _producerService = new TestProducerService<string, TestMessage>(_sourceTopic);

                var builder = new NetStreamBuilder(cfg =>
                {
                    cfg.BootstrapServers = "localhost:9092";
                    cfg.ConsumerGroup = $"filter.{Guid.NewGuid()}";
                });

                _stream = builder.Stream<string, TestMessage>(_sourceTopic)
                    .Filter(f => f.Message.Description == "hello")
                    .Handle(context => _actualHandledMessages.Add(context.Message));
                
                _stream.StartAsync(CancellationToken.None);

                _producerService.Produce(Guid.NewGuid().ToString(), new TestMessage { Description = "hello" });
                _producerService.Produce(Guid.NewGuid().ToString(), new TestMessage { Description = "world" });

                _expectedHandledMessages.Add(new TestMessage { Description = "hello" });
                _expectedHandledMessages.Add(new TestMessage { Description = "hello" });
            };

            Because of = () => _producerService.ProduceAsync(Guid.NewGuid().ToString(), new TestMessage { Description = "hello" })
                                               .BlockUntil(() => _actualHandledMessages.Count == _expectedHandledMessages.Count).Await();
            
            It should_only_handle_messages_that_satisify_the_filter = () => _expectedHandledMessages.Count.ShouldEqual(_actualHandledMessages.Count);
        }
    }
}
