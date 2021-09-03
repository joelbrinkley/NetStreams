using System.Collections.Generic;
using System.Threading.Tasks;
using NetStreams.Configuration;
using NetStreams.Internal;

namespace NetStreams.Specs.Infrastructure.Mocks
{
    public class NullTopicCreator : ITopicCreator
    {
        public void Dispose()
        {
        }

        public Task Create(TopicConfiguration topicConfig)
        {
            return Task.CompletedTask;
        }

        public Task CreateAll(IEnumerable<TopicConfiguration> topicConfigurations)
        {
            return Task.CompletedTask;
        }
    }
}
