using Grpc.Core;
using IntegrationsBenchmark.Flats;
using IntegrationsBenchmark.WebApi.Services.Interfaces;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace IntegrationsBenchmark.WebApi.Services
{
    public class FlatWeatherForecasterService : FlatWeatherForecaster.FlatWeatherForecasterServerBase
    {
        private static readonly Empty Empty = new Empty();

        private readonly IWeatherForecasterService _service;

        public FlatWeatherForecasterService(IWeatherForecasterService service)
        {
            _service = service;
        }

        public override async Task<WeatherDataContainer> Forecast(Empty request, ServerCallContext context)
        {
            var serviceResult = await _service.Forecast(31);
            var forecasts = new List<WeatherData>();
            serviceResult.Select(forecast =>
                new WeatherData
                {
                    Date = forecast.Date.ToUniversalTime().Ticks,
                    Summary = forecast.Summary,
                    TemperatureC = forecast.TemperatureC,
                    TemperatureF = forecast.TemperatureF
                })
                .ToList()
                .ForEach(forecasts.Add);

            var response = new WeatherDataContainer()
            {
                Forecasts = forecasts
            };
            return response;
        }

        public override async Task ForecastHalfDuplexStream(Empty request, IServerStreamWriter<WeatherData> responseStream, ServerCallContext context)
        {
            var serviceResult = await _service.Forecast(31);
            foreach (var weatherData in serviceResult)
                await responseStream.WriteAsync(new WeatherData
                {
                    Date = weatherData.Date.ToUniversalTime().Ticks,
                    Summary = weatherData.Summary,
                    TemperatureC = weatherData.TemperatureC,
                    TemperatureF = weatherData.TemperatureF
                });
        }

        public override async Task ForecastFullDuplexStream(IAsyncStreamReader<Empty> requestStream, IServerStreamWriter<ForecastFullDuplexResponse> responseStream, ServerCallContext context)
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
                        await responseStream.WriteAsync(new ForecastFullDuplexResponse
                        {
                            Type = ForecastFullDuplexResponseType.Item,
                            Item = new WeatherData
                            {
                                Date = weatherData.Date.ToUniversalTime().Ticks,
                                Summary = weatherData.Summary,
                                TemperatureC = weatherData.TemperatureC,
                                TemperatureF = weatherData.TemperatureF
                            }
                        });
                    await responseStream.WriteAsync(new ForecastFullDuplexResponse
                    {
                        Type = ForecastFullDuplexResponseType.End
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
