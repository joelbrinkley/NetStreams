using System;

namespace NetStreams.Telemetry
{
    public abstract class NetStreamTelemetryEvent
    {
        public Guid Id { get; set; }
        public DateTimeOffset OccurredOn { get; set; }
        public string EventName { get; set; }
        public string FullName { get; set; }
        public string StreamProcessorName { get; set; }

        public NetStreamTelemetryEvent(string streamProcessorName)
        {
            Id = Guid.NewGuid();
            OccurredOn = DateTimeOffset.UtcNow;
            EventName = this.GetType().Name;
            FullName = this.GetType().FullName;
            StreamProcessorName = streamProcessorName;
        }

        public NetStreamTelemetryEvent()
        {

        }
    }
}
