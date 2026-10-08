using DomainService;
using Infrastructure.Mqtt;
using MQTTnet;

namespace Application.Processors;

public class DeviceMessageProcessor(IMqttProvider provider, IDeviceDomainService domainService, ILogger<DeviceMessageProcessor> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var mqttFactory = new MqttClientFactory();
        var mqttClient = await provider.GetClient();

        //     var mess = new MqttApplicationMessageBuilder();
        //             mess.WithPayload("restart");
        //             mess.WithTopic("todevice/30EDA0E2549E");
        //             var res = await mqttClient.PublishAsync(mess.Build());
        // return;
        var mqttSubscribeOptions = mqttFactory.CreateSubscribeOptionsBuilder().WithTopicFilter("fromdevice/+").Build();
        mqttClient.ApplicationMessageReceivedAsync += async e =>
        {
            var mess = new MqttApplicationMessageBuilder();
            var payload = e.ApplicationMessage.ConvertPayloadToString();
            var topic = e.ApplicationMessage.Topic;

            try 
            {
                await domainService.ConsumeMessage( topic, payload);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Had an issue for {topic} with {payload}", topic, payload);
            }
        };

        await mqttClient.SubscribeAsync(mqttSubscribeOptions, stoppingToken);

        while(mqttClient.IsConnected && !stoppingToken.IsCancellationRequested)
        {
            await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
        }

        Environment.Exit(-1);
    }

}
