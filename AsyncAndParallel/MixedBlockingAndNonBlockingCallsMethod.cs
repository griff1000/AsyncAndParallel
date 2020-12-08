namespace AsyncAndParallel
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Contains a single method containing a mixture of blocking and non-blocking calls
    /// </summary>
    public class MixedBlockingAndNonBlockingCallsMethod
    {
        /// <summary>
        /// Contains a mixture of blocking and non-blocking calls
        /// </summary>
        /// <param name="iteration">Which iteration this is</param>
        /// <param name="delayDuration">How long (in ms) to delay for</param>
        /// <param name="identifier">Some text to describe this iteration uniquely</param>
        public async Task StartIteration(int iteration, int delayDuration, string identifier)
        {
            await Task.Delay(delayDuration);
            Console.WriteLine($"Async sleep iteration task call {iteration} complete for {identifier}");

            for (var i = 0; i < 10; i++)
            {
                var exampleDelayCalculation = i - 1 + 1001; // could be anything, just something to set a breakpoint on
                Thread.Sleep(500 + exampleDelayCalculation - exampleDelayCalculation);
            }
        }
    }
}
