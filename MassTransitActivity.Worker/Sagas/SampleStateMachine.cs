using System.Diagnostics;
using MassTransit;
using MassTransitActivity.Contracts.Sagas;

namespace MassTransitActivity.Worker.Sagas;

public class SampleStateMachine : MassTransitStateMachine<SampleState>
{
    public State? StartState { get; private set; } = null;
    public State? Step1Completed { get; private set; } = null;
    public State? Step2Completed { get; set; } = null;
    public State? Step3Completed { get; set; } = null;

    public SampleStateMachine()
    {
        InstanceState(x => x.CurrentState, StartState);
        DeclareEvents();
        Initially(
            When(OneStepped)
                .Then(c => c.Saga.RunSteps = c.Message.Step)
                .ThenAsync(context => SendCommand(new RunStep2(), context))
                .TransitionTo(Step1Completed)
        );

        During(Step1Completed,
            When(TwoStepped)
                .Then(c => c.Saga.RunSteps = c.Message.Step)
                .ThenAsync(context => SendCommand(new RunStep3(), context))
                .TransitionTo(Step2Completed)
        );

        During(Step2Completed,
            When(ThreeStepped)
                .Then(c => c.Saga.RunSteps = c.Message.Step)
                .TransitionTo(Step3Completed)
                .Finalize()
        );

        SetCompletedWhenFinalized();
    }

    private async Task SendCommand<TCommand>(TCommand command, BehaviorContext<SampleState, object> context)
    {
        await context.Publish(command!);
    }


    private void DeclareEvents()
    {
        Event(() => OneStepped, x => x.CorrelateById((c) => TraceId(c)));
        Event(() => TwoStepped, x => x.CorrelateById((c) => TraceId(c)));
        Event(() => ThreeStepped, x => x.CorrelateById((c) => TraceId(c)));
    }

    private Guid TraceId(ConsumeContext consumeContext)
    {
        var activity = Activity.Current;
        if (activity == null)
        {
            return consumeContext.CorrelationId ?? Guid.NewGuid();
        }
        else
        {
            return new Guid(activity.TraceId.ToHexString());
        }
    }

    // public Event<RunStep1>? RunStep1 { get; private set; } = null;
    // public Event<RunStep2>? RunStep2 { get; private set; } = null;
    // public Event<RunStep3>? RunStep3 { get; private set; } = null;
    public Event<IOneStepped>? OneStepped { get; private set; } = null;
    public Event<ITwoStepped>? TwoStepped { get; private set; } = null;
    public Event<IThreeStepped>? ThreeStepped { get; private set; } = null;
}