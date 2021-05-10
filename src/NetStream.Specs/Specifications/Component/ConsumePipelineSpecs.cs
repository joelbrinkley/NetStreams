using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Machine.Specifications;
using Machine.Specifications.Model;
using NetStreams.Internal;
using NetStreams.Specs.Infrastructure.Models;

namespace NetStreams.Specs.Specifications.Component
{
    class ConsumePipelineSpecs
    {
        [Subject("AppendStep")]
        class when_appending_multiple_steps
        {
            static ConsumePipeline<string, string> _pipeline;
            static string _log = "";
            static TestHandleStep<string, string> _handle1;
            static TestHandleStep<string, string> _handle2;
            static TestHandleStep<string, string> _handle3;

            private Establish context = () =>
            {
                _pipeline = new ConsumePipeline<string, string>();
                _handle1 = new TestHandleStep<string, string>("Handle1", context => _log += "Handle1 Succeeded; ");
                _handle2 = new TestHandleStep<string, string>("Handle2", context => _log += "Handle2 Succeeded; ");
                _handle3 = new TestHandleStep<string, string>("Handle3", context => _log += "Handle3 Succeeded; ");
                _pipeline.AppendStep(_handle1);
                _pipeline.AppendStep(_handle2);
            };

            Because of = () => _pipeline.AppendStep(_handle3);

            private It should_have_two_after_one = () =>
                ((TestHandleStep<string, string>)_handle1.Next).Name.ShouldEqual("Handle2");

            private It should_have_three_after_two = () =>
                ((TestHandleStep<string, string>)_handle2.Next).Name.ShouldEqual("Handle3");

            //private It should_have_two_in_log = () => _log.ShouldContain("Handle2");
        }
    }
}
