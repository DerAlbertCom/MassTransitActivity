using MassTransit;
using MassTransit.Configuration;

namespace CS.Configuration.MassTransit.Filters;

public class DiagnosticTracingPipeSpecification : IPipeSpecification<ConsumeContext>, IPipeSpecification<PublishContext>
{
    public IEnumerable<ValidationResult> Validate()
    {
        return Enumerable.Empty<ValidationResult>();
    }

    public void Apply(IPipeBuilder<ConsumeContext> builder)
    {
        builder.AddFilter(new DiagnosticsTracingConsumeFilter());
    }

    public void Apply(IPipeBuilder<PublishContext> builder)
    {
        builder.AddFilter(new DiagnosticsTracingPublishFilter());
    }
}