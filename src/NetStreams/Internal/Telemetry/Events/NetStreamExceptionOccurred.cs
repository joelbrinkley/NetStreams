using NetStreams.Telemetry;

namespace NetStreams.Internal
{
    internal class NetStreamExceptionOccurred : NetStreamsTelemetryEvent
    {
        public NetStreamExceptionOccurred(string streamProcessorName) : base(streamProcessorName)
        {
        }
    }
}