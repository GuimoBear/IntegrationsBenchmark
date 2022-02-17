using BenchmarkDotNet.Columns;
using BenchmarkDotNet.Reports;
using BenchmarkDotNet.Running;
using System.ComponentModel;
using System.Reflection;

namespace IntegrationsBenchmark.Benchmarks.Helpers
{
    public class ChannelNameColumn : IColumn
    {
        public string Id => nameof(ChannelNameColumn);
        public string ColumnName { get; } = "Channel";
        public string Legend => "The channel name";

        public bool IsDefault(Summary summary, BenchmarkCase benchmarkCase) => false;
        public string GetValue(Summary summary, BenchmarkCase benchmarkCase)
        {
            var type = benchmarkCase.Descriptor.WorkloadMethod.DeclaringType;
            return type.GetCustomAttribute<DescriptionAttribute>()?.Description ?? type.Name.Replace("Benchmarks", string.Empty);
        }

        public string GetValue(Summary summary, BenchmarkCase benchmarkCase, SummaryStyle style) => GetValue(summary, benchmarkCase);

        public bool IsAvailable(Summary summary) => true;
        public bool AlwaysShow => true;
        public ColumnCategory Category => ColumnCategory.Job;
        public int PriorityInCategory => -10;
        public bool IsNumeric => false;
        public UnitType UnitType => UnitType.Dimensionless;
        public override string ToString() => ColumnName;
    }
}
