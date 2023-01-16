using System.Diagnostics;
using MassTransit;
using MassTransitActivity.Contracts.Sagas;
using Microsoft.Extensions.Logging;

namespace MassTransitActivity.Worker.Sagas;

public class SampleStateMachine : MassTransitStateMachine<SampleState>
{
    public State? StartState { get; private set; } = null;
    public State? Step2Completed { get; set; } = null;
    public State? Step3Completed { get; set; } = null;
    public State? Step1Completed { get; private set; } = null;


    public SampleStateMachine(ILogger<SampleStateMachine> logger)
    {
        // why .Publish() instead of .Send() https://stackoverflow.com/questions/62713786/masstransit-endpointconvention-azure-service-bus/62714778#62714778


        // state definition order is relevant for the int value of the CurrentState
        InstanceState(x => x.CurrentState, StartState, Step1Completed, Step2Completed, Step3Completed);
        DeclareEvents();
        Initially(
            When(OneStepped)
                .Then(c => c.Saga.RunSteps = c.Message.Step)
                .Then(c => logger.LogInformation("Step {Step} run in State {State}", c.Saga.RunSteps,
                    c.Saga.CurrentState))
                .TransitionTo(Step1Completed)
                .Publish(new RunStep2())
                .Then(c => logger.LogInformation("Step {Step} completed in State {State}", c.Saga.RunSteps,
                    c.Saga.CurrentState))
        );

        During(Step1Completed,
            When(TwoStepped)
                .Then(c => c.Saga.RunSteps = c.Message.Step)
                .Then(c => logger.LogInformation("Step {Step} run in State {State}", c.Saga.RunSteps,
                    c.Saga.CurrentState))
                .TransitionTo(Step2Completed)
                .Publish(new RunStep3())
                .Then(c => logger.LogInformation("Step {Step} completed in State {State}", c.Saga.RunSteps,
                    c.Saga.CurrentState))
        );

        During(Step2Completed,
            When(ThreeStepped)
                .Then(c => c.Saga.RunSteps = c.Message.Step)
                .Then(c => logger.LogInformation("Step {Step} run in State {State}", c.Saga.RunSteps,
                    c.Saga.CurrentState))
                .TransitionTo(Step3Completed)
                .Then(c => logger.LogInformation("Step {Step} completed in State {State}", c.Saga.RunSteps,
                    c.Saga.CurrentState))
                .Finalize()
                .Then(c => logger.LogInformation("Step {Step} finalized in State {State}", c.Saga.RunSteps,
                    c.Saga.CurrentState))
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