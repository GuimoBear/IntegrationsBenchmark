using BenchmarkDotNet.Attributes;
using Grpc.Core;
using IntegrationsBenchmark.Benchmarks.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;

namespace IntegrationsBenchmark.Benchmarks
{
    [Description("Grpc with Flatbuffer progressive deserialization")]
    public class FlatbufferProgressiveDeserializationBenchmark : BenchmarkBase
    {
        private static readonly Flats.Empty Empty = new Flats.Empty();

        protected override string Url => "localhost";

        private Channel GrpcChannel;
        private Flats.FlatWeatherForecasterProgressive.FlatWeatherForecasterProgressiveClient GrpcClient;
        private DuplexStreamPool<Flats.Empty, Flats.ForecastProgressiveFullDuplexResponse> StreamPool;

        private AsyncDuplexStreamingCall<Flats.Empty, Flats.ForecastProgressiveFullDuplexResponse> StreamingCall;

        [GlobalSetup]
        public override void GlobalSetup()
        {
            GrpcChannel = new Channel(Url, 6000, ChannelCredentials.Insecure);
            GrpcChannel.ConnectAsync().Wait();
            GrpcClient = new Flats.FlatWeatherForecasterProgressive.FlatWeatherForecasterProgressiveClient(GrpcChannel);
            StreamPool = new DuplexStreamPool<Flats.Empty, Flats.ForecastProgressiveFullDuplexResponse>(ct => GrpcClient.ForecastFullDuplexStream(cancellationToken: ct), Environment.ProcessorCount, true);
        }

        [IterationSetup]
        public void IterationSetup()
            => StreamingCall = StreamPool.Allocate();

        [Benchmark(Description = "Send request to duplex streaming channel")]
        public async Task<IEnumerable<Flats.WeatherDataProgressive>> SendDuplexAsync()
        {
            await StreamingCall.RequestStream.WriteAsync(Empty);
            var res = new List<Flats.WeatherDataProgressive>();
            while (await StreamingCall.ResponseStream.MoveNext() && StreamingCall.ResponseStream.Current.Type == Flats.ForecastProgressiveFullDuplexResponseType.Item)
                res.Add(StreamingCall.ResponseStream.Current.Item);
            return res;
        }

        [Benchmark(Description = "Send request and get data stream")]
        public async Task<IEnumerable<Flats.WeatherDataProgressive>> SendStreamAsync()
        {
            using var stream = GrpcClient.ForecastHalfDuplexStream(Empty);
            var res = new List<Flats.WeatherDataProgressive>();
            await foreach (var weatherData in stream.ResponseStream.ReadAllAsync())
                res.Add(weatherData);
            return res;
        }

        [Benchmark(Description = "Send request")]
        public async Task<IEnumerable<Flats.WeatherDataProgressive>> SendAsync()
        {
            var response = await GrpcClient.Forecast(Empty);
            var res = new List<Flats.WeatherDataProgressive>();
            foreach (var weatherData in response.Forecasts)
                res.Add(weatherData);
            return res;
        }

        [IterationCleanup]
        public void IterationCleanup()
            => StreamPool.Free(StreamingCall);

        [GlobalCleanup]
        public override async Task GlobalCleanupAsync()
        {
            try { await StreamPool.DisposeAsync(); } catch { }
            try { await GrpcChannel.ShutdownAsync(); } catch { }
        }
    }
}
