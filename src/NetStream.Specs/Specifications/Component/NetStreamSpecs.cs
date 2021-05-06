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
using NetStreams.Internal.Exceptions;
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
                    .Returns((ConsumeResult<string, TestMessage>) null);

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
            static Task _streamTask;
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

                _stream.Handle(Console.WriteLine);

                _streamTask = _stream.StartAsync(_cancellationTokenSource.Token);
            };

            Because of = () => _actualException = Catch.Exception(() => _streamTask.Wait());

            It should_stop_the_task = () => _streamTask.Status.ShouldEqual(TaskStatus.Faulted);

            It should_report_an_exception_on_the_task = () => _streamTask.Exception.ShouldNotBeNull();

            It should_report_a_stream_exception_on_the_task = () => _streamTask.Exception.InnerExceptions.Single().ShouldBeAssignableTo(typeof(StreamFaultedException));

            It should_report_original_exception_on_the_task = () => _expectedException.ShouldMatch(((StreamFaultedException)_streamTask.Exception.InnerExceptions.Single()).InnerException);
        }

        [Subject("ErrorHandling")]
        class when_an_error_occurs_while_streaming_with_an_onerror
        {
            static NetStream<string, TestMessage> _stream;
            static CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
            static Mock<IConsumer<string, TestMessage>> _mockConsumer;
            static ExpectedObject _expectedException;
            static Exception _actualException;
            static Task _streamTask;

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

                _streamTask = _stream.StartAsync(_cancellationTokenSource.Token);
            };

            Because of = () => _streamTask.BlockUntil(() => _actualException != null).Await();

            It should_call_on_error_with_exception = () => _expectedException.ShouldMatch(_actualException);
        }
    }
}