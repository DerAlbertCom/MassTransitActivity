using System.Diagnostics;
using MassTransit;

namespace CS.Configuration.MassTransit.Filters;

public class DiagnosticsTracingPublishAndSendFilter : IFilter<PublishContext>, IFilter<SendContext>
{
    public void Probe(ProbeContext context)
    {
    }

    public async Task Send(SendContext context, IPipe<SendContext> next)
    {
        UpdateContext(context);

        await next.Send(context);
    }


    public async Task Send(PublishContext context, IPipe<PublishContext> next)
    {
        UpdateContext(context);

        await next.Send(context);
    }

    private static void UpdateContext(SendContext context)
    {
        if (Activity.Current != null)
        {
            context.Headers.Set(HeaderNames.TraceParent, Activity.Current.Id);
            if (context.CorrelationId == null)
            {
                context.CorrelationId = new Guid(Activity.Current.TraceId.ToHexString());
            }
        }
    }
}