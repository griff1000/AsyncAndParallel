namespace AsyncAndParallel
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;

    internal class Program
    {
        private static bool _delayAfterOperation = false;
        private static ConsoleColor _normalColour;
        private const int DelayDurationOne = 500;
        private const int DelayDurationTwo = 300;
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
        private static void WriteMethodStartMessage(string message)
        {
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine(message);
            Console.ForegroundColor = _normalColour;
        }

        private static async Task DelayIfRequired()
        {
            if (_delayAfterOperation) await Task.Delay(DelayDurationOne + 100);
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

        private static async Task SleepLoopTests()
        {
            WriteMethodStartMessage("SleepLoopTests starting...");
            var count = 10;

            var sw = Stopwatch.StartNew();
            var asl1 = new AsyncSleepLoop();
            await asl1.StartSleepLoop(count, DelayDurationOne, "One");
            sw.Stop();
            await DelayIfRequired();

            Console.WriteLine($"Duration of async sleep iteration loop for {count} iterations sleeping for {DelayDurationOne} ms was {sw.ElapsedMilliseconds} ms");

            sw = Stopwatch.StartNew();
            var asl2 = new AsyncSleepLoop();
            for (var i = 0; i < count; i++)
            {
                await asl2.StartIteration(i, DelayDurationTwo, "Two");
            }
            sw.Stop();
            await DelayIfRequired();

            Console.WriteLine($"Duration of async sleep iteration loop for {count} iterations sleeping for {DelayDurationTwo} ms was {sw.ElapsedMilliseconds} ms");
        }

        private static async Task DelayLoopTests()
        {
            WriteMethodStartMessage("DelayLoopTests starting...");
            var sw = Stopwatch.StartNew();
            var count = 10;
            var adl1 = new AsyncDelayLoop();
            await adl1.StartDelayLoop(count, DelayDurationOne, "DelayLoopTests One");
            sw.Stop();
            await DelayIfRequired();
            Console.WriteLine($"Duration of async delay iteration loop for {count} iterations delaying for {DelayDurationOne} ms was {sw.ElapsedMilliseconds} ms");

            sw = Stopwatch.StartNew();
            var adl2 = new AsyncDelayLoop();
            for (var i = 0; i < 10; i++)
            {
                await adl2.StartIteration(i, DelayDurationTwo, "DelayLoopTests Two");
            }
            sw.Stop();
            await DelayIfRequired();
            Console.WriteLine($"Duration of async delay iteration loop for {count} iterations delay for {DelayDurationTwo} ms was {sw.ElapsedMilliseconds} ms");
        }

        private static async Task ParallelForEachSleepLoopTests()
        {
            WriteMethodStartMessage("ParallelForEachSleepLoopTests starting...");

            var sw = Stopwatch.StartNew();
            var array = new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
            var asl = new AsyncSleepLoop();
            Parallel.ForEach(array, (i, state) => asl.StartIteration(i, DelayDurationOne, "ParallelForEachSleepLoopTests"));
            await DelayIfRequired();
            Console.WriteLine($"Duration of Parallel.ForEach sleep iteration loop for {array.Length} iterations sleep for {DelayDurationOne} ms was {sw.ElapsedMilliseconds} ms");
        }

        private static async Task ParallelForEachDelayLoopTests()
        {
            WriteMethodStartMessage("ParallelForEachDelayLoopTests starting...");

            var sw = Stopwatch.StartNew();
            var array = new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
            var adl = new AsyncDelayLoop();
            Parallel.ForEach(array, (i, state) => adl.StartIteration(i, DelayDurationOne, "ParallelForEachDelayLoopTests"));
            await DelayIfRequired();
            Console.WriteLine($"Duration of Parallel.ForEach delay iteration loop for {array.Length} iterations delay for {DelayDurationOne} ms was {sw.ElapsedMilliseconds} ms");
        }

        private static async Task ParallelForEachDelayLoopWithAsyncTests()
        {
            WriteMethodStartMessage("ParallelForEachDelayLoopWithAsyncTests starting...");

            var sw = Stopwatch.StartNew();
            var array = new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
            var adl = new AsyncDelayLoop();
            Parallel.ForEach(array, async (i, state) => await adl.StartIteration(i, DelayDurationOne, "ParallelForEachDelayLoopWithAsyncTests"));
            sw.Stop();
            await DelayIfRequired();
            Console.WriteLine($"Duration of Parallel.ForEach delay iteration loop for {array.Length} iterations delay for {DelayDurationOne} ms was {sw.ElapsedMilliseconds} ms");
        }

        private static async Task ParallelForEachDelayLoopWithAsyncAttachedTests()
        {
            WriteMethodStartMessage("ParallelForEachDelayLoopWithAsyncAttachedTests starting...");

            var sw = Stopwatch.StartNew();
            var array = new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
            var adl = new AsyncDelayLoop();

            await Task.Factory.StartNew(() =>
            {
                Parallel.ForEach(array,
                    async (i, state) => await adl.StartIteration(i, DelayDurationOne, "ParallelForEachDelayLoopWithAsyncAttachedTests"));
            }, TaskCreationOptions.AttachedToParent);
            sw.Stop();
            await DelayIfRequired();
            Console.WriteLine($"Duration of attached Parallel.ForEach delay iteration loop for {array.Length} iterations delay for {DelayDurationOne} ms was {sw.ElapsedMilliseconds} ms");
        }

        private static async Task TaskWhenAllDelayLoopTests()
        {
            WriteMethodStartMessage("TaskWhenAllDelayLoopTests starting...");

            var sw = Stopwatch.StartNew();
            var adl = new AsyncDelayLoop();
            var count = 10;

            var taskList = new List<Task>();
            for (int i = 1; i <= count; i++)
            {
                taskList.Add(adl.StartIteration(i, DelayDurationOne, "TaskWhenAllDelayLoopTests"));
            }

            await Task.WhenAll(taskList.ToArray());

            Console.WriteLine($"Duration of Task.WhenAll delay iteration loop for {count} iterations delay for {DelayDurationOne} ms was {sw.ElapsedMilliseconds} ms");
        }

        private static Task TaskWaitAllDelayLoopTests()
        {
            WriteMethodStartMessage("TaskWaitAllDelayLoopTests starting...");

            var sw = Stopwatch.StartNew();
            var adl = new AsyncDelayLoop();
            var count = 10;

            var taskList = new List<Task>();
            for (int i=1; i <= count; i++)
            {
                taskList.Add(adl.StartIteration(i, DelayDurationOne, "TaskWaitAllDelayLoopTests"));
            }

            Task.WaitAll(taskList.ToArray());

            Console.WriteLine($"Duration of Task.WhenAll delay iteration loop for {count} iterations delay for {DelayDurationOne} ms was {sw.ElapsedMilliseconds} ms");

            return Task.CompletedTask;
        }

        private static async Task TaskWhenAllSleepAndDelayLoopTests()
        {
            WriteMethodStartMessage("TaskWhenAllSleepAndDelayLoopTests starting...");

            var sw = Stopwatch.StartNew();
            var count = 100;
            var adsl = new AsyncDelayAndSleepLoop();

            var taskList = new List<Task>();
            for (int i = 1; i <= count; i++)
            {
                taskList.Add(adsl.StartIteration(i, DelayDurationOne, "TaskWhenAllSleepAndDelayLoopTests"));
            }

            
            await Task.WhenAll(taskList.ToArray());

            Console.WriteLine($"Duration of Task.WhenAll delay iteration loop for {count} iterations delay for {DelayDurationOne * 10} ms was {sw.ElapsedMilliseconds} ms");
        }

        private static async Task TaskWhenAllHugeNumberOfTasksDelayLoopTests()
        {
            WriteMethodStartMessage("TaskWhenAllHugeNumberOfTasksDelayLoopTests starting...");

            var sw = Stopwatch.StartNew();
            var count = 10000;
            var adl = new AsyncDelayLoop();

            var taskList = new List<Task>();
            for (int i =1; i<= count; i++)
            {
                taskList.Add(adl.StartIteration(i, DelayDurationOne, "TaskWhenAllHugeNumberOfTasksDelayLoopTests", true));
            }

            await Task.WhenAll(taskList.ToArray());

            Console.WriteLine($"Duration of Task.WhenAll delay iteration loop for {count} iterations delay for {DelayDurationOne} ms was {sw.ElapsedMilliseconds} ms");
        }

        private static async Task TaskWhenAllExceptionHandling()
        {
            WriteMethodStartMessage("TaskWhenAllExceptionHandling starting...");

            var count = 10;
            var taskList = new List<Task>();
            var taskClass = new AsyncDoStuffThrowOccasionalException();
            for (int i = 0; i< count; i++)
            {
                taskList.Add(taskClass.StartIteration(i, 1, $"TaskWhenAllExceptionHandling iteration {i}"));
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

        private static Task TaskWaitAllExceptionHandling()
        {
            WriteMethodStartMessage("TaskWaitAllExceptionHandling starting...");

            var count = 10;
            var taskList = new List<Task>();
            var taskClass = new AsyncDoStuffThrowOccasionalException();
            for (int i = 0; i < count; i++)
            {
                taskList.Add(taskClass.StartIteration(i, 1, $"TaskWaitAllExceptionHandling iteration {i}"));
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

        private static async Task DemoThreadDebugging()
        {
            WriteMethodStartMessage("DemoThreadDebugging starting...");

            var sw = Stopwatch.StartNew();
            var count = 5;
            var adl = new AsyncDelayLoop();

            var taskList = new List<Task>();
            for (int i = 1; i <= count; i++)
            {
                taskList.Add(adl.StartIteration(i, DelayDurationOne, "DemoThreadDebugging", true));
            }

            await Task.WhenAll(taskList.ToArray());

            Console.WriteLine($"Duration of Task.WhenAll delay iteration loop for {count} iterations delay for {DelayDurationOne} ms was {sw.ElapsedMilliseconds} ms");
        }
    }
}
