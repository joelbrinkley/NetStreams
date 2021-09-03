using NetStreams.Telemetry;
using System;

namespace NetStreams.Telemetry.Events
{
    public class NetStreamExceptionOccurred : NetStreamTelemetryEvent
    {
        public object Message { get; set; }
        public object Key { get; set; }
        public int? Partition { get; set; }
        public string Topic { get; set; }
        public Exception Exception { get; set; }
        public long? Offset { get; set; }

        internal static NetStreamExceptionOccurred Create<TKey, TMessage>(string streamProcessorName, Exception exception, IConsumeContext<TKey, TMessage> context)
        {
            return new NetStreamExceptionOccurred(streamProcessorName, exception)
            {

                Message = context == null ? default(TMessage) : context.Message,
                Key = context == null ? default(TKey) : context.Key,
                Partition = context?.Partition,
                Topic = context?.TopicName,
                Offset = context?.Offset
            };
        }

        public NetStreamExceptionOccurred(string streamProcessorName, Exception exception)
            : base(streamProcessorName)
        {
            Exception = exception;
        }

        public NetStreamExceptionOccurred()
        {

        }
    }
}