using System.Diagnostics;
using MassTransit;

namespace CS.Configuration.MassTransit.Filters;

public class DiagnosticsTracingSendFilter : IFilter<SendContext>
{
    public async Task Send(SendContext context, IPipe<SendContext> next)
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

    public void Probe(ProbeContext context)
    {
    }
}