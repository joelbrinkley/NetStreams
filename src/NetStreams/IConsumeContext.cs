using System;

namespace NetStreams
{
    public interface IConsumeContext
    {
        string SourceTopic { get; }
        int Partition { get; }
        long Offset { get; }
        Func<long> GetLag { get; }
        string ConsumerGroup { get;  }
    }
}
