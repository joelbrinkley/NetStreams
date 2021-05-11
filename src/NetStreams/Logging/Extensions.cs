namespace NetStreams.Logging
{
    public static class Extensions
    {
        public static void UseConsole(this LogContext logContext)
        {
            logContext.AddLogger(new ConsoleLog());
        }
    }
}
