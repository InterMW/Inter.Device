using Device.Common;
using Device.Domain;
using Infrastructure.RepositoryCore;

namespace DomainService.MessageStrategies;

public class InfoMessageStrategy(IDeviceRepository deviceRepository, IIpRepository ipRepository) : IMessageStrategy
{
    public string Topic => "fromdevice/info";

    public async Task HandleMessage(string payload)
    {
        Console.WriteLine(payload);
        bool didChange = false;
        var parts = payload.Split(':');

        var serialNumber = parts[0];

        var temperature = int.Parse(parts[1]);

        var ip = parts[2];

        DeviceModel device;
        try
        {
            device = await deviceRepository.GetDeviceAsync(serialNumber);
        }
        catch (DeviceNotFoundException)
        {
            return;
        }

        if (device.Temperature != temperature)
        {
            didChange = true;
            device.Temperature = temperature;
        }

        if (device.IpAddress != ip)
        {
            didChange = true;
            var result = await ipRepository.Lookup(ip);

            if (result is not null)
            {
                device.Latitude = (float)(result.Latitude ?? 0);
                device.Longitude = (float)(result.Longitude ?? 0);
                device.IpAddress = ip;
            }
        }

        if (didChange)
        {
            await deviceRepository.SetDeviceAsync(device);
        }
    }
}
