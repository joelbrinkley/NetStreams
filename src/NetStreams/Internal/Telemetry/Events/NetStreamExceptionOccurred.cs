using NetStreams.Telemetry;
using System;

namespace NetStreams.Internal
{
    internal class NetStreamExceptionOccurred<TKey, TMessage> : NetStreamsTelemetryEvent
    {
        public IConsumeContext<TKey, TMessage> ConsumeContext { get; }
        public Exception Exception { get; }

        public NetStreamExceptionOccurred(string streamProcessorName,Exception exception, IConsumeContext<TKey, TMessage> consumeContext)
            : base(streamProcessorName)
        {
            Exception = exception;
            ConsumeContext = consumeContext;
        }
    }
}