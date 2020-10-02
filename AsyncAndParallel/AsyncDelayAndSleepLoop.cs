namespace AsyncAndParallel
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    public class AsyncDelayAndSleepLoop
    {
        public async Task StartIteration(int iteration, int delayDuration, string identifier)
        {
            await Task.Delay(delayDuration);
            Console.WriteLine($"Async sleep iteration task call {iteration} complete for {identifier}");

            for (int i = 0; i < 10; i++)
            {
                var delayOfOne = i - 1 + 1001;
                Thread.Sleep(500 + delayOfOne - delayOfOne);
            }

        }
    }
}
