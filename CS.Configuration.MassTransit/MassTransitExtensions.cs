using CS.Configuration.MassTransit.Filters;
using MassTransit;

namespace CS.Configuration.MassTransit;

public static class MassTransitExtensions
{
    public static IBusFactoryConfigurator UseDiagnosticsInstrumentation(
        this IBusFactoryConfigurator cfg)
    {
        cfg.ConfigurePublish(c => { c.AddPipeSpecification(new DiagnosticTracingPipeSpecification()); });
        cfg.ConfigureSend(c => { c.AddPipeSpecification(new DiagnosticTracingPipeSpecification()); });
        // for consuming
        cfg.AddPipeSpecification(new DiagnosticTracingPipeSpecification());
        return cfg;
    }
}