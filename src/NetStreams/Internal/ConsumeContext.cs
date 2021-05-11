using System;
using System.Threading;
using Confluent.Kafka;

namespace NetStreams
{
    public class ConsumeContext : IConsumeContext, ICancellationTokenCarrier
    {
        public CancellationToken CancellationToken { get; }
        public string ConsumerGroup { get; }
        public string SourceTopic { get; }
        public int Partition { get; }
        public long Offset { get; }
        public Func<long> GetLag { get; }

        public ConsumeContext(CancellationToken cancellationToken, string consumerGroup, string sourceTopic, int partition, long offset, Func<long> getLag)
        {
            CancellationToken = cancellationToken;
            ConsumerGroup = consumerGroup;
            SourceTopic = sourceTopic;
            Partition = partition;
            Offset = offset;
            GetLag = getLag;
        }

    }
}
