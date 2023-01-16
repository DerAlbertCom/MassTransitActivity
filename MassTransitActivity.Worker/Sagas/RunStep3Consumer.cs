using System.Diagnostics.CodeAnalysis;
using MassTransit;
using MassTransitActivity.Contracts.Sagas;
using Microsoft.Extensions.Logging;

namespace MassTransitActivity.Worker.Sagas;

public class RunStep3Consumer : IConsumer<RunStep3>
{
    private readonly ILogger<RunStep3Consumer> _logger;

    public RunStep3Consumer(ILogger<RunStep3Consumer> logger)
    {
        _logger = logger;
    }

    [SuppressMessage("ReSharper", "StructuredMessageTemplateProblem")]
    [SuppressMessage("Reliability", "CA2017:Parameter count mismatch")]
    public async Task Consume(ConsumeContext<RunStep3> context)
    {
        _logger.LogInformation($"{nameof(RunStep3Consumer)}, Consume");
        _logger.LogInformation($"{nameof(RunStep3Consumer)} {{TraceId}} {{SpanId}} {{ParentId}}");
        await context.Publish<IThreeStepped>(new ThreeStepped());
    }
}