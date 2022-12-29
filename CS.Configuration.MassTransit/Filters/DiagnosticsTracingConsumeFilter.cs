using System.Diagnostics;
using MassTransit;

namespace CS.Configuration.MassTransit.Filters;

public class DiagnosticsTracingConsumeFilter : IFilter<ConsumeContext>
{
    public async Task Send(ConsumeContext context, IPipe<ConsumeContext> next)
    {
        if (context.Headers.TryGetHeader(HeaderNames.TraceParent, out var value))
        {
            var traceContext = value as string;
            if (traceContext != null)
            {
                var operationName = GetOperationName(context);
                using var activity = new Activity(operationName);
                activity.SetParentId(traceContext);
                activity.Start();
                await next.Send(context);
            }
        }
    }

    private string GetOperationName(ConsumeContext context)
    {
        string? operationName;

        if (context.DestinationAddress != null)
        {
            if (context.DestinationAddress.Segments.Length == 2)
            {
                operationName = context.DestinationAddress.AbsolutePath.Replace("/", "");
            }
            else
            {
                operationName = context.DestinationAddress.AbsolutePath;
            }
        }
        else
        {
            operationName = context.SupportedMessageTypes.FirstOrDefault();
            if (operationName != null)
            {
                operationName = operationName.Substring("urn:message:".Length);
            }
        }

        if (operationName == null)
        {
            throw new InvalidOperationException(
                $"Was not able to find an operation name for message id {context.MessageId}");
        }

        return operationName.Replace(":", ".");
    }

    public void Probe(ProbeContext context)
    {
    }
}