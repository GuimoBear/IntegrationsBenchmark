using BenchmarkDotNet.Attributes;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace IntegrationsBenchmark.Benchmarks
{
    [Description("Rest with HTTP 2 and without SSL")]
    public class RestHttp2WithoutSslBenchmark : BenchmarkBase
    {
        protected override string Url => "http://localhost:5000";

        [GlobalSetup]
        public override void GlobalSetup()
        { 
            Initialize();
            Client.DefaultRequestVersion = HttpVersion.Version20;
            Client.DefaultVersionPolicy = HttpVersionPolicy.RequestVersionExact;
        }

        [Benchmark(Description = "Send request")]
        public async Task<IEnumerable<WeatherData>> SendAsync()
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
