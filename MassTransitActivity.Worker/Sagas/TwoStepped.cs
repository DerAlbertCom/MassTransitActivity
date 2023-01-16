using MassTransitActivity.Contracts.Sagas;

namespace MassTransitActivity.Worker.Sagas;

public class TwoStepped : ITwoStepped
{
    public int Step { get; } = 2;
}