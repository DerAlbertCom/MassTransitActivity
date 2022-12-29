using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace CS.Configuration.MassTransit;

public static class MassTransitActiveMqExtensions
{
    public static IBusRegistrationConfigurator UsingActiveMqCs(
        this IBusRegistrationConfigurator configurator,
        Action<IBusRegistrationContext, IActiveMqBusFactoryConfigurator>? configure = null)
    {
        configurator.UsingActiveMq((registrationContext, cfg) =>
        {
            var options = registrationContext.GetRequiredService<IOptions<CsMassTransitMessageBusOptions>>().Value;
            cfg.Host(options.Host, h =>
            {
                h.Username(options.Username);
                h.Password(options.Password);
            });
            cfg.UseDiagnosticsInstrumentation();
            cfg.ConfigureEndpoints(registrationContext);
            configure?.Invoke(registrationContext, cfg);
        });

        return configurator;
    }
}