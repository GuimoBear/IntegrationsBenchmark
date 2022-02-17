using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Hosting;
using System;
using System.Net;

namespace IntegrationsBenchmark.WebApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                    webBuilder.ConfigureKestrel(kestrelOptions =>
                    {
                        kestrelOptions.ListenAnyIP(443, listenOptions =>
                        {
                            listenOptions.Protocols = HttpProtocols.Http1AndHttp2AndHttp3;
                            listenOptions.UseHttps(
                                Environment.GetEnvironmentVariable("ASPNETCORE_Kestrel__Certificates__Default__Path"), 
                                Environment.GetEnvironmentVariable("ASPNETCORE_Kestrel__Certificates__Default__Password"));
                        });
                    });
                });
    }
}
