﻿using Confluent.Kafka;

namespace NetStreams
{
    public class ConsumeContext<TKey, TMessage> : IConsumeContext<TKey, TMessage>
    {
        readonly ConsumeResult<TKey, TMessage> _consumeResult;
        readonly IConsumer<TKey, TMessage> _consumer;

        public ConsumeContext(ConsumeResult<TKey, TMessage> consumeResult, IConsumer<TKey, TMessage> consumer, string consumerGroup)
        {
            _consumeResult = consumeResult;
            _consumer = consumer;
            ConsumeGroup = consumerGroup;
        }

        public string ConsumeGroup { get; }
        public TMessage Message => _consumeResult.Message.Value;
        public string TopicName => _consumeResult.Topic;
        public int Partition => _consumeResult.TopicPartition.Partition.Value;
        public long Offset => _consumeResult.Offset.Value;
        public long Lag => _consumer.GetWatermarkOffsets(_consumeResult.TopicPartition).High.Value - _consumeResult.TopicPartitionOffset.Offset.Value - 1;
        public TKey Key => _consumeResult.Message.Key;

    }
}
