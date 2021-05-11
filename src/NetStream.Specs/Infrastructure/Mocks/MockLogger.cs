using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NetStreams.Logging;

namespace NetStreams.Specs.Infrastructure.Mocks
{
    public class MockLogger : ILogger
    {
        public List<string> Messages { get; set; } = new List<string>();

        public void Write(string message)
        {
            Messages.Add(message);
        }
    }
}
