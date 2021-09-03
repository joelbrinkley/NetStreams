namespace NetStreams.Telemetry.Events
{
    public class MessageProcessingCompleted : NetStreamTelemetryEvent
    {
        public long Offset { get; set; }
        public string ConsumerGroup { get; set; }

        internal static MessageProcessingCompleted Create<TKey, TMessage>(string streamProcessorName, ConsumeContext<TKey, TMessage> consumeContext)
        {
            return new MessageProcessingCompleted(streamProcessorName)
            {
                Offset = consumeContext.Offset,
                ConsumerGroup = consumeContext.ConsumeGroup,
            };
        }

        public MessageProcessingCompleted(
            string streamProcessorName)
            : base(streamProcessorName)
        {
        }

        public MessageProcessingCompleted()
        {

        }
    }
}