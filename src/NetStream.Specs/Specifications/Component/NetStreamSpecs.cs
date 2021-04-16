using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Confluent.Kafka;
using Machine.Specifications;
using Moq;
using NetStreams.Configuration;
using NetStreams.Specs.Infrastructure;
using NetStreams.Specs.Infrastructure.Extensions;
using NetStreams.Specs.Infrastructure.Mocks;
using NetStreams.Specs.Infrastructure.Models;
using It = Machine.Specifications.It;

namespace NetStreams.Specs.Specifications.Component
{
    internal class NetStreamSpecs
    {
        [Subject("Start")]
        class when_the_start_task_is_canceled
        {
            static CancellationTokenSource _tokenSource;
            static Task _startTask;

            Establish context = () =>
            {
                _tokenSource = new CancellationTokenSource();

                var mockConsumer = new Mock<IConsumer<string, TestMessage>>();

                mockConsumer.Setup(x => x.Consume(Parameter.IsAny<int>()))
                    .Returns((ConsumeResult<string, TestMessage>)null);

                var netStream = new NetStream<string, TestMessage>(
                    Guid.NewGuid().ToString(),
                    new NetStreamConfiguration(),
                    new TestConsumerFactory(mockConsumer),
                    new TestProducerFactory(null),
                    new NullTopicCreator());

                _startTask = netStream.StartAsync(_tokenSource.Token);
            };

            Because of = () =>
                Task.Run(() => _tokenSource.Cancel()).BlockUntil(() => _startTask.Status == TaskStatus.RanToCompletion)
                    .Await();

            It should_run_to_completion = () => _startTask.Status.ShouldEqual(TaskStatus.RanToCompletion);
        }

        [Subject("Committing")]
        class after_an_at_least_once_stream_handles_a_message
        {
            static CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
            static TestMessage _messageAdded = new TestMessage();
            static List<TestMessage> testMessages = new List<TestMessage>();
            static Mock<IConsumer<string, TestMessage>> _mockConsumer;
            static NetStream<string, TestMessage> _stream;

            Establish context = () =>
            {
                _mockConsumer = new Mock<IConsumer<string, TestMessage>>();

                _mockConsumer
                    .Setup(x => x.Consume(Parameter.IsAny<int>()))
                    .Returns(() =>
                    {
                        if (testMessages.Any())
                        {
                            return new ConsumeResult<string, TestMessage>
                            {
                                Message = new Message<string, TestMessage>
                                {
                                    Key = _messageAdded.Key,
                                    Value = _messageAdded
                                }
                            };
                        }

                        return null;
                    })
                    .Callback(() => _cancellationTokenSource.Cancel());

                var mockProducer = new Mock<IProducer<string, TestMessage>>();
                var consumerFactoryMock = new TestConsumerFactory(_mockConsumer);
                var configuration = new NetStreamConfiguration()
                {
                    DeliveryMode = DeliveryMode.At_Least_Once
                };

                var stream = new NetStream<string, TestMessage>(Guid.NewGuid().ToString(), configuration,
                    consumerFactoryMock, new TestProducerFactory(mockProducer), new NullTopicCreator());

                stream.Handle(x => testMessages.Clear()).StartAsync(_cancellationTokenSource.Token);
            };

            Because of = () => Task.Run(() => testMessages.Add(_messageAdded)).BlockUntil(() => testMessages.Count == 0).Await();

            It should_commit = () => _mockConsumer.Verify(x => x.Commit(), Times.Exactly(1));
        }
    }
}