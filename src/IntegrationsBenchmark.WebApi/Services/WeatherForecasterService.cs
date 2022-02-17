using IntegrationsBenchmark.WebApi.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IntegrationsBenchmark.WebApi.Services
{
    public class WeatherForecasterService : IWeatherForecasterService
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        public Task<IEnumerable<WeatherForecast>> Forecast(int days)
        {
            var rng = new Random();
            return Task.FromResult<IEnumerable<WeatherForecast>>(
                Enumerable.Range(1, days).Select(index => new WeatherForecast
                {
                    Date = DateTime.Now.AddDays(index),
                    TemperatureC = rng.Next(-20, 55),
                    Summary = Summaries[rng.Next(Summaries.Length)]
                })
                .ToArray());
        }
    }
}
