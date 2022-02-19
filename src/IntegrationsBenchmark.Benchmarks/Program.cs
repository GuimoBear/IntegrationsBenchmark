using BenchmarkDotNet.Running;

namespace IntegrationsBenchmark.Benchmarks
{
    class Program
    {
        static void Main(string[] args)
        {
            new BenchmarkSwitcher(typeof(BenchmarkBase).Assembly).Run(args, new Config());
        }
    }
}
