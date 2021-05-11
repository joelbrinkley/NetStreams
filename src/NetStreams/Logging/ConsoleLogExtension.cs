using System;
using System.Collections.Generic;
using System.Text;

namespace NetStreams.Logging
{
    public static class ConsoleLogExtension
    {
        public static void AddConsole(this LogContext logContext)
        {
            logContext.AddLogger(new ConsoleLogger());
        }
    }
}
