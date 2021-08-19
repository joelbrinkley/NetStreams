using System;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Confluent.Kafka;
using ExpectedObjects;
using Machine.Specifications;
using Moq;
using NetStreams.Configuration;
using NetStreams.Internal;
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
                    new NetStreamConfiguration<string, TestMessage>(),
                    mockConsumer.Object,
                    new NullTopicCreator());

                _startTask = netStream.StartAsync(_tokenSource.Token);
            };

            Because of = () =>
                Task.Run(() => _tokenSource.Cancel()).BlockUntil(() => _startTask.Status == TaskStatus.RanToCompletion)
                    .Await();

            It should_run_to_completion = () => _startTask.Status.ShouldEqual(TaskStatus.RanToCompletion);
        }

        [Subject("ErrorHandling")]
        class when_an_error_occurs_while_streaming_with_an_onerror
        {
            static NetStream<string, TestMessage> _stream;
            static CancellationTokenSource _cancellationTokenSource = new();
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

                var configuration = new NetStreamConfiguration<string, TestMessage>
                {
                    DeliveryMode = DeliveryMode.At_Least_Once
                };

                _stream = new NetStream<string, TestMessage>(
                    Guid.NewGuid().ToString(),
                    configuration,
                    _mockConsumer.Object,
                    new NullTopicCreator(),
                    null,
                    null,
                    ex =>
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