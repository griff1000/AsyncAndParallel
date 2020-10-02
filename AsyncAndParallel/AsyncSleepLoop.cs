namespace AsyncAndParallel
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    public class AsyncSleepLoop
    {
        public async Task StartSleepLoop(int count, int delayDuration, string identifier)
        {
            Console.WriteLine($"Starting async sleep iteration loop for {identifier}");
            for (var i = 0; i < count; i++)
            {
                await StartIteration(i, delayDuration, identifier);
            }
        }

        public Task StartIteration(int iteration, int delayDuration, string identifier)
        {
            Thread.Sleep(delayDuration);
            Console.WriteLine($"Async sleep iteration task call {iteration} complete for {identifier}");

            return Task.CompletedTask;
        }
    }
}
