using BenchmarkDotNet.Attributes;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Grpc.Net.Client;
using IntegrationsBenchmark.Benchmarks.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace IntegrationsBenchmark.Benchmarks
{
    [Description("Grpc with SSL and HTTP 3")]
    [MemoryDiagnoser(true)]
    public class GrpcWithSslHttp3Benchmark : BenchmarkBase
    {
        private static readonly Empty Empty = new Empty();

        protected override string Url => "https://localhost:6001";

        private GrpcChannel GrpcChannel;
        private Protos.WeatherForecaster.WeatherForecasterClient GrpcClient;
        private DuplexStreamPool<Empty, Protos.ForecastFullDuplexResponse> StreamPool;

        private AsyncDuplexStreamingCall<Empty, Protos.ForecastFullDuplexResponse> StreamingCall;

        [GlobalSetup]
        public override void GlobalSetup()
        {
            Initialize();
            Client.BaseAddress = null;
            Client.DefaultRequestVersion = HttpVersion.Version30;
            Client.DefaultVersionPolicy = HttpVersionPolicy.RequestVersionExact;
            GrpcChannel = GrpcChannel.ForAddress(Url, new GrpcChannelOptions { HttpClient = Client, DisposeHttpClient = false });
            GrpcClient = new Protos.WeatherForecaster.WeatherForecasterClient(GrpcChannel);
            StreamPool = new DuplexStreamPool<Empty, Protos.ForecastFullDuplexResponse>(ct => GrpcClient.ForecastFullDuplexStream(cancellationToken: ct), Environment.ProcessorCount, true);
        }

        [IterationSetup]
        public void IterationSetup()
            => StreamingCall = StreamPool.Allocate();

        [Benchmark(Description = "Send request to duplex streaming channel")]
        public async Task<List<Protos.WeatherData>> SendDuplexAsync()
        {
            await StreamingCall.RequestStream.WriteAsync(Empty);
            var res = new List<Protos.WeatherData>();
            while (await StreamingCall.ResponseStream.MoveNext() && StreamingCall.ResponseStream.Current.BodyCase == Protos.ForecastFullDuplexResponse.BodyOneofCase.Data)
                res.Add(StreamingCall.ResponseStream.Current.Data);
            return res;
        }

        [Benchmark(Description = "Send request and get data stream")]
        public async Task<List<Protos.WeatherData>> SendStreamAsync()
        {
            using var stream = GrpcClient.ForecastHalfDuplexStream(Empty);
            var res = new List<Protos.WeatherData>();
            await foreach (var weatherData in stream.ResponseStream.ReadAllAsync())
                res.Add(weatherData);
            return res;
        }

        [Benchmark(Description = "Send request")]
        public async Task<IList<Protos.WeatherData>> SendAsync()
        {
            var response = await GrpcClient.ForecastAsync(Empty);
            return response.Forecasts;
        }

        [IterationCleanup]
        public void IterationCleanup()
            => StreamPool.Free(StreamingCall);

        [GlobalCleanup]
        public override async Task GlobalCleanupAsync()
        {
            try { await StreamPool.DisposeAsync(); } catch { }
            try { GrpcChannel.Dispose(); } catch { }
            Finalize();
        }
    }
}
