using IntegrationsBenchmark.WebApi.Services;
using IntegrationsBenchmark.WebApi.Services.Interfaces;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace IntegrationsBenchmark.WebApi
{
    public class StartupGrpc
    {
        public StartupGrpc(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddTransient<IWeatherForecasterService, WeatherForecasterService>();
            services.AddGrpc(options =>
            {
                options.ResponseCompressionLevel = System.IO.Compression.CompressionLevel.NoCompression;
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGrpcService<ProtoWeatherForecasterService>();
            });
        }
    }
}
