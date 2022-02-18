using System.Net.Http;
using System.Threading.Tasks;

namespace IntegrationsBenchmark.Benchmarks
{
    public abstract class BenchmarkBase
    {
        protected HttpClient Client { get; private set; }

        protected virtual string Url => "https://localhost:5001";

        public abstract void GlobalSetup();

        public abstract Task GlobalCleanupAsync();

        protected void Initialize()
        {
            Client = new HttpClient() { BaseAddress = new System.Uri(Url) };
        }

        protected void Finalize()
        {
            try { Client.Dispose(); } catch { }
        }
    }
}
