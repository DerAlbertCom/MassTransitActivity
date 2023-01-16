using System.Diagnostics;
using MassTransit;

namespace CS.Configuration.MassTransit.Filters;

public class DiagnosticsTracingPublishFilter : IFilter<PublishContext>
{
    public void Probe(ProbeContext context)
    {
    }

    public async Task Send(PublishContext context, IPipe<PublishContext> next)
    {
        if (Activity.Current != null)
        {
            context.Headers.Set(HeaderNames.TraceParent, Activity.Current.Id);
            if (context.CorrelationId == null)
            {
                context.CorrelationId = new Guid(Activity.Current.TraceId.ToHexString());
            }
        }

        await next.Send(context);
    }
}