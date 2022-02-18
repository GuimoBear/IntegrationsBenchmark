using BenchmarkDotNet.Running;
using System.Threading.Tasks;

namespace IntegrationsBenchmark.Benchmarks
{
    class Program
    {
        static async Task Main(string[] args)
        {
            new BenchmarkSwitcher(typeof(BenchmarkBase).Assembly).Run(args, new Config());
        }
    }
}
