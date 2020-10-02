namespace AsyncAndParallel
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    public class AsyncDelayLoop
    {
        public async Task StartDelayLoop(int count, int delayDuration, string identifier)
        {
            Console.WriteLine($"Starting async delay iteration loop for {identifier}");
            for (var i = 0; i < count; i++)
            {
                await StartIteration(i, delayDuration, identifier);
            }
        }

        public async Task StartIteration(int iteration, int delayDuration, string identifier)
        {
            await Task.Delay(delayDuration);
            Console.WriteLine($"Async delay iteration task call {iteration} complete for {identifier}");

            //For threading debug demo
            for (int i = 0; i < 10; i++)
            {
                var delayOfOne = i - 1 + 1001;
                await Task.Delay(500);
            }
        }
    }
}
