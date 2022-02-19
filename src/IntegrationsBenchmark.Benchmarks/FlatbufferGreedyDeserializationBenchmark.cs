using BenchmarkDotNet.Attributes;
using Grpc.Core;
using IntegrationsBenchmark.Benchmarks.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;

namespace IntegrationsBenchmark.Benchmarks
{
    [Description("Grpc with Flatbuffer greedy deserialization")]
    public class FlatbufferGreedyDeserializationBenchmark : BenchmarkBase
    {
        private static readonly Flats.Empty Empty = new Flats.Empty();

        protected override string Url => "localhost";

        private Channel GrpcChannel;
        private Flats.FlatWeatherForecasterGreedy.FlatWeatherForecasterGreedyClient GrpcClient;
        private DuplexStreamPool<Flats.Empty, Flats.ForecastGreedyFullDuplexResponse> StreamPool;

        private AsyncDuplexStreamingCall<Flats.Empty, Flats.ForecastGreedyFullDuplexResponse> StreamingCall;

        [GlobalSetup]
        public override void GlobalSetup()
        {
            GrpcChannel = new Channel(Url, 6000, ChannelCredentials.Insecure);
            GrpcChannel.ConnectAsync().Wait();
            GrpcClient = new Flats.FlatWeatherForecasterGreedy.FlatWeatherForecasterGreedyClient(GrpcChannel);
            StreamPool = new DuplexStreamPool<Flats.Empty, Flats.ForecastGreedyFullDuplexResponse>(ct => GrpcClient.ForecastFullDuplexStream(cancellationToken: ct), Environment.ProcessorCount, true);
        }

        [IterationSetup]
        public void IterationSetup()
            => StreamingCall = StreamPool.Allocate();

        [Benchmark(Description = "Send request to duplex streaming channel")]
        public async Task<IEnumerable<Flats.WeatherDataGreedy>> SendDuplexAsync()
        {
            await StreamingCall.RequestStream.WriteAsync(Empty);
            var res = new List<Flats.WeatherDataGreedy>();
            while (await StreamingCall.ResponseStream.MoveNext() && StreamingCall.ResponseStream.Current.Type == Flats.ForecastGreedyFullDuplexResponseType.Item)
                res.Add(StreamingCall.ResponseStream.Current.Item);
            return res;
        }

        [Benchmark(Description = "Send request and get data stream")]
        public async Task<IEnumerable<Flats.WeatherDataGreedy>> SendStreamAsync()
        {
            using var stream = GrpcClient.ForecastHalfDuplexStream(Empty);
            var res = new List<Flats.WeatherDataGreedy>();
            await foreach (var weatherData in stream.ResponseStream.ReadAllAsync())
                res.Add(weatherData);
            return res;
        }

        [Benchmark(Description = "Send request")]
        public async Task<IEnumerable<Flats.WeatherDataGreedy>> SendAsync()
        {
            var response = await GrpcClient.Forecast(Empty);
            var res = new List<Flats.WeatherDataGreedy>();
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
