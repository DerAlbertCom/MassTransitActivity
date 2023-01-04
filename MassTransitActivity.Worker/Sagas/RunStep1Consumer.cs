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
        _logger.LogInformation($"{nameof(RunStep1Consumer)} {{TraceId}} {{ParentId}}");
        await context.RespondAsync(new OneStepped());
    }
}

public class RunStep2Consumer : IConsumer<RunStep2>
{
    private readonly ILogger<RunStep1Consumer> _logger;

    public RunStep2Consumer(ILogger<RunStep1Consumer> logger)
    {
        _logger = logger;
    }

    [SuppressMessage("ReSharper", "StructuredMessageTemplateProblem")]
    [SuppressMessage("Reliability", "CA2017:Parameter count mismatch")]
    public async Task Consume(ConsumeContext<RunStep2> context)
    {
        _logger.LogInformation($"{nameof(RunStep2Consumer)}, Consume");
        _logger.LogInformation($"{nameof(RunStep1Consumer)} {{TraceId}} {{ParentId}}");
        await context.RespondAsync(new TwoStepped());
    }
}

public class RunStep3Consumer : IConsumer<RunStep3>
{
    private readonly ILogger<RunStep1Consumer> _logger;

    public RunStep3Consumer(ILogger<RunStep1Consumer> logger)
    {
        _logger = logger;
    }

    [SuppressMessage("ReSharper", "StructuredMessageTemplateProblem")]
    [SuppressMessage("Reliability", "CA2017:Parameter count mismatch")]
    public async Task Consume(ConsumeContext<RunStep3> context)
    {
        _logger.LogInformation($"{nameof(RunStep3Consumer)}, Consume");
        _logger.LogInformation($"{nameof(RunStep3Consumer)} {{TraceId}} {{ParentId}}");
        await context.RespondAsync(new ThreeStepped());
    }
}