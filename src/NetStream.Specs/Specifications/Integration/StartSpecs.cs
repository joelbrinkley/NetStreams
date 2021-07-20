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
        class when_a_stream_is_started_on_plaintext_cluster
        {
            static string _sourceTopic = $"start.{Guid.NewGuid()}";
            static TestProducerService<string, TestMessage> _producerService;
            static List<TestMessage> _actualMessages = new List<TestMessage>();
            static List<TestMessage> _expectedMessages = new List<TestMessage>();

            Establish context = () =>
            {
                new TopicService().CreateDefaultTopic(_sourceTopic);

                _producerService = TestProducerFactory.Plaintext<string, TestMessage>(_sourceTopic);
                
                DefaultBuilder.Plaintext<string, TestMessage>()
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
        class when_a_stream_is_started_on_ssl_cluster
        {
            static string _sourceTopic = $"start.{Guid.NewGuid()}";
            static TestProducerService<string, TestMessage> _producerService;
            static List<TestMessage> _actualMessages = new List<TestMessage>();
            static List<TestMessage> _expectedMessages = new List<TestMessage>();

            Establish context = () =>
            {
                new TopicService().CreateDefaultTopic(_sourceTopic);

                _producerService = TestProducerFactory.Ssl<string, TestMessage>(_sourceTopic);

                DefaultBuilder.Ssl<string, TestMessage>()
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
        class when_a_stream_is_started_on_sasl_cluster
        {
            static string _sourceTopic = $"start.{Guid.NewGuid()}";
            static TestProducerService<string, TestMessage> _producerService;
            static List<TestMessage> _actualMessages = new List<TestMessage>();
            static List<TestMessage> _expectedMessages = new List<TestMessage>();

            Establish context = () =>
            {
                new TopicService().CreateDefaultTopic(_sourceTopic);

                _producerService = TestProducerFactory.SaslScram256<string, TestMessage>(_sourceTopic);

                DefaultBuilder.SaslScram256<string, TestMessage>()
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
    }
}
