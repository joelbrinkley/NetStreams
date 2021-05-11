using System;
using Confluent.Kafka.Admin;

namespace NetStreams.Logging
{
    public interface ILog
    {
        void Information(string message);
        void Debug(string message);
        void Warn(string message);
        void Error(Exception exception, string message);
    }
}
