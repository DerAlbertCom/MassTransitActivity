using System.Diagnostics;
using Carmen.Configuration.MassTransit;
using MassTransit;
using MassTransitActivity.Worker;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Enrichers.Span;

IHostBuilder CreateHostBuilder(string[] args)
{
    Activity.DefaultIdFormat = ActivityIdFormat.W3C;

    return Host.CreateDefaultBuilder(args)
        .ConfigureServices((_, services) =>
        {
            services.AddMassTransit(x =>
            {
                x.UsingActiveMq((context, cfg) =>
                {
                    cfg.Host("localhost", h =>
                    {
                        h.Username("admin");
                        h.Password("admin");
                    });
                    cfg.UseDiagnosticsInstrumentation();
                    cfg.ConfigureEndpoints(context);
                });
                x.AddConsumer<GettingStartedConsumer>();
            });
        }).ConfigureLogging(logBuilder =>
        {
            logBuilder.ClearProviders();
            var logger = new LoggerConfiguration()
                .Enrich.WithSpan()
                .WriteTo.ColoredConsole()
                .CreateLogger();
            logBuilder.AddSerilog(logger, true);
        });
}

var builder = CreateHostBuilder(args);
await builder.RunConsoleAsync();