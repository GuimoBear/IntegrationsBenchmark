using IntegrationsBenchmark.WebApi.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace IntegrationsBenchmark.WebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private readonly IWeatherForecasterService _service;

        public WeatherForecastController(IWeatherForecasterService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IEnumerable<WeatherForecast>> Get()
            => await _service.Forecast(31);
    }
}
