using Confluent.Kafka;

namespace NetStreams
{
    public class ConsumeContext<T>: IConsumeContext<T>
    {
        readonly ConsumeResult<string, T> _consumeResult;

        public ConsumeContext(ConsumeResult<string, T> consumeResult)
        {
            _consumeResult = consumeResult;
        }

        public T Message => _consumeResult.Message.Value;
    }
}
