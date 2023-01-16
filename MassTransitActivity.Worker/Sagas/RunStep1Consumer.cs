using System.Diagnostics.CodeAnalysis;
using MassTransit;
using MassTransitActivity.Contracts.Sagas;
using Microsoft.Extensions.Logging;

namespace MassTransitActivity.Worker.Sagas;

public class RunStep1Consumer : IConsumer<RunStep1>
{
    private readonly ILogger<RunStep1Consumer> _logger;

    public RunStep1Consumer(ILogger<RunStep1Consumer> logger)
    {
        _logger = logger;
    }

    [SuppressMessage("ReSharper", "StructuredMessageTemplateProblem")]
    [SuppressMessage("Reliability", "CA2017:Parameter count mismatch")]
    public async Task Consume(ConsumeContext<RunStep1> context)
    {
        _logger.LogInformation($"{nameof(RunStep1Consumer)}, Consume");
        _logger.LogInformation($"{nameof(RunStep1Consumer)} {{TraceId}} {{SpanId}} {{ParentId}}");
        await context.Publish<IOneStepped>(new { Step = 1 });
    }
}