using System;

namespace NetStreams.Internal.Exceptions
{
    internal class StreamFaultedException : Exception
    {
        internal StreamFaultedException(Exception inner)
            : base("An unhandled Exception has halted the Build", inner)
        {

        }
    }
}
