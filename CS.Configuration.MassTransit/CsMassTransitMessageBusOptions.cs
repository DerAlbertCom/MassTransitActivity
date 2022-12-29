namespace CS.Configuration.MassTransit;

public class CsMassTransitMessageBusOptions
{
    public string Host { get; set; } = "localhost";
    public string Username { get; set; } = "admin";
    public string Password { get; set; } = "admin";
}