using System.Diagnostics;
using FluentAssertions;
using MassTransit;
using NSubstitute;

namespace CS.Configuration.MassTransit.Filters;

public class DiagnosticsTracingConsumeFilterTests
{
    private readonly DiagnosticsTracingConsumeFilter _sut;

    public DiagnosticsTracingConsumeFilterTests()
    {
        _sut = new DiagnosticsTracingConsumeFilter();
    }

    [Fact]
    public async Task Send_DoesNotCreatesAnActivity_WithMissingTraceParent()
    {
        Activity? createdActivity = null;
        var context = Substitute.For<ConsumeContext>();


        await SutSend(context, (_) => { createdActivity = Activity.Current; });

        createdActivity.Should().BeNull();
    }

    [Fact]
    public async Task Send_CreatesAnActivity_WithATraceParent()
    {
        Activity? createdActivity = null;
        var context = Substitute.For<ConsumeContext>();

        context.Headers.TryGetHeader("traceparent", out Arg.Any<object?>())
            .Returns(x =>
            {
                x[1] = "00-0af7651916cd43dd8448eb211c80319c-b7ad6b7169203331-01";
                return true;
            });
        context.SupportedMessageTypes.Returns(new[] { "DiesIstNurEinDummy" });

        await SutSend(context, (_) => { createdActivity = Activity.Current; });

        createdActivity.Should().NotBeNull();
    }

    [Fact]
    public async Task Send_CreatesAnActivity_WithCorrectIdentifiers()
    {
        Activity? createdActivity = null;
        var context = Substitute.For<ConsumeContext>();

        context.Headers.TryGetHeader("traceparent", out Arg.Any<object?>())
            .Returns(x =>
            {
                x[1] = "00-0af7651916cd43dd8448eb211c80319c-b7ad6b7169203331-01";
                return true;
            });
        context.SupportedMessageTypes.Returns(new[] { "HelloDiesIstEinDummy" });

        await SutSend(context, (_) => { createdActivity = Activity.Current; });

        createdActivity.Should().NotBeNull();

        createdActivity!.TraceId.ToHexString().Should().Be("0af7651916cd43dd8448eb211c80319c");
        createdActivity.ParentSpanId.ToHexString().Should().Be("b7ad6b7169203331");
    }

    private async Task SutSend(ConsumeContext context, Action<ConsumeContext> func)
    {
        await _sut.Send(context, new RunPipe((pipeContext) =>
        {
            func(pipeContext);
            return Task.CompletedTask;
        }));
    }
}

public class RunPipe : IPipe<ConsumeContext>
{
    private readonly Func<ConsumeContext, Task> _sendFunc;
    private readonly Action<ProbeContext>? _probeFunc;

    public RunPipe(Func<ConsumeContext, Task> sendFunc)
        : this(sendFunc, null)
    {
    }

    private RunPipe(Func<ConsumeContext, Task> sendFunc, Action<ProbeContext>? probeFunc)
    {
        _sendFunc = sendFunc;
        _probeFunc = probeFunc;
    }

    public async Task Send(ConsumeContext context)
    {
        await _sendFunc.Invoke(context);
    }

    public void Probe(ProbeContext context)
    {
        _probeFunc?.Invoke(context);
    }
}