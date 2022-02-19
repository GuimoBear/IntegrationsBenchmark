using Grpc.Core;
using IntegrationsBenchmark.Flats;
using IntegrationsBenchmark.WebApi.Services.Interfaces;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace IntegrationsBenchmark.WebApi.Services
{
    public class FlatGreedyWeatherForecasterService : FlatWeatherForecasterGreedy.FlatWeatherForecasterGreedyServerBase
    {
        private static readonly Empty Empty = new Empty();

        private readonly IWeatherForecasterService _service;

        public FlatGreedyWeatherForecasterService(IWeatherForecasterService service)
        {
            _service = service;
        }

        public override async Task<WeatherDataGreedyContainer> Forecast(Empty request, ServerCallContext context)
        {
            var serviceResult = await _service.Forecast(31);
            var forecasts = new List<WeatherDataGreedy>();
            serviceResult.Select(forecast =>
                new WeatherDataGreedy
                {
                    Date = forecast.Date.ToUniversalTime().Ticks,
                    Summary = forecast.Summary,
                    TemperatureC = forecast.TemperatureC,
                    TemperatureF = forecast.TemperatureF
                })
                .ToList()
                .ForEach(forecasts.Add);

            var response = new WeatherDataGreedyContainer()
            {
                Forecasts = forecasts
            };
            return response;
        }

        public override async Task ForecastHalfDuplexStream(Empty request, IServerStreamWriter<WeatherDataGreedy> responseStream, ServerCallContext context)
        {
            var serviceResult = await _service.Forecast(31);
            foreach (var weatherData in serviceResult)
                await responseStream.WriteAsync(new WeatherDataGreedy
                {
                    Date = weatherData.Date.ToUniversalTime().Ticks,
                    Summary = weatherData.Summary,
                    TemperatureC = weatherData.TemperatureC,
                    TemperatureF = weatherData.TemperatureF
                });
        }

        public override async Task ForecastFullDuplexStream(IAsyncStreamReader<Empty> requestStream, IServerStreamWriter<ForecastGreedyFullDuplexResponse> responseStream, ServerCallContext context)
        {
            try
            {
                while (!context.CancellationToken.IsCancellationRequested && await requestStream.MoveNext())
                {
                    var request = requestStream.Current;
                    if (request == null)
                        continue;
                    var serviceResult = await _service.Forecast(31);
                    foreach (var weatherData in serviceResult)
                        await responseStream.WriteAsync(new ForecastGreedyFullDuplexResponse
                        {
                            Type = ForecastGreedyFullDuplexResponseType.Item,
                            Item = new WeatherDataGreedy
                            {
                                Date = weatherData.Date.ToUniversalTime().Ticks,
                                Summary = weatherData.Summary,
                                TemperatureC = weatherData.TemperatureC,
                                TemperatureF = weatherData.TemperatureF
                            }
                        });
                    await responseStream.WriteAsync(new ForecastGreedyFullDuplexResponse
                    {
                        Type = ForecastGreedyFullDuplexResponseType.End
                    });
                }
            }
            catch (RpcException ex) when (ex.StatusCode == StatusCode.Cancelled)
            {
                //
            }
            catch (IOException ex) when (ex.Message == "The client reset the request stream.")
            {
                //
            }
        }
    }
}
