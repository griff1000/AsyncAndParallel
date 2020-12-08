namespace AsyncAndParallel
{
    using System;
    using System.Threading.Tasks;

    /// <summary>
    /// Contains a single, non-blocking truly async method that throws exceptions every even-numbered iteration
    /// </summary>
    public class NonBlockingButThrowsOccasionalException
    {
        /// <summary>
        /// Fully async method that throws an ApplicationException every even-numbered iteration
        /// </summary>
        /// <param name="iteration">Which iteration this is</param>
        /// <param name="delayDuration">How long (in ms) to delay for</param>
        /// <param name="identifier">Some text to describe this iteration uniquely</param>
        /// <returns></returns>
        public async Task StartIteration(int iteration, int delayDuration, string identifier)
        {
            await Task.Delay(delayDuration);
            if (iteration % 2 == 0) throw new ApplicationException($"Can't stand those even bastards! Take your {iteration} and bugger off!");

            Console.WriteLine($"Async sleep iteration task call {iteration} complete for {identifier}");
        }
    }
}
