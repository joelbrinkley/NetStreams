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

        public Task Create(ITopicConfiguration topicConfig)
        {
            return Task.CompletedTask;
        }

        public Task CreateAll(IEnumerable<ITopicConfiguration> topicConfigurations)
        {
            return Task.CompletedTask;
        }
    }
}
