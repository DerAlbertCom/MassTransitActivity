using MassTransitActivity.Contracts.Sagas;

namespace MassTransitActivity.Worker.Sagas;

public class ThreeStepped : IThreeStepped
{
    public int Step { get; } = 3;
}