using System.Collections.Generic;

namespace NetStreams
{
    public class NetStreamResult
    {
        public NetStreamResult(object message, List<KeyValuePair<string, string>> headers = null)
        {
            Message = message;
            Headers = headers ?? new List<KeyValuePair<string, string>>();
        }
        
        public object Message { get; }
        public bool HasValue => Message != null;

        public List<KeyValuePair<string, string>> Headers { get; }

        public T GetMessage<T>()
        {
            return (T)Message;
        }
    }

    public class NetStreamResult<TMessage> : NetStreamResult
    {
        public new TMessage Message => GetMessage<TMessage>();

        public NetStreamResult(object message, List<KeyValuePair<string, string>> headers = null) : base(message, headers)
        {
        }
    }
}