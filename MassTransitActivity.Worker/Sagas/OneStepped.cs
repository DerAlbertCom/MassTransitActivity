using MassTransitActivity.Contracts.Sagas;

namespace MassTransitActivity.Worker.Sagas;

public class OneStepped : IOneStepped
{
    public int Step { get; } = 1;
}