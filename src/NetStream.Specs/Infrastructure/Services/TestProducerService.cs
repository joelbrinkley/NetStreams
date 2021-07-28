using System;
using System.Threading.Tasks;
using Confluent.Kafka;
using Machine.Specifications;
using NetStreams.Serialization;

namespace NetStreams.Specs.Infrastructure.Services
{
    public class TestProducerService<TKey, TMessage>
    {
        readonly string _topic;
        IMessageProducer<TKey, TMessage> _producer;

        public TestProducerService(string topic, Action<ProducerConfig> configure)
        {
            var producerConfig = new ProducerConfig();
            configure(producerConfig);
            var kafkaProducer = new ProducerBuilder<TKey, TMessage>(producerConfig)
                .SetValueSerializer(new HeaderSerializationStrategy<TMessage>())
                .Build();

            _producer = new NetStreamProducer<TKey, TMessage>(topic, kafkaProducer);
            _topic = topic;
        }

        public void Produce(TKey key, TMessage message)
        {
            _producer.ProduceAsync(key, message).Await();
        }

        public async Task ProduceAsync(TKey key, TMessage message)
        {
            await _producer.ProduceAsync(key, message);
        }
    }
}
