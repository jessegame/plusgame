using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;

namespace PlusCommon.Timing
{
    /// <summary>
    /// Code timer
    /// </summary>
    public static class CodeTimer
    {
        /// <summary>
        /// init
        /// </summary>
        public static void Initialize()
        {
            Process.GetCurrentProcess().PriorityClass = ProcessPriorityClass.High;
            Thread.CurrentThread.Priority = ThreadPriority.Highest;
            Time("", 1, () => { });
        }

        /// <summary>
        /// time action
        /// </summary>
        /// <param name="name"></param>
        /// <param name="action"></param>
        public static void Time(string name, Action action)
        {
            Time(name, 1, action);
        }

        /// <summary>
        /// time action
        /// </summary>
        /// <param name="name"></param>
        /// <param name="iteration">cycle Count</param>
        /// <param name="action"></param>
        /// <code>
        /// Time("test1", 1, () => { });
        /// </code>
        public static void Time(string name, int iteration, Action action)
        {
            if (String.IsNullOrEmpty(name)) return;

            StringBuilder log = new StringBuilder();
            // 1.
            GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced);
            int[] gcCounts = new int[GC.MaxGeneration + 1];
            for (int i = 0; i <= GC.MaxGeneration; i++)
            {
                gcCounts[i] = GC.CollectionCount(i);
            }

            // 2.
            Stopwatch watch = new Stopwatch();
            watch.Start();
            ulong cycleCount = GetCurrentThreadTimes();
            for (int i = 0; i < iteration; i++) action();
            ulong cpuCycles = GetCurrentThreadTimes() - cycleCount;
            watch.Stop();

            // 3.
            log.AppendLine(name);
            log.AppendLine("\tTime Elapsed:\t" + watch.ElapsedMilliseconds.ToString("N0") + "ms");
            log.AppendLine("\tCPU Cycles:\t" + cpuCycles.ToString("N0"));

            // 4.
            for (int i = 0; i <= GC.MaxGeneration; i++)
            {
                int count = GC.CollectionCount(i) - gcCounts[i];
                log.AppendLine("\tGen " + i + ": \t\t" + count);
            }
            log.AppendLine();
            Console.WriteLine(log.ToString());
        }

        private static ulong GetCycleCount()
        {
            ulong cycleCount = 0;
            QueryThreadCycleTime(GetCurrentThread(), ref cycleCount);
            return cycleCount;
        }

        private static ulong GetCurrentThreadTimes()
        {
            ulong l;
            ulong kernelTime, userTimer;
            GetThreadTimes(GetCurrentThread(), out l, out l, out kernelTime,
               out userTimer);
            return kernelTime + userTimer;
        }

        //Vista和Server 2008中新的函数
        [DllImport("kernel32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool QueryThreadCycleTime(IntPtr threadHandle, ref ulong cycleTime);

        [DllImport("kernel32.dll")]
        static extern IntPtr GetCurrentThread();

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool GetThreadTimes(IntPtr hThread, out ulong lpCreationTime, out ulong lpExitTime, out ulong lpKernelTime, out ulong lpUserTime);
    }
}
