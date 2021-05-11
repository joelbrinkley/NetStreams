using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NetStreams.Logging;

namespace NetStreams.Specs.Infrastructure.Mocks
{
    public class MockLog : ILog
    {
        public List<string> Messages { get; set; } = new List<string>();

        public void Information(string message)
        {
            Messages.Add(message);
        }

        public void Debug(string message)
        {
            Messages.Add(message);
        }

        public void Warn(string message)
        {
            Messages.Add(message);
        }

        public void Error(Exception exception, string message)
        {
            Messages.Add(message);
        }
    }
}
