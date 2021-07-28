using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Confluent.Kafka;
using Machine.Specifications;
using NetStreams.Internal;
using NetStreams.Specs.Infrastructure.Extensions;
using NetStreams.Specs.Infrastructure.Models;
using NetStreams.Specs.Infrastructure.Mothers;
using NetStreams.Specs.Infrastructure.Services;

namespace NetStreams.Specs.Specifications.Integration
{
    internal class ToTopicSpecs
    {
        [Subject("ToTopic")]
        class when_writing_to_a_topic_with_a_resolve_key_function
        {
            static TestProducerService<string, TestMessage> _producerService;
            static string _actualKey;
            static string _sourceTopic = $"kwkey.source.{Guid.NewGuid()}";
            static string _destinationTopic = $"kwkey.dest.{Guid.NewGuid()}";
            static TestMessage _message;

            Establish context = () =>
            {
                new TopicService().CreateAll(_sourceTopic, _destinationTopic);

                _producerService = TestProducerMother.New<string, TestMessage>(_sourceTopic);

                _message = new TestMessage { Description = "Hello World" };

               DefaultBuilder.New<string, TestMessage>()
                    .Stream(_sourceTopic)
                    .Transform(context => context.Message)
                    .ToTopic<string, TestMessage>(_destinationTopic, message => message.Id)
                    .Build()
                    .StartAsync(CancellationToken.None);

               DefaultBuilder.New<string, TestMessage>()
                    .Stream(_destinationTopic)
                    .Handle(context => _actualKey = context.Key)
                    .Build()
                    .StartAsync(CancellationToken.None);
            };
            Because of = () => _producerService.ProduceAsync(_message.Id, _message).BlockUntil(() => _actualKey != null).Await();

            It should_resolve_the_key = () => _actualKey.ShouldEqual(_message.Id);
        }

        [Subject("ToTopic")]
        class when_writing_to_a_topic_with_no_resolve_key_function_for_a_reference_type
        {
            static TestProducerService<string, TestMessage> _producerService;
            static string _actualKey = "default value";
            static string _sourceTopic = $"kwr.source.{Guid.NewGuid()}";
            static string _destinationTopic = $"kwr.dest.{Guid.NewGuid()}";
            static TestMessage _message;

            Establish context = () =>
            {
                new TopicService().CreateAll(_sourceTopic, _destinationTopic);

                _producerService = TestProducerMother.New<string, TestMessage>(_sourceTopic);

                _message = new TestMessage { Description = "Hello World" };

                DefaultBuilder.New<string, TestMessage>()
                     .Stream(_sourceTopic)
                    .Transform(context => context.Message)
                    .ToTopic<string, TestMessage>(_destinationTopic)
                    .Build()
                    .StartAsync(CancellationToken.None);

                DefaultBuilder.New<string, TestMessage>()
                    .Stream(_destinationTopic)
                    .Handle(context => _actualKey = context.Key)
                    .Build()
                    .StartAsync(CancellationToken.None);
            };

            Because of = () => _producerService.ProduceAsync(_message.Id, _message).BlockUntil(() => _actualKey == null).Await();

            It should_default_the_key_to_the_default_value_of_the_response_key_type = () => _actualKey.ShouldBeNull();
        }

        [Subject("ToTopic")]
        class when_writing_to_a_topic_with_no_resolve_key_function_for_a_value_type
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

                _producerService = TestProducerMother.New<int, TestMessage>(_sourceTopic);

                _message = new TestMessage { Description = "Hello World" };

                DefaultBuilder.New<int, TestMessage>()
                  .Stream(_sourceTopic)
                    .Transform(context => context.Message)
                    .ToTopic<int, TestMessage>(_destinationTopic)
                    .Build()
                    .StartAsync(CancellationToken.None);

                DefaultBuilder.New<int, TestMessage>()
                    .Stream(_destinationTopic)
                    .Handle(context => _actualKey = context.Key)
                    .Build()
                    .StartAsync(CancellationToken.None);
            };

            Because of = () => _producerService.ProduceAsync(1, _message).BlockUntil(() => _actualKey == 0).Await();

            It should_default_the_key_to_the_default_value_of_the_response_key_type = () => _actualKey.ShouldEqual(default(int));
        }


        [Subject("ToTopic")]
        public class when_writing_to_a_topic_with_enable_message_type_header_set_to_false
        {
            static TestProducerService<int, TestMessage> _producerService;
            static IEnumerable<KeyValuePair<string, string>> _actualHeaders;
            static string _sourceTopic = $"kh.source.{Guid.NewGuid()}";
            static string _destinationTopic = $"kh.dest.{Guid.NewGuid()}";
            static TestMessage _message;

            Establish context = () =>
            {
                new TopicService().CreateDefaultTopic(_sourceTopic);
                new TopicService().CreateDefaultTopic(_destinationTopic);

                _producerService = TestProducerMother.New<int, TestMessage>(_sourceTopic);

                _message = new TestMessage { Description = "Hello World" };

                new NetStreamBuilder<string, TestMessage>(cfg =>
                {
                    cfg.BootstrapServers = "localhost:9092";
                    cfg.ConsumerGroup = Guid.NewGuid().ToString();
                    cfg.AutoOffsetReset = AutoOffsetReset.Earliest;
                    cfg.EnableMessageTypeHeader = false;
                })
                .Stream(_sourceTopic)
                .ToTopic<string, TestMessage>(_destinationTopic)
                .Build()
                .StartAsync(CancellationToken.None);

                DefaultBuilder.New<string, TestMessage>()
                    .Stream(_destinationTopic)
                    .Handle(context => _actualHeaders = context.Headers)
                    .Build()
                    .StartAsync(CancellationToken.None);
            };

            Because of = () => _producerService.ProduceAsync(1, _message).BlockUntil(() => _actualHeaders != null).Await();

            It should_not_write_the_header = () => _actualHeaders.Any(h => h.Key == NetStreamConstants.HEADER_TYPE).ShouldBeFalse();
        }
    }
}