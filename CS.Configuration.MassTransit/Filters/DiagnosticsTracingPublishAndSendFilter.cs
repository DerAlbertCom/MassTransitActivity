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
        // if the traceparent header is present, don't override so that trace parent is the same in all
        // messages in a saga.
        // masstransit passes Headers if you Publish messages over an existing Context.
        if (context.Headers.Get<string>(HeaderNames.TraceParent) != null)
        {
            return;
        }
        
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