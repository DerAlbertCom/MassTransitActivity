using System.Diagnostics;
using MassTransit;
using MassTransitActivity.Contracts.Sagas;

namespace MassTransitActivity.Worker.Sagas;

public class SampleStateMachine : MassTransitStateMachine<SampleState>
{
    public State? Step1Completed { get; private set; } = null;
    public State? Step2Completed { get; set; } = null;
    public State? Step3Completed { get; set; } = null;

    public SampleStateMachine()
    {
        InstanceState(x => x.CurrentState, Step1Completed, Step2Completed, Step3Completed);
        DeclareEvents();
        Initially(
            When(RunStep1)
//                .Send((c) => c.Send(new RunStep1()))
                .TransitionTo(Step1Completed),
            When(RunStep2)
//                .Send((c) => c.Send(new RunStep2()))
                .TransitionTo(Step2Completed),
            When(RunStep3)
                .Send((c) => c.Send(new RunStep3()))
                .TransitionTo(Step3Completed),
            When(OneStepped).TransitionTo(Step1Completed),
            When(TwoStepped).TransitionTo(Step2Completed),
            When(ThreeStepped).TransitionTo(Step3Completed).Finalize()
        );
        SetCompletedWhenFinalized();
    }


    private void DeclareEvents()
    {
        Event(() => OneStepped, x => x.CorrelateById((a) => CorrelationId()));
        Event(() => TwoStepped, x => x.CorrelateById((a) => CorrelationId()));
        Event(() => ThreeStepped, x => x.CorrelateById((a) => CorrelationId()));
    }

    private Guid CorrelationId()
    {
        var activity = Activity.Current!;
        return new Guid(activity.TraceId.ToHexString());
    }

    public Event<RunStep1>? RunStep1 { get; private set; } = null;
    public Event<RunStep2>? RunStep2 { get; private set; } = null;
    public Event<RunStep3>? RunStep3 { get; private set; } = null;
    public Event<OneStepped>? OneStepped { get; private set; } = null;
    public Event<TwoStepped>? TwoStepped { get; private set; } = null;
    public Event<ThreeStepped>? ThreeStepped { get; private set; } = null;
}