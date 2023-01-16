using System.Diagnostics;
using MassTransit;
using MassTransitActivity.Contracts.Sagas;
using Microsoft.Extensions.Logging;

namespace MassTransitActivity.Worker.Sagas;

public class SampleStateMachine : MassTransitStateMachine<SampleState>
{
    public State? StartState { get; private set; } = null;
    public State? Step1Completed { get; private set; } = null;
    public State? Step2Completed { get; set; } = null;
    public State? Step3Completed { get; set; } = null;

    public SampleStateMachine(ILogger<SampleStateMachine> logger)
    {
        // why .Publish() instead of .Send() https://stackoverflow.com/questions/62713786/masstransit-endpointconvention-azure-service-bus/62714778#62714778

        InstanceState(x => x.CurrentState, StartState);
        DeclareEvents();
        Initially(
            When(OneStepped)
                .Then(c => c.Saga.RunSteps = c.Message.Step)
                .TransitionTo(Step1Completed)
                .Then(c => logger.LogInformation("Step {Step} completed", c.Saga.RunSteps))
                .Publish(new RunStep2())
        );

        During(Step1Completed,
            When(TwoStepped)
                .Then(c => c.Saga.RunSteps = c.Message.Step)
                .TransitionTo(Step2Completed)
                .Then(c => logger.LogInformation("Step {Step} completed", c.Saga.RunSteps))
                .Publish(new RunStep3())
        );

        During(Step2Completed,
            When(ThreeStepped)
                .Then(c => c.Saga.RunSteps = c.Message.Step)
                .TransitionTo(Step3Completed)
                .Then(c => logger.LogInformation("Step {Step} completed", c.Saga.RunSteps))
                .Finalize()
        );

        SetCompletedWhenFinalized();
    }


    private void DeclareEvents()
    {
        Event(() => OneStepped, configurator => configurator.CorrelateById((context) => TraceId(context)));
        Event(() => TwoStepped, configurator => configurator.CorrelateById((context) => TraceId(context)));
        Event(() => ThreeStepped, configurator => configurator.CorrelateById((context) => TraceId(context)));
    }

    private Guid TraceId(ConsumeContext consumeContext)
    {
        if (consumeContext.CorrelationId != null)
        {
            return consumeContext.CorrelationId.Value;
        }

        var activity = Activity.Current;
        if (activity == null)
        {
            return consumeContext.CorrelationId ?? NewId.NextGuid();
        }
        else
        {
            return new Guid(activity.TraceId.ToHexString());
        }
    }

    public Event<IOneStepped>? OneStepped { get; private set; } = null;
    public Event<ITwoStepped>? TwoStepped { get; private set; } = null;
    public Event<IThreeStepped>? ThreeStepped { get; private set; } = null;
}