using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NetStreams.Logging
{
    public class LogContext : ILogger
    {
        public List<ILogger> _loggers = new List<ILogger>();

        public void AddLogger(ILogger logger)
        {
            if (!_loggers.Select(x => x.GetType()).Contains(logger.GetType()))
                _loggers.Add(logger);
        }

        public void Write(string message)
        {
            foreach (var logger in _loggers)
            {
                logger.Write(message);
            }
        }
    }
}
