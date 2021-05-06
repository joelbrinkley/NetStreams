namespace NetStreams
{
    public class TransformResult
    {
        public TransformResult(object message)
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

    public class TransformResult<TMessage> : TransformResult
    {
        public new TMessage Message => GetMessage<TMessage>();

        public TransformResult(object message) : base(message)
        {
        }
    }
}