using System;
using System.Runtime.Serialization;

namespace NetStreams.Internal.Exceptions
{
    [Serializable]
    internal class NullOrEmptyTopicException : Exception
    {
        public NullOrEmptyTopicException()
        {
        }
        public NullOrEmptyTopicException(string topic) : base($"{topic} cannot be a null or empty string.")
        {
        }

        public NullOrEmptyTopicException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected NullOrEmptyTopicException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
