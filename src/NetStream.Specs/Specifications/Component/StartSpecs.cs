using Confluent.Kafka;
using Machine.Specifications;
using NetStreams.Specs.Infrastructure;
using System;
using System.Threading;
using System.Threading.Tasks;
using NetStreams.Configuration;
using NetStreams.Specs.Infrastructure.Extensions;
using NetStreams.Specs.Infrastructure.Mocks;
using NetStreams.Specs.Infrastructure.Models;

namespace NetStreams.Specs.Specifications.Component
{
    class StartSpecs
    {
        [Subject("Start")]
        class when_the_start_task_is_canceled
        {
            static CancellationTokenSource _tokenSource;
            static Task _startTask;

            Establish context = () =>
            {
                _tokenSource = new CancellationTokenSource();

                var mockConsumer = new Moq.Mock<IConsumer<string, TestMessage>>();

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

            Because of = () => Task.Run(() => _tokenSource.Cancel()).BlockUntil(() => _startTask.Status == TaskStatus.RanToCompletion).Await();

            It should_run_to_completion = () => _startTask.Status.ShouldEqual(TaskStatus.RanToCompletion);
        }
    }
}
