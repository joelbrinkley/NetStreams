using System;
using System.Collections.Generic;
using System.Diagnostics;
using Confluent.Kafka;
using Confluent.Kafka.Admin;

namespace NetStreams.Internal.Extensions
{
    internal static class AdminClientExtensions
    {
        internal static bool TopicExists(this IAdminClient adminClient, string topicName, out List<CreateTopicReport> createTopicErrorReports)
        {
           createTopicErrorReports = GetTopicMetadataErrorReports(adminClient, topicName);
            return
                createTopicErrorReports.Count < 1 &&
                !createTopicErrorReports.Exists(x => x.Error == ErrorCode.UnknownTopicOrPart);
        }

        internal static List<CreateTopicReport> GetTopicMetadataErrorReports(this IAdminClient adminClient, string topicName, int timeoutInSeconds = 15)
        {
            return adminClient
                .GetMetadata(topicName, TimeSpan.FromSeconds(timeoutInSeconds)).Topics
                .FindAll(x => x.Error != ErrorCode.NoError)
                .ConvertAll(x => new CreateTopicReport { Topic = x.Topic, Error = x.Error });
        }

        internal static void EnsureTopicCreation(this IAdminClient adminClient, string topicName, int timeoutInSeconds = 15)
        {
            List<CreateTopicReport> createTopicErrorReports = new List<CreateTopicReport>();
            var sw = new Stopwatch();
            sw.Start();

            while (sw.ElapsedMilliseconds < TimeSpan.FromSeconds(timeoutInSeconds).TotalMilliseconds)
            {
                if (TopicExists(adminClient, topicName, out createTopicErrorReports))
                {
                    return;
                }
            }
            throw new CreateTopicsException(createTopicErrorReports);
        }
    }
}
