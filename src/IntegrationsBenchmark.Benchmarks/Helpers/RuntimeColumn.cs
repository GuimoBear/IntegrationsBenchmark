using BenchmarkDotNet.Columns;
using BenchmarkDotNet.Reports;
using BenchmarkDotNet.Running;

namespace IntegrationsBenchmark.Benchmarks.Helpers
{
    public class RuntimeColumn : IColumn
    {
        public string Id => nameof(RuntimeColumn);

        public string ColumnName => "Runtime";

        public string Legend => "The job runtime";

        public string GetValue(Summary summary, BenchmarkCase benchmarkCase)
            => benchmarkCase.GetRuntime().Name;

        public string GetValue(Summary summary, BenchmarkCase benchmarkCase, SummaryStyle style)
            => benchmarkCase.GetRuntime().Name;

        public bool IsDefault(Summary summary, BenchmarkCase benchmarkCase)
            => false;

        public bool IsAvailable(Summary summary) => true;
        public bool AlwaysShow => true;
        public ColumnCategory Category => ColumnCategory.Job;
        public int PriorityInCategory => 0;
        public bool IsNumeric => false;
        public UnitType UnitType => UnitType.Dimensionless;
        public override string ToString() => ColumnName;
    }
}
