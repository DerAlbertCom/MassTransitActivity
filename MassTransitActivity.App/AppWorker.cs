
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using MassTransit;
using MassTransitActivity.Contracts;
using MassTransitActivity.Contracts.Sagas;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace MassTransitActivity.App;

public class AppWorker : BackgroundService
{
    private readonly IBus _bus;
    private readonly ILogger<AppWorker> _logger;

    public AppWorker(IBus bus,ILogger<AppWorker> logger)
    {
        _bus = bus;
        _logger = logger;
    }

    [SuppressMessage("Reliability", "CA2017:Parameter count mismatch", Justification = "TraceId und SpanId")]
    [SuppressMessage("ReSharper", "StructuredMessageTemplateProblem", Justification = "")]
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        int counter = 0;
        while (!stoppingToken.IsCancellationRequested)
        {
            counter++;
            using (var activity = new Activity("AppWorker.ExecuteAsync"))
            {
                activity.Start();
                _logger.LogInformation("App Trace {TraceId} {SpanId}");
                await _bus.Publish(new GettingStarted() { Value = $"It is {DateTime.Now}" });
                if (counter % 2 == 0)
                {
                    await _bus.Publish(new RunStep1());
                }
            }
            await Task.Delay(5000);
        }

    }
}