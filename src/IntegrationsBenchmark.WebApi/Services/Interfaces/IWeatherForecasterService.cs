using System.Collections.Generic;
using System.Threading.Tasks;

namespace IntegrationsBenchmark.WebApi.Services.Interfaces
{
    public interface IWeatherForecasterService
    {
        Task<IEnumerable<WeatherForecast>> Forecast(int days);
    }
}
