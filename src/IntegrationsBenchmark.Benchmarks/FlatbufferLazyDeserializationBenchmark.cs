using BenchmarkDotNet.Attributes;
using Grpc.Core;
using IntegrationsBenchmark.Benchmarks.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;

namespace IntegrationsBenchmark.Benchmarks
{
    [Description("Grpc with Flatbuffer lazy deserialization")]
    [MemoryDiagnoser(true)]
    public class FlatbufferLazyDeserializationBenchmark : BenchmarkBase
    {
        private static readonly Flats.Empty Empty = new Flats.Empty();

        protected override string Url => "localhost";

        private Channel GrpcChannel;
        private Flats.FlatWeatherForecasterLazy.FlatWeatherForecasterLazyClient GrpcClient;
        private DuplexStreamPool<Flats.Empty, Flats.ForecastLazyFullDuplexResponse> StreamPool;

        private AsyncDuplexStreamingCall<Flats.Empty, Flats.ForecastLazyFullDuplexResponse> StreamingCall;

        [GlobalSetup]
        public override void GlobalSetup()
        {
            GrpcChannel = new Channel(Url, 6000, ChannelCredentials.Insecure);
            GrpcChannel.ConnectAsync().Wait();
            GrpcClient = new Flats.FlatWeatherForecasterLazy.FlatWeatherForecasterLazyClient(GrpcChannel);
            StreamPool = new DuplexStreamPool<Flats.Empty, Flats.ForecastLazyFullDuplexResponse>(ct => GrpcClient.ForecastFullDuplexStream(cancellationToken: ct), Environment.ProcessorCount, true);
        }

        [IterationSetup]
        public void IterationSetup()
            => StreamingCall = StreamPool.Allocate();

        [Benchmark(Description = "Send request to duplex streaming channel")]
        public async Task<List<Flats.WeatherDataLazy>> SendDuplexAsync()
        {
            await StreamingCall.RequestStream.WriteAsync(Empty);
            var res = new List<Flats.WeatherDataLazy>();
            while (await StreamingCall.ResponseStream.MoveNext() && StreamingCall.ResponseStream.Current.Type == Flats.ForecastLazyFullDuplexResponseType.Item)
                res.Add(StreamingCall.ResponseStream.Current.Item);
            return res;
        }

        [Benchmark(Description = "Send request and get data stream")]
        public async Task<List<Flats.WeatherDataLazy>> SendStreamAsync()
        {
            using var stream = GrpcClient.ForecastHalfDuplexStream(Empty);
            var res = new List<Flats.WeatherDataLazy>();
            await foreach (var weatherData in stream.ResponseStream.ReadAllAsync())
                res.Add(weatherData);
            return res;
        }

        [Benchmark(Description = "Send request")]
        public async Task<List<Flats.WeatherDataLazy>> SendAsync()
        {
            var response = await GrpcClient.Forecast(Empty);
            var res = new List<Flats.WeatherDataLazy>();
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
