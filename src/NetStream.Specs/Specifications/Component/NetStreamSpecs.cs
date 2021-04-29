using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Confluent.Kafka;
using ExpectedObjects;
using Machine.Specifications;
using Moq;
using NetStreams.Configuration;
using NetStreams.Exceptions;
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
            static List<TestMessage> _testMessages = new List<TestMessage>();
            static Mock<IConsumer<string, TestMessage>> _mockConsumer;
            static NetStream<string, TestMessage> _stream;

            Establish context = () =>
            {
                _mockConsumer = new Mock<IConsumer<string, TestMessage>>();

                _mockConsumer
                    .Setup(x => x.Consume(Parameter.IsAny<int>()))
                    .Returns(() =>
                    {
                        if (_testMessages.Any())
                            return new ConsumeResult<string, TestMessage>
                            {
                                Message = new Message<string, TestMessage>
                                {
                                    Key = _messageAdded.Id,
                                    Value = _messageAdded
                                }
                            };

                        return null;
                    })
                    .Callback(() => _cancellationTokenSource.Cancel());

                var mockProducer = new Mock<IProducer<string, TestMessage>>();
                var consumerFactoryMock = new TestConsumerFactory(_mockConsumer);
                var configuration = new NetStreamConfiguration
                {
                    DeliveryMode = DeliveryMode.At_Least_Once
                };

                _stream = new NetStream<string, TestMessage>(
                    Guid.NewGuid().ToString(),
                    configuration,
                    consumerFactoryMock,
                    new NullTopicCreator());

                _testMessages.Add(_messageAdded);
            };

            Because of = () =>
                _stream.Handle(x => _testMessages.Clear()).StartAsync(_cancellationTokenSource.Token)
                    .BlockUntil(() => _testMessages.Count == 0).Await();

            It should_commit = () => _mockConsumer.Verify(x => x.Commit(), Times.Exactly(1));
        }

        [Subject("ErrorHandling")]
        class when_an_error_occurs_while_streaming
        {
            static NetStream<string, TestMessage> _stream;
            static CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
            static Mock<IConsumer<string, TestMessage>> _mockConsumer;
            static ExpectedObject _expectedException;
            static Exception _actualException;

            Establish context = () =>
            {
                _mockConsumer = new
                    Mock<IConsumer<string, TestMessage>>();

                var exceptionToThrow = new Exception("Boom!");
                _expectedException = exceptionToThrow.ToExpectedObject();

                _mockConsumer
                    .Setup(x => x.Consume(Parameter.IsAny<int>()))
                    .Throws(exceptionToThrow);

                var consumerFactoryMock = new TestConsumerFactory(_mockConsumer);
                var configuration = new NetStreamConfiguration
                {
                    DeliveryMode = DeliveryMode.At_Least_Once
                };

                _stream = new NetStream<string, TestMessage>(Guid.NewGuid().ToString(), configuration,
                    consumerFactoryMock, new NullTopicCreator());

                _stream
                    .Handle(x => Console.WriteLine(x))
                    .OnError(ex =>
                    {
                        _actualException = ex;
                        _cancellationTokenSource.Cancel();
                    });
            };

            Because of = () =>
                _stream.StartAsync(_cancellationTokenSource.Token).BlockUntil(() => _actualException != null).Await();

            It should_call_on_error_with_expected_exception = () => _expectedException.ShouldMatch(_actualException);
        }

        [Subject("ErrorHandling")]
        class when_an_error_occurs_while_streaming_with_no_handler_defined
        {
            static NetStream<string, TestMessage> _stream;
            static CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
            static Mock<IConsumer<string, TestMessage>> _mockConsumer;
            static ExpectedObject _expectedInnerException;
            static Exception _actualException;

            Establish context = () =>
            {
                _mockConsumer = new
                    Mock<IConsumer<string, TestMessage>>();

                var innerException = new Exception("Boom!");
                _expectedInnerException = innerException.ToExpectedObject();

                _mockConsumer
                    .Setup(x => x.Consume(Parameter.IsAny<int>()))
                    .Throws(innerException);

                var consumerFactoryMock = new TestConsumerFactory(_mockConsumer);
                var configuration = new NetStreamConfiguration
                {
                    DeliveryMode = DeliveryMode.At_Least_Once
                };

                _stream = new NetStream<string, TestMessage>(
                    Guid.NewGuid().ToString(),
                    configuration,
                    consumerFactoryMock, new NullTopicCreator());

                _stream.Handle(x => Console.WriteLine(x));
            };

            Because of = () => _actualException = Catch.Exception(() => _stream.StartAsync(_cancellationTokenSource.Token).BlockUntil(() => _actualException != null).Await());
            
            It should_throw_the_exception = () => _actualException.GetType().ShouldEqual(typeof(NetStreamsException));

            It should_have_inner_exception = () => _expectedInnerException.ShouldMatch(_actualException.InnerException);
        }
    }
}