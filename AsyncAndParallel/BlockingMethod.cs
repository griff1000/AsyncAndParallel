namespace AsyncAndParallel
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Contains a single, old-style blocking method
    /// </summary>
    public class BlockingMethod
    {
        /// <summary>
        /// A blocking method
        /// </summary>
        /// <param name="iteration">Which iteration this is</param>
        /// <param name="delayDuration">How long (in ms) to delay for</param>
        /// <param name="identifier">Some text to describe this iteration uniquely</param>
        public Task StartIteration(int iteration, int delayDuration, string identifier)
        {
            Thread.Sleep(delayDuration); // think of this as a non-async call that takes considerable
                                         // time doing some I/O e.g. HttpClient.Send(...)
            Console.WriteLine($"Async sleep iteration task call {iteration} complete for {identifier}");

            return Task.CompletedTask;
        }
    }
}
