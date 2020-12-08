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
        /// <param name="allNonBlocking">If true, all operations will be truly async, else one operation will be blocking</param>
        public async Task StartIteration(int iteration, int delayDuration, string identifier, bool allNonBlocking=false)
        {
            await Task.Delay(delayDuration);
            Console.WriteLine($"Async delay task call {iteration} complete for {identifier}");

            for (var i = 0; i < 10; i++)
            {
                if (allNonBlocking)
                {
                    await Task.Delay(500);
                }
                else
                {
                    Thread.Sleep(500);
                }
            }
            Console.WriteLine($"Loop delay/sleep iteration task call {iteration} complete for {identifier}");
        }
    }
}
