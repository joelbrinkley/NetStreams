namespace NetStreams
{
    public class HandleResult
    {
        public HandleResult(object message)
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

    public class HandleResult<TMessage> : HandleResult
    {
        public new TMessage Message => GetMessage<TMessage>();

        public HandleResult(object message) : base(message)
        {
        }
    }
}