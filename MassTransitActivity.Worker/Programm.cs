using System.Diagnostics;
using CS.Configuration.MassTransit;
using MassTransit;
using MassTransitActivity.Contracts.Sagas;
using MassTransitActivity.Worker;
using MassTransitActivity.Worker.Sagas;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Enrichers.Span;

IHostBuilder CreateHostBuilder(string[] args)
{
    Activity.DefaultIdFormat = ActivityIdFormat.W3C;
    GlobalTopology.Send.UseCorrelationId<RunStep1>((_) =>
    {
        var activity = Activity.Current!;
        return new Guid(activity.TraceId.ToHexString());
    });
    GlobalTopology.Send.UseCorrelationId<RunStep2>((_) =>
    {
        var activity = Activity.Current!;
        return new Guid(activity.TraceId.ToHexString());
    });
    GlobalTopology.Send.UseCorrelationId<RunStep3>((_) =>
    {
        var activity = Activity.Current!;
        return new Guid(activity.TraceId.ToHexString());
    });

    return Host.CreateDefaultBuilder(args)
        .ConfigureServices((_, services) =>
        {
            services.AddMassTransit(x =>
            {
                x.UsingActiveMqCs();
                x.AddConsumer<GettingStartedConsumer>();
                x.AddConsumer<RunStep1Consumer>();
                x.AddConsumer<RunStep2Consumer>();
                x.AddConsumer<RunStep3Consumer>();
                x.AddSagaStateMachine<SampleStateMachine, SampleState>()
                    .InMemoryRepository();
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