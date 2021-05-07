namespace NetStreams
{
    public class NetStreamResult
    {
        public NetStreamResult(object message)
        {
            Message = message;
        }
        
        public object Message { get; }
        public bool HasValue => Message != null;

        public T GetMessage<T>()
        {
            return (T)Message;
        }
    }

    public class NetStreamResult<TMessage> : NetStreamResult
    {
        public new TMessage Message => GetMessage<TMessage>();

        public NetStreamResult(object message) : base(message)
        {
        }
    }
}