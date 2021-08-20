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

           var returnResult = new ConsumeResult<string, TestMessage>()
            {
                Message = new Message<string, TestMessage>
                {
                    Key = Guid.NewGuid().ToString(),
                    Value = new TestMessage()
                }
            };

            mockConsumer.Setup(x => x.Consume(Parameter.IsAny<int>()))
                .Returns(() => returnResult)
                .Callback(() => returnResult = null);

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
