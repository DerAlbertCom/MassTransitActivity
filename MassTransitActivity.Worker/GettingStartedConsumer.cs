using System.Diagnostics.CodeAnalysis;
using MassTransit;
using MassTransitActivity.Contracts;
using Microsoft.Extensions.Logging;

namespace MassTransitActivity.Worker;

public class GettingStartedConsumer :
    IConsumer<GettingStarted>
{
    private readonly ILogger<GettingStartedConsumer> _logger;

    public GettingStartedConsumer(ILogger<GettingStartedConsumer> logger)
    {
        _logger = logger;
    }

    [SuppressMessage("Reliability", "CA2017:Parameter count mismatch", Justification = "TraceId und SpanId")]
    [SuppressMessage("ReSharper", "StructuredMessageTemplateProblem", Justification = "dito")]
    public Task Consume(ConsumeContext<GettingStarted> context)
    {
        _logger.LogInformation("Received Text: {Text}", context.Message.Value);
        _logger.LogInformation("Received Trace: {TraceId} {SpanId} {ParentId}");
        return Task.CompletedTask;
    }
}

class GettingStartedConsumerDefinition :
    ConsumerDefinition<GettingStartedConsumer>
{
    public GettingStartedConsumerDefinition()
    {
        // override the default endpoint name
        EndpointName = "getting-started";

        // limit the number of messages consumed concurrently
        // this applies to the consumer only, not the endpoint
        ConcurrentMessageLimit = 4;
    }

    protected override void ConfigureConsumer(IReceiveEndpointConfigurator endpointConfigurator,
        IConsumerConfigurator<GettingStartedConsumer> consumerConfigurator)
    {
        // configure message retry with millisecond intervals
        endpointConfigurator.UseMessageRetry(r => r.Intervals(100, 200, 500, 800, 1000));

        // use the outbox to prevent duplicate events from being published
        endpointConfigurator.UseInMemoryOutbox();
    }
}