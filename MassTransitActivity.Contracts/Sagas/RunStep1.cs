namespace MassTransitActivity.Contracts.Sagas;

public record RunStep1;

public interface IOneStepped
{
    int Step { get; }
}

public record RunStep2;

public interface ITwoStepped
{
    int Step { get; }
}

public record RunStep3;

public interface IThreeStepped
{
    int Step { get; }
}