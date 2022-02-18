using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Hosting;
using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace IntegrationsBenchmark.WebApi
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            await Task.WhenAll(
                CreateRestHostBuilder(args).Build().RunAsync(),
                CreateGrpcHostBuilder(args).Build().RunAsync());
        }

        public static IHostBuilder CreateRestHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<StartupRest>();
                });

        public static IHostBuilder CreateGrpcHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<StartupGrpc>();
                    webBuilder.ConfigureKestrel(kestrelOptions =>
                    {
                        foreach (var url in (Environment.GetEnvironmentVariable("ASPNETCORE_GRPC_URLS") ?? "").Split(';', StringSplitOptions.RemoveEmptyEntries))
                        {
                            var match = Regex.Match(url, @"^(?<protocol>http|https):.+:(?<port>\d+)$");
                            if (match.Success)
                            {
                                kestrelOptions.ListenAnyIP(int.Parse(match.Groups["port"].Value), listenOptions =>
                                {
                                    if (match.Groups["protocol"].Value == "https")
                                    {
                                        listenOptions.Protocols = HttpProtocols.Http1AndHttp2AndHttp3;
                                        listenOptions.UseHttps(
                                            Environment.GetEnvironmentVariable("ASPNETCORE_Kestrel__Certificates__Default__Path"),
                                            Environment.GetEnvironmentVariable("ASPNETCORE_Kestrel__Certificates__Default__Password"));
                                    }
                                    else
                                        listenOptions.Protocols = HttpProtocols.Http2;
                                });
                            }
                        }
                    });
                });
    }
}
