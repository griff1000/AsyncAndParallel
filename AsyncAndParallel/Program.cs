namespace AsyncAndParallel
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;

    internal class Program
    {
        private static ConsoleColor _normalColour;
        private const int DelayDurationOne = 500;
        private const int IterationCount = 10;

        private static async Task Main(string[] args)
        {
            _normalColour = Console.ForegroundColor;
            var shouldContinue = string.Empty;

            while (string.IsNullOrEmpty(shouldContinue) || shouldContinue.Substring(0, 1).ToLower() != "q")
            {
                var chosenTests = GetTests();
                foreach (var chosenTest in chosenTests)
                {
                    switch (chosenTest)
                    {
                        case TestsEnum.SleepLoopTests:
                            await SleepLoopTests();
                            break;
                        case TestsEnum.DelayLoopTests:
                            await DelayLoopTests();
                            break;
                        case TestsEnum.ParallelForEachSleepLoopTests:
                            await ParallelForEachSleepLoopTests();
                            break;
                        case TestsEnum.ParallelForEachDelayLoopTests:
                            await ParallelForEachDelayLoopTests();
                            break;
                        case TestsEnum.ParallelForEachDelayLoopWithAsyncTests:
                            await ParallelForEachDelayLoopWithAsyncTests();
                            break;
                        case TestsEnum.ParallelForEachDelayLoopWithAsyncAttachedTests:
                            await ParallelForEachDelayLoopWithAsyncAttachedTests();
                            break;
                        case TestsEnum.TaskWhenAllDelayLoopTests:
                            await TaskWhenAllDelayLoopTests();
                            break;
                        case TestsEnum.TaskWaitAllDelayLoopTests:
                            await TaskWaitAllDelayLoopTests();
                            break;
                        case TestsEnum.TaskWhenAllSleepAndDelayLoopTests:
                            await TaskWhenAllSleepAndDelayLoopTests();
                            break;
                        case TestsEnum.TaskWhenAllHugeNumberOfTasksDelayLoopTests:
                            await TaskWhenAllHugeNumberOfTasksDelayLoopTests();
                            break;
                        case TestsEnum.TaskWhenAllExceptionHandling:
                            await TaskWhenAllExceptionHandling();
                            break;
                        case TestsEnum.TaskWaitAllExceptionHandling:
                            await TaskWaitAllExceptionHandling();
                            break;
                        case TestsEnum.DemoThreadDebugging:
                            await DemoThreadDebugging();
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }

                Console.WriteLine();
                Console.WriteLine("Test run complete.  Enter q or quit to finish or anything else to try again");
                shouldContinue = Console.ReadLine();
            }
        }

        #region helpers
        private static Stopwatch BeginExample(string message)
        {
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine(message);
            Console.ForegroundColor = _normalColour;
            return Stopwatch.StartNew();
        }
        private static TestsEnum[] GetTests()
        {
            string input;
            var testNames = Enum.GetNames(typeof(TestsEnum));
            List<TestsEnum> chosenTests;
            while (true)
            {
                chosenTests = new List<TestsEnum>();
                Console.Clear();
                Console.WriteLine("Enter a comma-delimited list of numbers for which tests you want to run:");
                var i = 1;
                foreach (var testName in testNames)
                {
                    Console.WriteLine($"{i++}: {testName}");
                }

                input = Console.ReadLine();
                if (string.IsNullOrEmpty(input)) continue; // try again

                var pattern = @"^\d{1,3}(?:,\d{1,3})*$";
                var regex = new Regex(pattern, RegexOptions.IgnorePatternWhitespace);
                if (string.IsNullOrEmpty(input) || !regex.IsMatch(input))
                    continue; // try again
                try
                {
                    foreach (var choice in input.Split(','))
                    {
                        var arrayOffset = int.Parse(choice) - 1;
                        if (arrayOffset < 0 || arrayOffset >= testNames.Length)
                            throw new ArgumentOutOfRangeException("Input out of range");

                        var chosenTestName = testNames[arrayOffset];
                        var chosenTest = (TestsEnum)Enum.Parse(typeof(TestsEnum), chosenTestName);
                        chosenTests.Add(chosenTest);
                    }
                }
                catch (ArgumentOutOfRangeException)
                {
                    continue; // try again
                }

                break; // Got the array of values so return
            }

            return chosenTests.ToArray();
        }

        #endregion

        /// <summary>
        /// Loops around a blocking method trying to do it asynchronously (but it's blocking so it can't)
        /// with no parallelism
        /// </summary>
        private static async Task SleepLoopTests()
        {
            var sw = BeginExample("SleepLoopTests starting...");

            var sut = new BlockingMethod();
            for (var i = 0; i < IterationCount; i++)
            {
                await sut.StartIteration(i, DelayDurationOne, "One");
            }
            sw.Stop();

            Console.WriteLine($"Duration of async sleep iteration loop for {IterationCount} iterations sleeping for {DelayDurationOne} ms was {sw.ElapsedMilliseconds} ms");
        }

        /// <summary>
        /// Loops around a non-blocking method asynchronously but with no parallelism
        /// </summary>
        private static async Task DelayLoopTests()
        {
            var sw = BeginExample("DelayLoopTests starting...");

            var sut = new NonBlockingMethod();
            for (var i = 0; i < IterationCount; i++)
            {
                await sut.StartIteration(i, DelayDurationOne, "One");
            }
            sw.Stop();
            Console.WriteLine($"Duration of async delay iteration loop for {IterationCount} iterations delay for {DelayDurationOne} ms was {sw.ElapsedMilliseconds} ms");
        }

        /// <summary>
        /// Does a parallel ForEach around a blocking method
        /// </summary>
        /// <remarks>Since the method is blocking (even though it returns a task) the Console.WriteLine won't
        /// be executed till all calls have been completed in parallel</remarks>
        private static Task ParallelForEachSleepLoopTests()
        {
            var sw = BeginExample("ParallelForEachSleepLoopTests starting...");

            var array = new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
            var sut = new BlockingMethod();
            Parallel.ForEach(array, (i, state) => sut.StartIteration(i, DelayDurationOne, "ParallelForEachSleepLoopTests"));
            sw.Stop();
            Console.WriteLine($"Duration of Parallel.ForEach sleep iteration loop for {array.Length} iterations sleep for {DelayDurationOne} ms was {sw.ElapsedMilliseconds} ms");
            return Task.CompletedTask;
        }

        /// <summary>
        /// Does a parallel ForEach around a non-blocking method
        /// </summary>
        /// <remarks>Since the method is non-blocking and will therefore return a task instantly the Console.WriteLine will be
        /// executed almost immediately, before any of the the tasks have had a chance to complete</remarks>
        private static Task ParallelForEachDelayLoopTests()
        {
            var sw = BeginExample("ParallelForEachDelayLoopTests starting...");

            var array = new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
            var sut = new NonBlockingMethod();
            Parallel.ForEach(array, (i, state) => sut.StartIteration(i, DelayDurationOne, "ParallelForEachDelayLoopTests"));
            
            sw.Stop();
            Console.WriteLine($"Duration of Parallel.ForEach delay iteration loop for {array.Length} iterations delay for {DelayDurationOne} ms was {sw.ElapsedMilliseconds} ms");
            return Task.CompletedTask;
        }

        /// <summary>
        /// Does a parallel ForEach around a non-blocking method with async/await
        /// </summary>
        /// <remarks>Even though Parallel.ForEach is being used with the async/await operators, it's not doing what
        /// you expect; it's still not actually awaiting the tasks on the current thread so the Console.WriteLine
        /// is executing much earlier than you would expect, before any of the method calls have completed.
        /// NB even though Parallel.ForEach uses async/await in its lambda, it is not itself async so we
        /// do not need to mark this test method as async</remarks>
        private static Task ParallelForEachDelayLoopWithAsyncTests()
        {
            var sw = BeginExample("ParallelForEachDelayLoopWithAsyncTests starting...");

            var array = new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
            var sut = new NonBlockingMethod();
            Parallel.ForEach(array, async (i, state) => await sut.StartIteration(i, DelayDurationOne, "ParallelForEachDelayLoopWithAsyncTests"));
            
            sw.Stop();
            Console.WriteLine($"Duration of Parallel.ForEach delay iteration loop for {array.Length} iterations delay for {DelayDurationOne} ms was {sw.ElapsedMilliseconds} ms");
            return Task.CompletedTask;
        }

        /// <summary>
        /// Does a parallel ForEach around a non-blocking method with async/await AND attaches the tasks to the parent
        /// </summary>
        /// <remarks>Even though Parallel.ForEach is being used with the async/await operators and attaching tasks
        /// to the parent thread, it's still not doing what you expect; the Console.WriteLine
        /// is still executing much earlier than you would expect, before any of the method calls have completed.
        /// This may be a badly written test; I *THINK* what's happening here is that the Task.Factory.StartNew
        /// creates a single new task (on a new thread) that does have the parent set correctly; however that
        /// thread then runs the Parallel.ForEach which kicks off a bunch of new tasks, each on their own thread,
        /// which aren't connected to the parent as you'd hope.</remarks>
        
        private static async Task ParallelForEachDelayLoopWithAsyncAttachedTests()
        {
            var sw = BeginExample("ParallelForEachDelayLoopWithAsyncAttachedTests starting...");

            var array = new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
            var sut = new NonBlockingMethod();

            await Task.Factory.StartNew(() =>
            {
                Parallel.ForEach(array,
                    async (i, state) => await sut.StartIteration(i, DelayDurationOne, "ParallelForEachDelayLoopWithAsyncAttachedTests"));
            }, TaskCreationOptions.AttachedToParent);
            
            sw.Stop();
            Console.WriteLine($"Duration of attached Parallel.ForEach delay iteration loop for {array.Length} iterations delay for {DelayDurationOne} ms was {sw.ElapsedMilliseconds} ms");
        }

        /// <summary>
        /// Kicks off a number or tasks but instead of awaiting them it adds the tasks to a <see cref="List{Task}"/>
        /// and then does a Task.WhenAll on that list.
        /// </summary>
        /// <remarks>Now the Console.WriteLine executes when we expect it to, after all tasks have completed.  Those
        /// tasks have been run in parallel though, since we weren't awaiting any individual task, just waiting
        /// for them all to have completed.</remarks>
        private static async Task TaskWhenAllDelayLoopTests()
        {
            var sw = BeginExample("TaskWhenAllDelayLoopTests starting...");

            var sut = new NonBlockingMethod();
            var taskList = new List<Task>();
            
            for (var i = 1; i <= IterationCount; i++)
            {
                taskList.Add(sut.StartIteration(i, DelayDurationOne, "TaskWhenAllDelayLoopTests"));
            }
            await Task.WhenAll(taskList.ToArray());

            sw.Stop();
            Console.WriteLine($"Duration of Task.WhenAll delay iteration loop for {IterationCount} iterations delay for {DelayDurationOne} ms was {sw.ElapsedMilliseconds} ms");
        }

        /// <summary>
        /// Kicks off a number or tasks but instead of awaiting them it adds the tasks to a <see cref="List{Task}"/>
        /// and then does a Task.WaitAll on that list.
        /// </summary>
        /// <remarks>Now the Console.WriteLine executes when we expect it to, after all tasks have completed.  Those
        /// tasks have been run in parallel though, since we weren't awaiting any individual task, just waiting
        /// for them all to have completed.</remarks>
        private static Task TaskWaitAllDelayLoopTests()
        {
            var sw = BeginExample("TaskWaitAllDelayLoopTests starting...");

            var sut = new NonBlockingMethod();
            var taskList = new List<Task>();
            
            for (var i=1; i <= IterationCount; i++)
            {
                taskList.Add(sut.StartIteration(i, DelayDurationOne, "TaskWaitAllDelayLoopTests"));
            }
            Task.WaitAll(taskList.ToArray());

            sw.Stop();
            Console.WriteLine($"Duration of Task.WaitAll delay iteration loop for {IterationCount} iterations delay for {DelayDurationOne} ms was {sw.ElapsedMilliseconds} ms");
            return Task.CompletedTask;
        }

        /// <summary>
        /// Kicks off a fairly large number of tasks containing a mixture of blocking and non-blocking calls
        /// </summary>
        /// <remarks>The first few tasks are created in parallel but then due to the blocking calls the
        /// thread pool gets exhausted so subsequent tasks have to wait till previous ones have been
        /// completed</remarks>
        private static async Task TaskWhenAllSleepAndDelayLoopTests()
        {
            var sw = BeginExample("TaskWhenAllSleepAndDelayLoopTests starting...");

            const int count = 100;
            var sut = new MixedBlockingAndNonBlockingCallsMethod();
            var taskList = new List<Task>();

            for (var i=1; i <= count; i++)
            {
                taskList.Add(sut.StartIteration(i, DelayDurationOne, "TaskWhenAllSleepAndDelayLoopTests"));
            }
            await Task.WhenAll(taskList.ToArray());

            sw.Stop();
            Console.WriteLine($"Duration of Task.WhenAll delay iteration loop for {count} iterations delay for {DelayDurationOne * 10} ms was {sw.ElapsedMilliseconds} ms");
        }

        /// <summary>
        /// Kicks off a very large number of tasks containing all non-blocking calls
        /// </summary>
        /// <remarks>Since each call is non-blocking, the thread becomes available to process other tasks straight
        /// away so the thread pool doesn't get exhausted and you get massive throughput.  Once complete, each task
        /// can get picked up again by a random thread, not necessarily the one it started on.</remarks>
        private static async Task TaskWhenAllHugeNumberOfTasksDelayLoopTests()
        {
            var sw = BeginExample("TaskWhenAllHugeNumberOfTasksDelayLoopTests starting...");

            const int count = 10000;
            var sut = new NonBlockingMethod();
            var taskList = new List<Task>();

            for (var i=1; i <= count; i++)
            {
                taskList.Add(sut.StartIteration(i, DelayDurationOne, "TaskWhenAllHugeNumberOfTasksDelayLoopTests", true));
            }
            await Task.WhenAll(taskList.ToArray());

            sw.Stop();
            Console.WriteLine($"Duration of Task.WhenAll delay iteration loop for {count} iterations delay for {DelayDurationOne} ms was {sw.ElapsedMilliseconds} ms");
        }

        /// <summary>
        /// Kicks off a bunch of non-blocking tasks, half of which will throw exceptions
        /// </summary>
        /// <remarks>All tasks run; of those which throw exceptions, only the first exception is returned.  All other
        /// tasks run until completion in splendid isolation</remarks>
        private static async Task TaskWhenAllExceptionHandling()
        {
            BeginExample("TaskWhenAllExceptionHandling starting...");

            var taskList = new List<Task>();
            var sut = new NonBlockingButThrowsOccasionalException();
            for (int i = 0; i< IterationCount; i++)
            {
                taskList.Add(sut.StartIteration(i, 1, $"TaskWhenAllExceptionHandling iteration {i}"));
            }

            try
            {
                await Task.WhenAll(taskList.ToArray());
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

        }

        /// <summary>
        /// Kicks off a bunch of non-blocking tasks, half of which will throw exceptions
        /// </summary>
        /// <remarks>All tasks run; all exceptions thrown are returned in an AggregateException.  All other
        /// tasks run until completion in splendid isolation</remarks>
        private static Task TaskWaitAllExceptionHandling()
        {
            BeginExample("TaskWaitAllExceptionHandling starting...");

            var taskList = new List<Task>();
            var sut = new NonBlockingButThrowsOccasionalException();
            for (int i = 0; i < IterationCount; i++)
            {
                taskList.Add(sut.StartIteration(i, 1, $"TaskWaitAllExceptionHandling iteration {i}"));
            }

            try
            {
                Task.WaitAll(taskList.ToArray());
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

            return Task.CompletedTask;
        }

        /// <summary>
        /// Make sure to put a breakpoint in the NonBlockingMethod.StartIteration method somewhere
        /// within the extraIterativeDelay logic
        /// </summary>
        /// <remarks>Can use this to demo the threads, tasks and Parallel Watch windows, freezing threads/tasks,
        /// switching between threads/tasks etc.</remarks>
        private static async Task DemoThreadDebugging()
        {
            BeginExample("DemoThreadDebugging starting...");

            var count = 5;
            var sut = new NonBlockingMethod();

            var taskList = new List<Task>();
            for (int i = 1; i <= count; i++)
            {
                taskList.Add(sut.StartIteration(i, DelayDurationOne, "DemoThreadDebugging", true));
            }

            await Task.WhenAll(taskList.ToArray());

            Console.WriteLine($"DemoThreadDebugging complete");
        }
    }
}
