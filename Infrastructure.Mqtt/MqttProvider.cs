using MQTTnet;

namespace Infrastructure.Mqtt;

public interface IMqttProvider
{
    Task<IMqttClient> GetClient();
}

public class MqttProvider(MqttClientOptions clientOptions) : IMqttProvider
{
    private IMqttClient client = null;

    public async Task<IMqttClient> GetClient()
    {
        if (client is null)
        {
          var mqttFactory = new MqttClientFactory();
          client = mqttFactory.CreateMqttClient();
          await client.ConnectAsync(clientOptions, CancellationToken.None);
        }

        return client;
    }
}
