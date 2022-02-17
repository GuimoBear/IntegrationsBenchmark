using BenchmarkDotNet.Running;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IntegrationsBenchmark.Benchmarks
{
    class Program
    {
        static async Task Main(string[] args)
        {
            /*
            var benchmark = new GrpcBenchmark();
            try
            {
                benchmark.GlobalSetup();
                for (int i = 1; i <= 20; i++)
                {
                    Console.WriteLine(i);
                    try
                    {
                        benchmark.IterationSetup();
                        var res = await benchmark.SendDuplexAsync();
                        foreach (var (data, idx) in res.Zip(Enumerable.Range(1, res.Count())))
                            Console.WriteLine($"{idx}: {data}");
                    }
                    finally
                    {
                        benchmark.IterationCleanup();
                    }
                }
            }
            finally
            {
                await benchmark.GlobalCleanupAsync();
            }
            */
            new BenchmarkSwitcher(typeof(BenchmarkBase).Assembly).Run(args, new Config());
        }
    }
}
