using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using System;
using System.Reflection;
using Serilog;
using Serilog.Events;

namespace Wool.DevChallenge.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var appName = Assembly.GetExecutingAssembly().GetName().Name;
            Console.WriteLine($"Starting service: {appName}");
            Console.Title = appName;
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
                .Enrich.FromLogContext()
                .WriteTo.Console()
                .CreateLogger();

            Log.Information("Starting web host {AppName}", appName);
            CreateHostBuilder(args).Build().Run();

            Log.CloseAndFlush();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseSerilog((hostingContext, loggerConfiguration) => loggerConfiguration
                    .ReadFrom.Configuration(hostingContext.Configuration)
                )
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
