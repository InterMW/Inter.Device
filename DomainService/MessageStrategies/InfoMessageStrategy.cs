using Device.Domain;
using Infrastructure.RepositoryCore;

namespace DomainService.MessageStrategies;

public class InfoMessageStrategy() : IMessageStrategy
{
    public string Topic => "fromdevice/info";

    public async Task HandleMessage(string payload)
    {
        Console.WriteLine($"For now this is not being handled, but it looks like\n{payload}");
    }
}
