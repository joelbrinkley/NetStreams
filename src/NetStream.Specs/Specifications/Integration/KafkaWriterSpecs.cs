using System;
using System.Threading;
using System.Threading.Tasks;
using Confluent.Kafka;
using Machine.Specifications;
using NetStreams.Specs.Infrastructure.Extensions;
using NetStreams.Specs.Infrastructure.Models;
using NetStreams.Specs.Infrastructure.Services;

namespace NetStreams.Specs.Specifications.Integration
{
    internal class KafkaWriterSpecs
    {
        [Subject("KafkaWriter")]
        class when_configuring_a_kafka_writer_with_no_resolve_key_function
        {
            static TestProducerService<string, TestMessage> _producerService;
            static string _actualKey = "default value";
            static string _sourceTopic = $"kw.source.{Guid.NewGuid()}";
            static string _destinationTopic = $"kw.dest.{Guid.NewGuid()}";
            static TestMessage _message;

            Establish context = () =>
            {
                new TopicService().CreateDefaultTopic(_sourceTopic);
                new TopicService().CreateDefaultTopic(_destinationTopic);

                _producerService = new TestProducerService<string, TestMessage>(_sourceTopic);

                _message = new TestMessage { Description = "Hello World" };

                var builder = new NetStreamBuilder(cfg =>
                {
                    cfg.ConsumerGroup = Guid.NewGuid().ToString();
                    cfg.BootstrapServers = "localhost:9092";
                });

                _stream1 = builder
                    .Stream<string, TestMessage>(_sourceTopic)
                    .Handle<string, TestMessage>(context => context.Message)
                    .ToTopic(_destinationTopic)
                    .StartAsync(CancellationToken.None);

                var stream2 = builder
                    .Stream<string, TestMessage>(_destinationTopic)
                    .Handle(context => _actualKey = context.Key)
                    .StartAsync(CancellationToken.None);
            };

            Because of = () => _producerService.ProduceAsync(_message.Id, _message).BlockUntil(() => _actualKey == null, 30).Await();

            It should_default_the_key_to_the_default_value_of_the_response_key_type = () => _actualKey.ShouldBeNull();
            static Task _stream1;
        }
    }
}