using Machine.Specifications;
using System;
using System.Collections.Generic;
using System.Threading;
using NetStreams.Specs.Infrastructure;
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
            static string _destinationTopic = $"filter.{Guid.NewGuid()}";
            static List<TestMessage> _expectedHandledMessages = new List<TestMessage>();
            static List<TestMessage> _actualHandledMessages = new List<TestMessage>();
            static TestProducerService<string, TestMessage> _producerService;

            Establish context = () =>
            {
                new TopicService().CreateAll(_sourceTopic, _destinationTopic);

                _producerService = new TestProducerService<string, TestMessage>(_sourceTopic);

                DefaultBuilder.New<string, TestMessage>()
                    .Stream(_sourceTopic)
                    .Filter(f => f.Message.Description == "hello")
                    .ToTopic<string, TestMessage>(_destinationTopic, message => message.Id)
                    .Build()
                    .StartAsync(CancellationToken.None);

                DefaultBuilder.New<string, TestMessage>()
                    .Stream(_destinationTopic)
                    .Handle(context => _actualHandledMessages.Add(context.Message))
                    .Build()
                    .StartAsync(CancellationToken.None);

                _producerService.Produce(Guid.NewGuid().ToString(), new TestMessage { Description = "hello" });
                _producerService.Produce(Guid.NewGuid().ToString(), new TestMessage { Description = "world" });

                _expectedHandledMessages.Add(new TestMessage { Description = "hello" });
                _expectedHandledMessages.Add(new TestMessage { Description = "hello" });
            };

            Because of = () => _producerService.ProduceAsync(Guid.NewGuid().ToString(), new TestMessage { Description = "hello" })
                                               .BlockUntil(() => _actualHandledMessages.Count == _expectedHandledMessages.Count).Await();
            
            It should_only_handle_messages_that_satisfy_the_filter = () => _expectedHandledMessages.Count.ShouldEqual(_actualHandledMessages.Count);
        }
    }
}
