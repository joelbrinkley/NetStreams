using System;
using System.Collections.Generic;
using System.Text;

namespace NetStreams.Internal.Exceptions
{
    public class StreamFaultedException : Exception
    {
        public StreamFaultedException(Exception inner)
            : base("An unhandled Exception has halted the Stream", inner)
        {

        }
    }
}
