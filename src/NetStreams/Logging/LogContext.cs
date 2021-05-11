using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NetStreams.Logging
{
    public class LogContext : ILog
    {
        public List<ILog> _loggers = new List<ILog>();

        public void AddLogger(ILog log)
        {
            if (!_loggers.Select(x => x.GetType()).Contains(log.GetType()))
                _loggers.Add(log);
        }

        public void Information(string message)
        {
            _loggers.ForEach(log => log.Information(message));
        }

        public void Debug(string message)
        {
            _loggers.ForEach(log => log.Debug(message));
        }

        public void Warn(string message)
        {
            _loggers.ForEach(log => log.Warn(message));
        }

        public void Error(Exception exception, string message)
        {
            _loggers.ForEach(log => log.Error(exception, message));
        }
    }
}
