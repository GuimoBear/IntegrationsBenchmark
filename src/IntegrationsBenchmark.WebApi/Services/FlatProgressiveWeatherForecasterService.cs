using Grpc.Core;
using IntegrationsBenchmark.Flats;
using IntegrationsBenchmark.WebApi.Services.Interfaces;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace IntegrationsBenchmark.WebApi.Services
{
    public class FlatProgressiveWeatherForecasterService : FlatWeatherForecasterProgressive.FlatWeatherForecasterProgressiveServerBase
    {
        private static readonly Empty Empty = new Empty();

        private readonly IWeatherForecasterService _service;

        public FlatProgressiveWeatherForecasterService(IWeatherForecasterService service)
        {
            _service = service;
        }

        public override async Task<WeatherDataProgressiveContainer> Forecast(Empty request, ServerCallContext context)
        {
            var serviceResult = await _service.Forecast(31);
            var forecasts = new List<WeatherDataProgressive>();
            serviceResult.Select(forecast =>
                new WeatherDataProgressive
                {
                    Date = forecast.Date.ToUniversalTime().Ticks,
                    Summary = forecast.Summary,
                    TemperatureC = forecast.TemperatureC,
                    TemperatureF = forecast.TemperatureF
                })
                .ToList()
                .ForEach(forecasts.Add);

            var response = new WeatherDataProgressiveContainer()
            {
                Forecasts = forecasts
            };
            return response;
        }

        public override async Task ForecastHalfDuplexStream(Empty request, IServerStreamWriter<WeatherDataProgressive> responseStream, ServerCallContext context)
        {
            var serviceResult = await _service.Forecast(31);
            foreach (var weatherData in serviceResult)
                await responseStream.WriteAsync(new WeatherDataProgressive
                {
                    Date = weatherData.Date.ToUniversalTime().Ticks,
                    Summary = weatherData.Summary,
                    TemperatureC = weatherData.TemperatureC,
                    TemperatureF = weatherData.TemperatureF
                });
        }

        public override async Task ForecastFullDuplexStream(IAsyncStreamReader<Empty> requestStream, IServerStreamWriter<ForecastProgressiveFullDuplexResponse> responseStream, ServerCallContext context)
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
                        await responseStream.WriteAsync(new ForecastProgressiveFullDuplexResponse
                        {
                            Type = ForecastProgressiveFullDuplexResponseType.Item,
                            Item = new WeatherDataProgressive
                            {
                                Date = weatherData.Date.ToUniversalTime().Ticks,
                                Summary = weatherData.Summary,
                                TemperatureC = weatherData.TemperatureC,
                                TemperatureF = weatherData.TemperatureF
                            }
                        });
                    await responseStream.WriteAsync(new ForecastProgressiveFullDuplexResponse
                    {
                        Type = ForecastProgressiveFullDuplexResponseType.End
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
