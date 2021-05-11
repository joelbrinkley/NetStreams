using System;
using NetStreams.Logging;
using Serilog;

namespace NetStreams.Serilog
{
    public class SerilogLogger : ILog
    {
        readonly ILogger _logger;

        public SerilogLogger(ILogger logger)
        {
            _logger = logger;
        }
        public void Information(string message)
        {
            _logger.Information(message);
        }

        public void Debug(string message)
        {
            _logger.Debug(message);
        }

        public void Warn(string message)
        {
            _logger.Warning(message);
        }

        public void Error(Exception exception, string message)
        {
            _logger.Error(exception, message);
        }
    }
}
