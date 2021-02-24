using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace NetStreams.Specs.Infrastructure.Extensions
{
    public static class TaskExtensions
    {
        public static async Task BlockUntil(this Task task, Func<bool> predicate, int timeout = 15)
        {
            var stopwatch = Stopwatch.StartNew();

            await task;

            while(!predicate())
            {
                if(stopwatch.Elapsed.TotalSeconds > timeout)
                {
                    break;
                }

                await Task.Delay(TimeSpan.FromSeconds(1));
            }
        }
    }
}
