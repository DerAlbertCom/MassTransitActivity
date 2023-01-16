using System.Diagnostics.CodeAnalysis;
using MassTransit;
using MassTransitActivity.Contracts.Sagas;
using Microsoft.Extensions.Logging;

namespace MassTransitActivity.Worker.Sagas;

public class RunStep2Consumer : IConsumer<RunStep2>
{
    private readonly ILogger<RunStep2Consumer> _logger;

    public RunStep2Consumer(ILogger<RunStep2Consumer> logger)
    {
        _logger = logger;
    }

    [SuppressMessage("ReSharper", "StructuredMessageTemplateProblem")]
    [SuppressMessage("Reliability", "CA2017:Parameter count mismatch")]
    public async Task Consume(ConsumeContext<RunStep2> context)
    {
        _logger.LogInformation($"{nameof(RunStep2Consumer)}, Consume");
        _logger.LogInformation($"{nameof(RunStep2Consumer)} {{TraceId}} {{SpanId}} {{ParentId}}");
        await context.Publish<ITwoStepped>(new TwoStepped());
    }
}