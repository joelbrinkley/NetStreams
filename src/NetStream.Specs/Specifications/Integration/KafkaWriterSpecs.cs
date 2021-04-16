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
        class when_configuring_a_kafka_writer_with_no_resolve_key_function_for_a_reference_type
        {
            static TestProducerService<string, TestMessage> _producerService;
            static string _actualKey = "default value";
            static string _sourceTopic = $"kwr.source.{Guid.NewGuid()}";
            static string _destinationTopic = $"kwr.dest.{Guid.NewGuid()}";
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

                var _stream1 = builder
                    .Stream<string, TestMessage>(_sourceTopic)
                    .Handle<string, TestMessage>(context => context.Message)
                    .ToTopic(_destinationTopic)
                    .StartAsync(CancellationToken.None);

                var stream2 = builder
                    .Stream<string, TestMessage>(_destinationTopic)
                    .Handle(context => _actualKey = context.Key)
                    .StartAsync(CancellationToken.None);
            };

            Because of = () => _producerService.ProduceAsync(_message.Id, _message).BlockUntil(() => _actualKey == null).Await();

            It should_default_the_key_to_the_default_value_of_the_response_key_type = () => _actualKey.ShouldBeNull();
        }

        [Subject("KafkaWriter")]
        class when_configuring_a_kafka_writer_with_no_resolve_key_function_for_a_value_type
        {
            static TestProducerService<int, TestMessage> _producerService;
            static int _actualKey = 1;
            static string _sourceTopic = $"kwv.source.{Guid.NewGuid()}";
            static string _destinationTopic = $"kwv.dest.{Guid.NewGuid()}";
            static TestMessage _message;

            Establish context = () =>
            {
                new TopicService().CreateDefaultTopic(_sourceTopic);
                new TopicService().CreateDefaultTopic(_destinationTopic);

                _producerService = new TestProducerService<int, TestMessage>(_sourceTopic);

                _message = new TestMessage { Description = "Hello World" };

                var builder = new NetStreamBuilder(cfg =>
                {
                    cfg.ConsumerGroup = Guid.NewGuid().ToString();
                    cfg.BootstrapServers = "localhost:9092";
                });

                var _stream1 = builder
                    .Stream<int, TestMessage>(_sourceTopic)
                    .Handle<int, TestMessage>(context => context.Message)
                    .ToTopic(_destinationTopic)
                    .StartAsync(CancellationToken.None);

                var stream2 = builder
                    .Stream<int, TestMessage>(_destinationTopic)
                    .Handle(context => _actualKey = context.Key)
                    .StartAsync(CancellationToken.None);
            };

            Because of = () => _producerService.ProduceAsync(1, _message).BlockUntil(() => _actualKey == 0).Await();

            It should_default_the_key_to_the_default_value_of_the_response_key_type = () => _actualKey.ShouldEqual(default(int));
        }
    }
}