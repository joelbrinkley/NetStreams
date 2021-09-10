using Confluent.Kafka;
using Moq;
using NetStreams.Specs.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetStreams.Specs.Infrastructure.Mocks
{
    internal class MockConsumer
    {
        internal static Mock<IConsumer<string, TestMessage>> SetupToConsumeSingleTestMessage()
        {
            var mockConsumer = new Mock<IConsumer<string, TestMessage>>();

            var topic = Guid.NewGuid().ToString();
            var returnResult = new ConsumeResult<string, TestMessage>()
            {
                Message = new Message<string, TestMessage>
                {
                    Key = Guid.NewGuid().ToString(),
                    Value = new TestMessage(),
                    Headers = new Headers()
                },
                Topic = topic,
                TopicPartitionOffset = new TopicPartitionOffset(new TopicPartition(topic, new Partition(1)), new Offset(10))
            };

            mockConsumer.Setup(x => x.Consume(Parameter.IsAny<int>()))
                .Returns(() => returnResult)
                .Callback(() => returnResult = null);

            mockConsumer.Setup(x => x.GetWatermarkOffsets(Parameter.IsAny<TopicPartition>()))
                .Returns(() => new WatermarkOffsets(new Offset(10), new Offset(20)));

            return mockConsumer;
        }

        internal static Mock<IConsumer<string, TestMessage>> SetupToThrowError(Exception toThrow)
        {
            var mockConsumer = new
            Mock<IConsumer<string, TestMessage>>();

            mockConsumer
                .Setup(x => x.Consume(Parameter.IsAny<int>()))
                .Throws(toThrow);

            return mockConsumer;
        }
    }
}
