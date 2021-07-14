using System.Collections.Generic;

namespace NetStreams
{
    public interface IConsumeContext<TKey, TMessage>
    {
        TKey Key { get; }
        TMessage Message { get; }
        string TopicName { get; }
        int Partition { get; }
        long Offset { get; }
        long Lag { get; }
        string ConsumeGroup { get;  }
        IEnumerable<KeyValuePair<string, string>> Headers { get; }
    }
}
