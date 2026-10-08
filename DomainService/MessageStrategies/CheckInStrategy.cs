using Device.Domain;
using MQTTnet;
using Infrastructure.RepositoryCore;
using Infrastructure.Mqtt;

namespace DomainService.MessageStrategies;

public class CheckInStrategy(IMqttProvider mqttProvider) : IMessageStrategy
{
    public string Topic => "fromdevice/check";

    public async Task HandleMessage(string payload)
    {
        Console.WriteLine($"Device {payload} checked in");
        var mess = new MqttApplicationMessageBuilder();
        var client = await mqttProvider.GetClient();
        mess.WithPayload("ota_good");
        mess.WithTopic("todevice/"+payload);
        await client.PublishAsync(mess.Build());
    }
}
