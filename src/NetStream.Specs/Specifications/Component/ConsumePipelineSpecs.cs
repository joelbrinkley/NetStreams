using System;
using System.Threading;
using Confluent.Kafka;
using Machine.Specifications;
using Moq;
using NetStreams.Internal;
using NetStreams.Specs.Infrastructure.Mocks;
using NetStreams.Specs.Infrastructure.Models;
using It = Machine.Specifications.It;

namespace NetStreams.Specs.Specifications.Component
{
    internal class ConsumePipelineSpecs
    {
        [Subject("AppendStep")]
        class when_appending_multiple_steps
        {
            static ConsumePipeline<string, string> _pipeline;
            static string _log = "";
            static TestHandleStep<string, string> _handle1;
            static TestHandleStep<string, string> _handle2;
            static TestHandleStep<string, string> _handle3;

            Establish context = () =>
            {
                _pipeline = new ConsumePipeline<string, string>();
                _handle1 = new TestHandleStep<string, string>("Handle1", context => _log += "Handle1 Succeeded; ");
                _handle2 = new TestHandleStep<string, string>("Handle2", context => _log += "Handle2 Succeeded; ");
                _handle3 = new TestHandleStep<string, string>("Handle3", context => _log += "Handle3 Succeeded; ");
                _pipeline.AppendStep(_handle1);
                _pipeline.AppendStep(_handle2);
            };

            Because of = () => _pipeline.AppendStep(_handle3);

            It should_have_three_after_two = () =>
                ((TestHandleStep<string, string>) _handle2.Next).Name.ShouldEqual("Handle3");

            It should_have_two_after_one = () =>
                ((TestHandleStep<string, string>) _handle1.Next).Name.ShouldEqual("Handle2");
        }

        [Subject("PrependStep")]
        class when_prepending_multiple_steps
        {
            static ConsumePipeline<string, string> _pipeline;
            static string _log = "";
            static TestHandleStep<string, string> _handle1;
            static TestHandleStep<string, string> _handle2;
            static TestHandleStep<string, string> _handle3;

            Establish context = () =>
            {
                _pipeline = new ConsumePipeline<string, string>();
                _handle1 = new TestHandleStep<string, string>("Handle1", context => _log += "Handle1 Succeeded; ");
                _handle2 = new TestHandleStep<string, string>("Handle2", context => _log += "Handle2 Succeeded; ");
                _handle3 = new TestHandleStep<string, string>("Handle3", context => _log += "Handle3 Succeeded; ");
                _pipeline.PrependStep(_handle1);
                _pipeline.PrependStep(_handle2);
            };

            Because of = () => _pipeline.PrependStep(_handle3);

            It should_have_one_after_two = () =>
                ((TestHandleStep<string, string>) _handle2.Next).Name.ShouldEqual("Handle1");

            It should_have_two_after_three = () =>
                ((TestHandleStep<string, string>) _handle3.Next).Name.ShouldEqual("Handle2");
        }

        [Subject("ExecuteAsync")]
        class when_executing_the_pipeline
        {
            static ConsumePipeline<string, string> _pipeline;
            static string _log = "";
            static TestHandleStep<string, string> _handle1;
            static TestHandleStep<string, string> _handle2;
            static TestHandleStep<string, string> _handle3;
            static IConsumeContext<string, string> _testResult;

            Establish context = () =>
            {
                _pipeline = new ConsumePipeline<string, string>();
                _handle1 = new TestHandleStep<string, string>("Handle1", context => _log += "Handle1 Succeeded; ");
                _handle2 = new TestHandleStep<string, string>("Handle2", context => _log += "Handle2 Succeeded; ");
                _handle3 = new TestHandleStep<string, string>("Handle3", context => _log += "Handle3 Succeeded; ");
                _pipeline.AppendStep(_handle1);
                _pipeline.AppendStep(_handle2);
                _pipeline.AppendStep(_handle3);

                var consumeResult = new ConsumeResult<string, string>
                {
                    Message = new Message<string, string>
                    {
                        Key = Guid.NewGuid().ToString(),
                        Value = Guid.NewGuid().ToString()
                    }
                };

                _testResult = new ConsumeContext<string, string>(
                    consumeResult,
                    new Mock<IConsumer<string, string>>().Object,
                    Guid.NewGuid().ToString());

            };

            Because of = () => _pipeline.ExecuteAsync(_testResult, CancellationToken.None).Wait();

            It should_have_Handle2_in_the_run_log = () => _log.IndexOf("Handle2").ShouldBeGreaterThan(-1);
        }
    }
}