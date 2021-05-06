using System;
using System.Threading;
using System.Threading.Tasks;
using ExpectedObjects;
using Machine.Specifications;
using NetStreams.Specs.Infrastructure.Models;
using NetStreams.Specs.Infrastructure.Services;

namespace NetStreams.Specs.Specifications.Integration
{
    internal class TopicCreatorSpecs
    {
        [Subject("Topic Auto Create")]
        class when_a_stream_starts_with_a_new_topic_configuration
        {
            static string _destinationTopic;
            static string _sourceTopic;
            static ExpectedObject _expectedSourceTopic;
            static ExpectedObject _expectedDestinationTopic;
            static INetStream _stream;

            private Establish context = () =>
            {
                _sourceTopic = $"auto.{Guid.NewGuid()}";
                _destinationTopic = $"auto.{Guid.NewGuid()}";

                var builder = new NetStreamBuilder(cfg =>
                {
                    cfg.ConsumerGroup = Guid.NewGuid().ToString();
                    cfg.BootstrapServers = "localhost:9092";
                    cfg.AddTopicConfiguration(topicCfg =>
                    {
                        topicCfg.Name = _sourceTopic;
                        topicCfg.Partitions = 4;
                        topicCfg.RetentionMs = 2000;
                        topicCfg.ReplicationFactor = 1;
                    });

                    cfg.AddTopicConfiguration(topicCfg =>
                    {
                        topicCfg.Name = _destinationTopic;
                        topicCfg.Partitions = 1;
                        topicCfg.RetentionMs = 1000;
                        topicCfg.ReplicationFactor = 1;
                    });

                    _expectedSourceTopic = new
                    {
                        Name = _sourceTopic,
                        Partitions = 4,
                        RetentionMs = "2000",
                        ReplicationFactor = 1
                    }.ToExpectedObject();

                    _expectedDestinationTopic = new
                    {
                        Name = _destinationTopic,
                        Partitions = 1,
                        RetentionMs = "1000",
                        ReplicationFactor = 1
                    }.ToExpectedObject();
                });

                _stream = builder
                    .Stream<string, TestMessage>(_sourceTopic)
                    .Transform(context => new TestEvent())
                    .ToTopic<string, TestEvent>(_destinationTopic, message => message.Key);
                _stream.StartAsync(CancellationToken.None);
            };

            Because of = () => Task.Delay(TimeSpan.FromSeconds(1)).Await();

            It should_create_the_destination_topic_with_configuration = () => 
                _expectedDestinationTopic.ShouldMatch(new TopicService().GetTopic(_destinationTopic));
            
            It should_create_the_source_topic_with_configuration = () =>
                _expectedSourceTopic.ShouldMatch(new TopicService().GetTopic(_sourceTopic));
        }
    }
}