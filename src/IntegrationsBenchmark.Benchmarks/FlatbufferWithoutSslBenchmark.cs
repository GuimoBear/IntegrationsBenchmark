using BenchmarkDotNet.Attributes;
using Grpc.Core;
using IntegrationsBenchmark.Benchmarks.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;

namespace IntegrationsBenchmark.Benchmarks
{
    [Description("Grpc with Flatbuffer")]
    public class FlatbufferWithoutSslBenchmark : BenchmarkBase
    {
        private static readonly Flats.Empty Empty = new Flats.Empty();

        protected override string Url => "localhost";

        private Channel GrpcChannel;
        private Flats.FlatWeatherForecaster.FlatWeatherForecasterClient GrpcClient;
        private DuplexStreamPool<Flats.Empty, Flats.ForecastFullDuplexResponse> StreamPool;

        private AsyncDuplexStreamingCall<Flats.Empty, Flats.ForecastFullDuplexResponse> StreamingCall;

        [GlobalSetup]
        public override void GlobalSetup()
        {
            GrpcChannel = new Channel(Url, 6000, ChannelCredentials.Insecure);
            GrpcChannel.ConnectAsync().Wait();
            GrpcClient = new Flats.FlatWeatherForecaster.FlatWeatherForecasterClient(GrpcChannel);
            StreamPool = new DuplexStreamPool<Flats.Empty, Flats.ForecastFullDuplexResponse>(ct => GrpcClient.ForecastFullDuplexStream(cancellationToken: ct), Environment.ProcessorCount, true);
        }

        [IterationSetup]
        public void IterationSetup()
            => StreamingCall = StreamPool.Allocate();

        [Benchmark(Description = "Send request to duplex streaming channel")]
        public async Task<IEnumerable<Flats.WeatherData>> SendDuplexAsync()
        {
            await StreamingCall.RequestStream.WriteAsync(Empty);
            var res = new List<Flats.WeatherData>();
            while (await StreamingCall.ResponseStream.MoveNext() && StreamingCall.ResponseStream.Current.Type == Flats.ForecastFullDuplexResponseType.Item)
                res.Add(StreamingCall.ResponseStream.Current.Item);
            return res;
        }

        [Benchmark(Description = "Send request and get data stream")]
        public async Task<IEnumerable<Flats.WeatherData>> SendStreamAsync()
        {
            using var stream = GrpcClient.ForecastHalfDuplexStream(Empty);
            var res = new List<Flats.WeatherData>();
            await foreach (var weatherData in stream.ResponseStream.ReadAllAsync())
                res.Add(weatherData);
            return res;
        }

        [Benchmark(Description = "Send request")]
        public async Task<IEnumerable<Flats.WeatherData>> SendAsync()
        {
            var response = await GrpcClient.Forecast(Empty);
            var res = new List<Flats.WeatherData>();
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
