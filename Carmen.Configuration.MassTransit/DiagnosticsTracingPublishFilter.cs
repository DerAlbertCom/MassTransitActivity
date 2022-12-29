using System.Diagnostics;
using MassTransit;

namespace Carmen.Configuration.MassTransit;

public class DiagnosticsTracingPublishFilter : IFilter<PublishContext>
{
    public void Probe(ProbeContext context)
    { }

    public async Task Send(PublishContext context, IPipe<PublishContext> next)
    {
        context.Headers.Set(HeaderNames.TraceParent, Activity.Current?.Id);
        await next.Send(context);
    }
}