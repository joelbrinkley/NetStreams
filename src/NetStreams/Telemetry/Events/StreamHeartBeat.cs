using NetStreams.Telemetry;

namespace NetStreams.Telemetry.Events
{
    public class StreamHeartBeat : NetStreamTelemetryEvent
    {
        public StreamHeartBeat(string streamProcessorName) : base(streamProcessorName)
        {
        }

        public StreamHeartBeat()
        {

        }
    }
}