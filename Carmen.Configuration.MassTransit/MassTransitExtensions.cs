using MassTransit;

namespace Carmen.Configuration.MassTransit;

public static class MassTransitExtensions
{
    public static IBusFactoryConfigurator UseDiagnosticsInstrumentation(
        this IBusFactoryConfigurator cfg)
    {
        cfg.ConfigurePublish(c => { c.AddPipeSpecification(new DiagnosticTracingPipeSpecification()); });
        cfg.AddPipeSpecification(new DiagnosticTracingPipeSpecification());
        return cfg;
    }
}