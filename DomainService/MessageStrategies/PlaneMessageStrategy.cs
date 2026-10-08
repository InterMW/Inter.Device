using Infrastructure.Rabbit;

namespace DomainService.MessageStrategies;

public class PlaneMessageStrategy(IPlaneFramePublisher publisher) : IMessageStrategy
{
    public string Topic => "fromdevice/plane";

    public async Task HandleMessage(string payload)
    {
        var parts = payload.Split(":");
        await publisher.SendMessage(parts[0], parts[1]);
    }
}
