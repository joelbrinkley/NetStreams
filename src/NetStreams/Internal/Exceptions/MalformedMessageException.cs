using System;
using System.Runtime.Serialization;

namespace NetStreams.Internal.Exceptions
{
    [Serializable]
    public class MalformedMessageException : Exception
    {
        public MalformedMessageException()
        {
        }

        public MalformedMessageException(string message) : base(message)
        {
        }

        public MalformedMessageException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected MalformedMessageException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
