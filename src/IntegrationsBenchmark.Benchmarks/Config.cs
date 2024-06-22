using BenchmarkDotNet.Columns;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Diagnosers;
using BenchmarkDotNet.Environments;
using BenchmarkDotNet.Exporters;
using BenchmarkDotNet.Exporters.Csv;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Loggers;
using BenchmarkDotNet.Order;
using IntegrationsBenchmark.Benchmarks.Helpers;

namespace IntegrationsBenchmark.Benchmarks
{
    public class Config : ManualConfig
    {
        public const int Iterations = 2000;

        public Config()
        {
            AddLogger(ConsoleLogger.Default);

            AddExporter(CsvExporter.Default);
            AddExporter(MarkdownExporter.GitHub);
            AddExporter(HtmlExporter.Default);

            var md = MemoryDiagnoser.Default;
            AddDiagnoser(md);
            AddColumn(new RuntimeColumn());
            AddColumn(new ChannelNameColumn());
            AddColumn(TargetMethodColumn.Method);
            AddColumn(StatisticColumn.Mean);
            AddColumn(StatisticColumn.StdDev);
            AddColumn(StatisticColumn.Error);
            AddColumn(BaselineRatioColumn.RatioMean);
            AddColumnProvider(DefaultColumnProviders.Metrics);

            AddJob(Job.Default
                   .WithRuntime(CoreRuntime.Core80)
                   .WithLaunchCount(1)
                   .WithWarmupCount(2)
                   .WithUnrollFactor(Iterations)
                   .WithIterationCount(10)
            );
            Orderer = new DefaultOrderer(SummaryOrderPolicy.FastestToSlowest);
            Options |= ConfigOptions.JoinSummary;
        }
    }
}
