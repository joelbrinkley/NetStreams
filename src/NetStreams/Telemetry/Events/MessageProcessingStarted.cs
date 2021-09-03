using NetStreams.Telemetry;
using System.Collections.Generic;
using System.Linq;

namespace NetStreams.Telemetry.Events
{
    public class MessageProcessingStarted : NetStreamTelemetryEvent
    {
        public object Message { get; set; }
        public object Key { get; set; }
        public long Lag { get; set; }
        public int Partition { get; set; }
        public string Topic { get; set; }
        public long Offset { get; set; }
        public List<KeyValuePair<string, string>> Headers { get; set; }
        public string ConsumerGroup { get; set; }

        internal static MessageProcessingStarted Create<TKey, TMessage>(string streamProcessorName, ConsumeContext<TKey, TMessage> context)
        {
            return new MessageProcessingStarted(streamProcessorName)
            {
                Message = context.Message,
                Key = context.Key,
                Lag = context.Lag,
                Partition = context.Partition,
                Topic = context.TopicName,
                Offset = context.Offset,
                Headers = context.Headers.ToList(),
                ConsumerGroup = context.ConsumeGroup
            };
        }

        public MessageProcessingStarted(
            string streamProcessorName)
            : base(streamProcessorName)
        {
        }

        public MessageProcessingStarted()
        {

        }
    }
}