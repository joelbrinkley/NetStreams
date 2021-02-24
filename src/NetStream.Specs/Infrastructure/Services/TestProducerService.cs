using System.Threading.Tasks;
using Confluent.Kafka;
using Machine.Specifications;
using NetStreams.Serialization;

namespace NetStreams.Specs.Infrastructure.Services
{
    public class TestProducerService<TMessage> where TMessage : IMessage
    {
        readonly string _topic;
        IMessageProducer<TMessage> _producer;

        public TestProducerService(string topic)
        {
            var kafkaProducer = new ProducerBuilder<string, TMessage>(new ProducerConfig() { BootstrapServers = "localhost:9092" })
                .SetValueSerializer(new JsonSer<TMessage>())
                .Build();

            _producer = new NetStreamProducer<TMessage>(kafkaProducer);
            _topic = topic;
        }

        public void Produce(TMessage message)
        {
            _producer.ProduceAsync(_topic, message).Await();
        }

        public async Task ProduceAsync(TMessage message)
        {
            await _producer.ProduceAsync(_topic, message);
        }
    }
}
