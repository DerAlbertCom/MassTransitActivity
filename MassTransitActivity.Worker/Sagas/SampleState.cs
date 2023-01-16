using MassTransit;

namespace MassTransitActivity.Worker.Sagas;

public class SampleState : SagaStateMachineInstance
{
    public Guid CorrelationId { get; set; }
    public int CurrentState { get; set; }
    public int RunSteps { get; set; }
}