namespace DomainService.MessageStrategies;

public interface IMessageStrategy
{
    string Topic { get; }
    Task HandleMessage(string payload);
}
