using NetStreams.Telemetry;

namespace NetStreams.Internal
{
    internal class StreamHeartBeat : NetStreamsTelemetryEvent
    {
        public StreamHeartBeat(string streamProcessorName) : base(streamProcessorName)
        {

        }
    }
}