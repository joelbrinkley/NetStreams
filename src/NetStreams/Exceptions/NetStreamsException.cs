using System;

namespace NetStreams.Exceptions
{
    [Serializable]
    public class NetStreamsException : Exception
    {
        public NetStreamsException()
        {
        }

        public NetStreamsException(string message) : base(message)
        {
        }

        public NetStreamsException(string message, Exception inner) : base(message, inner)
        {

        }
    }
}
