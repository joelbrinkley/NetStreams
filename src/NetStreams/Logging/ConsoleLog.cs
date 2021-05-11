using System;
using System.Collections.Generic;
using System.Text;
using Confluent.Kafka.Admin;

namespace NetStreams.Logging
{
    public class ConsoleLog : ILog
    {
        public void Information(string message)
        {
            Console.WriteLine(message);
        }

        public void Debug(string message)
        {
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine(message);
            Console.ResetColor();
        }

        public void Warn(string message)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(message);
            Console.ResetColor();
        }

        public void Error(Exception exception, string message)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(message);
            Console.WriteLine(exception.Message);
            Console.WriteLine(exception.StackTrace);
            Console.ResetColor();

        }
    }
}
