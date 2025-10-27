using BenchmarkDotNet.Attributes;
using System.Threading;

namespace Ryujinx.Benchmarks
{
    [MemoryDiagnoser]
    public class SchedulerWaitBenchmark
    {
        private ManualResetEventSlim _evt;
        private const int Iterations = 10000;

        [Params(0, 1, 2)]
        public int WaitTimeoutMs { get; set; }

        [GlobalSetup]
        public void Setup()
        {
            _evt = new ManualResetEventSlim(false);
        }

        [GlobalCleanup]
        public void Cleanup()
        {
            _evt?.Dispose();
        }

        [Benchmark(Baseline = true)]
        public void BlockingWaitLoop()
        {
            // Simulate calling Wait() but using a short signal to avoid deadlock.
            for (int i = 0; i < Iterations; i++)
            {
                // Signal briefly to avoid blocking forever.
                _evt.Set();
                _evt.Wait();
                _evt.Reset();
            }
        }

        [Benchmark]
        public void TimedWaitLoop()
        {
            for (int i = 0; i < Iterations; i++)
            {
                // Use a timed wait to simulate a scheduler using Wait(timeout)
                _evt.Set();
                bool signaled = _evt.Wait(WaitTimeoutMs); // use parameterized timeout
                if (!signaled)
                {
                    // fallback behavior
                    Thread.Yield();
                }

                _evt.Reset();
            }
        }
    }
}
