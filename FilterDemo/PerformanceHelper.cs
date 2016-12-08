using System;
using System.Diagnostics;
using System.Threading;

namespace FilterTest
{
    public static class PerformanceHelper
    {
        public static void Bench(Action func, double benchtime = 1000, bool silentmode = false, string description = "")
        {
            int iterations = 1;
            double elapsed;

            while ((elapsed = Profile(iterations, func).TotalMilliseconds) < benchtime * 0.1)
            {
                iterations *= 10;
            }

            iterations = Convert.ToInt32(iterations * benchtime / elapsed);

            double timeperaction = Profile(iterations, func).TotalMilliseconds / iterations;

            if (!silentmode)
            {
                Console.Write(description);
                Console.WriteLine(" Time per action {0} ns", timeperaction * 1000);
            }
        }

        public static TimeSpan Profile(int iterations, Action func, bool silentmode = true, string description = "")
        {
            //Run at highest priority to minimize fluctuations caused by other processes/threads
            Process.GetCurrentProcess().PriorityClass = ProcessPriorityClass.High;
            Thread.CurrentThread.Priority = ThreadPriority.Highest;

            // warm up 
            func();

            var watch = new Stopwatch();

            // clean up
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();

            watch.Start();
            for (int i = 0; i < iterations; i++)
            {
                func();
            }
            watch.Stop();

            if (!silentmode)
            {
                Console.Write(description);
                Console.WriteLine(" Time Elapsed {0} ns", watch.Elapsed.TotalMilliseconds * 1000);
            }
            return watch.Elapsed;
        }
    }
}