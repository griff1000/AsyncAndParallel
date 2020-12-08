namespace AsyncAndParallel
{
    using System;
    using System.Threading.Tasks;

    /// <summary>
    /// Contains a single, non-blocking truly async method
    /// </summary>
    public class NonBlockingMethod
    {
        /// <summary>
        /// A non-blocking, truly async method
        /// </summary>
        /// <param name="iteration">Which iteration this is</param>
        /// <param name="delayDuration">How long (in ms) to delay for</param>
        /// <param name="identifier">Some text to describe this iteration uniquely</param>
        /// <param name="extraIterativeDelay">Whether or not to do some extra processing useful in a thread debugging demo
        /// - default is not to do that extra processing</param>
        public async Task StartIteration(int iteration, int delayDuration, string identifier, bool extraIterativeDelay = false)
        {
            await Task.Delay(delayDuration);  // think of this as an async call that takes considerable
                                              // time doing some I/O e.g. HttpClient.GetAsync(...)
            Console.WriteLine($"Async delay iteration task call {iteration} complete for {identifier}");

            //For threading debug demo only - not used by the other examples
            if (extraIterativeDelay)
            {
                for (var i = 0; i < 10; i++)
                {
                    var exampleDelayCalculation = i - 1 + 1001;  // could be anything, just something to set a breakpoint on
                    await Task.Delay(500 + exampleDelayCalculation - exampleDelayCalculation);
                }
            }
        }
    }
}
