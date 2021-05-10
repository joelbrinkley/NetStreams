using System;

namespace NetStreams.Internal.Exceptions
{
    public class StreamFaultedException : Exception
    {
        public StreamFaultedException(Exception inner)
            : base("An unhandled Exception has halted the Build", inner)
        {

        }
    }
}
