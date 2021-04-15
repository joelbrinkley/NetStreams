using Machine.Specifications;
using System;
using System.Collections.Generic;
using System.Threading;
using NetStreams.Specs.Infrastructure.Extensions;
using NetStreams.Specs.Infrastructure.Models;
using NetStreams.Specs.Infrastructure.Services;

namespace NetStreams.Specs.Specifications.Integration
{
    class HandleSpecs
    {
        [Subject("Handle")]
        class when_streaming_output_to_a_topic
        {
            static string _sourceTopic = $"output.source.{Guid.NewGuid()}";
            static string _destinationTopic = $"output.dest.{Guid.NewGuid()}";
            static TestProducerService<string, TestMessage> _producerService;
            static TestMessage _message;
            static List<TestEvent> _expectedMessages = new List<TestEvent>();
            static List<TestEvent> _actualMessages = new List<TestEvent>();

            Establish context = () =>
            {
                new TopicService().CreateDefaultTopic(_sourceTopic);
                new TopicService().CreateDefaultTopic(_destinationTopic);

                _message = new TestMessage() { Description = "Hello World" };

                _producerService = new TestProducerService<string, TestMessage>(_sourceTopic);

                var builder = new NetStreamBuilder(cfg =>
                {
                    cfg.ConsumerGroup = Guid.NewGuid().ToString();
                    cfg.BootstrapServers = "localhost:9092";
                });

                var testEvent = new TestEvent()
                {
                    Description = $"Handled Message with id {_message.Id}"
                };

                _expectedMessages.Add(testEvent);

                var stream1 = builder
                               .Stream<string, TestMessage>(_sourceTopic)
                               .Handle<string, TestEvent>(context => testEvent)
                               .ToTopic(_destinationTopic, message => message.Key)
                               .StartAsync(CancellationToken.None);

                var stream2 = builder
                             .Stream<string, TestEvent>(_destinationTopic)
                             .Handle(context => _actualMessages.Add(context.Message))
                             .StartAsync(CancellationToken.None);
            };

            Because of = () => _producerService.ProduceAsync(_message.Key, _message).BlockUntil(() => _actualMessages.Count == 1).Await();

            It should_write_output_to_topic = () => _expectedMessages.Count.ShouldEqual(_actualMessages.Count);
        }
    }
}
