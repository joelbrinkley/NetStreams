using System;

namespace NetStreams.Telemetry
{
    public abstract class NetStreamsTelemetryEvent
    {
        public Guid Id { get; }
        public DateTimeOffset OccurredOn { get; }
        public string EventName { get; }
        public string StreamProcessorName { get; }

        public NetStreamsTelemetryEvent(string streamProcessorName)
        {
            Id = Guid.NewGuid();
            OccurredOn = DateTimeOffset.UtcNow;
            EventName = this.GetType().FullName;
            StreamProcessorName = streamProcessorName;
        }
    }
}
