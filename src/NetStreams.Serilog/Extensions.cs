using NetStreams.Logging;
using Serilog;

namespace NetStreams.Serilog
{
    public static class Extensions
    {
        public static void UseSerilog(this LogContext logContext, ILogger logger)
        {
            logContext.AddLogger(new SerilogLogger(logger));
        }
    }
}
