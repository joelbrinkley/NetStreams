using Confluent.Kafka;
using Machine.Specifications;
using NetStreams.Correlation;
using NetStreams.Internal;
using NetStreams.Specs.Infrastructure.Extensions;
using NetStreams.Specs.Infrastructure.Models;
using NetStreams.Specs.Infrastructure.Mothers;
using NetStreams.Specs.Infrastructure.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NetStreams.Specs.Specifications.Integration
{
    class CorrelationSpecs
    {
        [Subject("Correlation")]
        class when_a_message_with_no_correlation_id_is_consumed
        {
            static string _sourceTopic = $"corr.src.{Guid.NewGuid()}";
            static string _destinationTopic = $"corr.dest.{Guid.NewGuid()}";
            static TestProducerService<string, TestMessage> _producer;
            static string _actualCorrelationId;

            Establish context = () =>
            {
                new TopicService().CreateAll(_sourceTopic, _destinationTopic);

                _producer = TestProducerMother.New<string, TestMessage>(_sourceTopic);

                new NetStreamBuilder<string, TestMessage>(cfg =>
                {
                    cfg.BootstrapServers = "localhost:9092";
                    cfg.ConsumerGroup = Guid.NewGuid().ToString();
                    cfg.AutoOffsetReset = AutoOffsetReset.Earliest;
                    cfg.EnableMessageCorrelation();
                })
                .Stream(_sourceTopic)
                .ToTopic<string, TestMessage>(_destinationTopic)
                .Build()
                .StartAsync(CancellationToken.None);

                DefaultBuilder.New<string, TestMessage>()
                .Stream(_destinationTopic)
                .Handle(context => _actualCorrelationId = context.Headers.FirstOrDefault(x => x.Key == CorrelationHeader.Value).Value)
                .Build()
                .StartAsync(CancellationToken.None);
            };

            Because of = () => _producer.ProduceAsync(Guid.NewGuid().ToString(), new TestMessage()).BlockUntil(() => _actualCorrelationId != null).Await();

            It should_be_published_with_a_correlation_id = () => _actualCorrelationId.ShouldNotBeEmpty();
        }

        [Subject("Correlation")]
        class when_a_message_with_a_correlation_id_is_consumed
        {
            static string _sourceTopic = $"correx.src.{Guid.NewGuid()}";
            static string _destination1 = $"correx.dest1.{Guid.NewGuid()}";
            static string _destination2 = $"correx.dest2.{Guid.NewGuid()}";
            static TestProducerService<string, TestMessage> _producer;
            static string _expectedCorrelationId;
            static string _actualCorrelationId;

            Establish context = () =>
            {
                new TopicService().CreateAll(_sourceTopic, _destination1, _destination2);

                _producer = TestProducerMother.New<string, TestMessage>(_sourceTopic);

                new NetStreamBuilder<string, TestMessage>(cfg =>
                {
                    cfg.BootstrapServers = "localhost:9092";
                    cfg.ConsumerGroup = Guid.NewGuid().ToString();
                    cfg.AutoOffsetReset = AutoOffsetReset.Earliest;
                    cfg.EnableMessageCorrelation();
                })
                .Stream(_sourceTopic)
                .ToTopic<string, TestMessage>(_destination1)
                .Build()
                .StartAsync(CancellationToken.None);

                new NetStreamBuilder<string, TestMessage>(cfg =>
                {
                    cfg.BootstrapServers = "localhost:9092";
                    cfg.ConsumerGroup = Guid.NewGuid().ToString();
                    cfg.AutoOffsetReset = AutoOffsetReset.Earliest;
                    cfg.EnableMessageCorrelation();
                })
               .Stream(_destination1)
               .Handle(context => _expectedCorrelationId = context.Headers.FirstOrDefault(x => x.Key == CorrelationHeader.Value).Value)
               .ToTopic<string, TestMessage>(_destination2)
               .Build()
               .StartAsync(CancellationToken.None);

               new NetStreamBuilder<string, TestMessage>(cfg =>
               {
                   cfg.BootstrapServers = "localhost:9092";
                   cfg.ConsumerGroup = Guid.NewGuid().ToString();
                   cfg.AutoOffsetReset = AutoOffsetReset.Earliest;
                   cfg.EnableMessageCorrelation();
               })
              .Stream(_destination2)
              .Handle(context => _actualCorrelationId = context.Headers.FirstOrDefault(x => x.Key == CorrelationHeader.Value).Value)
              .Build()
              .StartAsync(CancellationToken.None);
            };

            Because of = () => _producer.ProduceAsync(Guid.NewGuid().ToString(), new TestMessage()).BlockUntil(() => _actualCorrelationId != null).Await();

            It should_be_published_with_the_consumed_message_correlation_id = () => _actualCorrelationId.ShouldEqual(_expectedCorrelationId);
        }
    }
}
