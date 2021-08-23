using Confluent.Kafka;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using NetStreams.Internal;
using System.Threading;

namespace NetStreams
{
    public class NetStreamProducer<TKey, TMessage> : IMessageProducer<TKey, TMessage>
    {
        readonly string _topic;
        readonly IProducer<TKey, TMessage> _producer;
        readonly bool _enableMessageTypeHeader;

        public NetStreamProducer(string topic, IProducer<TKey, TMessage> producer, bool enableMessageTypeHeader = true)
        {
            _topic = topic;
            _producer = producer;
            _enableMessageTypeHeader = enableMessageTypeHeader;
        }

        public void Dispose()
        {
            _producer?.Dispose();
        }

        public async Task ProduceAsync(
            TKey key,
            TMessage message,
            List<KeyValuePair<string, string>> headers = null,
            CancellationToken token = default(CancellationToken))
        {
            var kafkaMessage = new Message<TKey, TMessage>()
            {
                Key = key,
                Value = message,
                Headers = new Headers()
            };

            if (headers != null) headers.ForEach(header => kafkaMessage.Headers.Add(new Header(header.Key, Encoding.UTF8.GetBytes(header.Value))));

            //TODO: Think about passing this in via method headers parameter
            if (_enableMessageTypeHeader)
            {
                kafkaMessage.Headers.Add(new Header(NetStreamConstants.HEADER_TYPE,
                    Encoding.UTF8.GetBytes(message.GetType().AssemblyQualifiedName.ToString())));
            }

            await _producer.ProduceAsync(_topic, kafkaMessage, token);
        }
    }

    public interface IMessageProducer<TKey, TMessage> : IDisposable
    {
        Task ProduceAsync(TKey key, TMessage message, List<KeyValuePair<string, string>> headers = null, CancellationToken token = default(CancellationToken));
    }

}
