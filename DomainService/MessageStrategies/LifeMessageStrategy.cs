using Device.Domain;
using Infrastructure.RepositoryCore;

namespace DomainService.MessageStrategies;

public class LifeMessageStrategy(IDeviceRepository deviceRepository) : IMessageStrategy
{
    public string Topic => "fromdevice/alive";

    public async Task HandleMessage(string payload)
    {
        if (!await deviceRepository.DeviceExists(payload))
        {
            await CreateNewDevice(payload);
        }
        else
        {
          await UpdateDevice(payload);
        }
    }

    private async Task UpdateDevice(string serialNumber)
    {
      var device = await deviceRepository.GetDeviceAsync(serialNumber);

      device.LastHeardFrom = DateTime.UtcNow;
      device.IsOnline = true;

      await deviceRepository.SetDeviceAsync(device);
    }

    private async Task CreateNewDevice(string serialNumber)
    {
        await deviceRepository.CreateDeviceAsync( 
            new DeviceModel 
            {
              SerialNumber = serialNumber,
              IsOnline = true,
            }
        );

        return;
    }
}
