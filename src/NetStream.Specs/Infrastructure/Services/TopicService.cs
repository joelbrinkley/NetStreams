using System;
using System.Collections.Generic;
using System.Linq;
using Confluent.Kafka;
using Confluent.Kafka.Admin;

namespace NetStreams.Specs.Infrastructure.Services
{
    public class TopicService
    {
        public void CreateDefaultTopic(string topicName)
        {
            var adminClient = new AdminClientBuilder(new AdminClientConfig { BootstrapServers = "localhost:9092" }).Build();
            adminClient.CreateTopicsAsync(new List<TopicSpecification>()
            {
                new TopicSpecification()
                {
                    Name = topicName,
                    NumPartitions =  1
                }
            });
        }
        public TopicInfo GetTopic(string topicName)
        {
            var adminClient = new AdminClientBuilder(new AdminClientConfig { BootstrapServers = "localhost:9092" }).Build();
            var metadata = adminClient.GetMetadata(topicName, TimeSpan.FromSeconds(60));
            var topicMetadata = metadata.Topics.SingleOrDefault();

            var results = adminClient.DescribeConfigsAsync(new List<ConfigResource>(new[] { new ConfigResource { Name = topicName, Type = ResourceType.Topic } })).Result;
            var topicInfo = results.SingleOrDefault();

            return new TopicInfo
            {
                Name = topicInfo.ConfigResource.Name,
                Partitions = topicMetadata.Partitions.Count,
                RetentionMs = long.Parse(topicInfo.Entries["retention.ms"].Value).ToString(),
                ReplicationFactor = topicMetadata.Partitions.FirstOrDefault().Replicas.Length
            };
        }
    }

    public class TopicInfo
    {
        public string Name { get; set; }
        public int Partitions { get; set; }
        public string RetentionMs { get; set; }
        public int ReplicationFactor { get; set; }
    }
}