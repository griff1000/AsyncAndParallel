namespace AsyncAndParallel
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    public class AsyncDoStuffThrowOccasionalException
    {
        public async Task StartIteration(int iteration, int delayDuration, string identifier)
        {
            await Task.Delay(delayDuration);
            if (iteration % 2 == 0) throw new ApplicationException("Can't stand those even bastards!");

            Console.WriteLine($"Async sleep iteration task call {iteration} complete for {identifier}");


        }
    }
}
