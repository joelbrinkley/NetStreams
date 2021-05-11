using System;
using System.Collections.Generic;
using System.Text;

namespace NetStreams.Logging
{
    public class ConsoleLogger : ILogger
    {
        public void Write(string message)
        {
            Console.WriteLine(message);
        }
    }
}
