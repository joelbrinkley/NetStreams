using Confluent.Kafka;

namespace NetStreams
{
    public class ConsumeContext<TKey, TMessage> : IConsumeContext<TKey, TMessage>
    {
        readonly ConsumeResult<TKey, TMessage> _consumeResult;

        public ConsumeContext(ConsumeResult<TKey, TMessage> consumeResult)
        {
            _consumeResult = consumeResult;
        }

        public TMessage Message => _consumeResult.Message.Value;

        public TKey Key => _consumeResult.Message.Key;

        public object Result { get; set; }
    }
}
