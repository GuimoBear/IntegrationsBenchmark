using BenchmarkDotNet.Attributes;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace IntegrationsBenchmark.Benchmarks
{
    [Description("Rest with HTTP 3")]
    [MemoryDiagnoser(true)]
    public class RestHttp3Benchmark : BenchmarkBase
    {
        [GlobalSetup]
        public override void GlobalSetup()
        {
            Initialize();
            Client.DefaultRequestVersion = HttpVersion.Version30;
            Client.DefaultVersionPolicy = HttpVersionPolicy.RequestVersionExact;
        }

        [Benchmark(Description = "Send request")]
        public async Task<List<WeatherData>> SendAsync()
        {
            var httpResponse = await Client.SendAsync(new HttpRequestMessage(HttpMethod.Get, "weatherforecast"));
            var str = await httpResponse.Content.ReadAsStringAsync();
            var forecasts = JsonSerializer.Deserialize<List<WeatherData>>(str);
            return forecasts;
        }

        [GlobalCleanup]
        public override Task GlobalCleanupAsync()
        {
            Finalize();
            return Task.CompletedTask;
        }
    }
}
