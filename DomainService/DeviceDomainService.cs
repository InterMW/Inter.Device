using Device.Common;
using Device.Domain;
using Infrastructure.RepositoryCore;
using MelbergFramework.Core.Time;
using Microsoft.Extensions.Logging;

namespace DomainService;

public interface IDeviceDomainService
{
    Task SetPositionAsync(string serialNumber, float latitude, float longitude);
    Task<DeviceModel> GetDeviceAsync(string serialNumber);
    IAsyncEnumerable<DeviceModel> GetDevicesAsync(CancellationToken ct);
    Task SetOnlineState(string serialNumber, bool state);
    Task CreateDeviceAsync(string serialNumber);
}

public class DeviceDomainService(
        IDeviceRepository repository,
        ILogger<DeviceDomainService> logger,
        IClock clock) : IDeviceDomainService
{

    public async Task SetPositionAsync(string serialNumber, float latitude, float longitude)
    {
        ValidateSerialNumber(serialNumber);
        var device = await repository.GetDeviceAsync(serialNumber);

        device.Latitude = latitude;
        device.Longitude = longitude;

        await repository.SetDeviceAsync(device);
    }

    public async Task CreateDeviceAsync(string serialNumber)
    {
        ValidateSerialNumber(serialNumber);
        try
        {
            if(await repository.DeviceExists(serialNumber))
            {
                throw new System.Exception("Device already exists");
            }

            await repository.CreateDeviceAsync(new DeviceModel() 
            { 
                IsOnline = false,
                SerialNumber = serialNumber,
                LastPowerChange = clock.GetUtcNow()
            });
        }
        catch (System.Exception ex)
        {
            logger.LogError(ex.Message);
            throw new DeviceCannotBeCreatedException(serialNumber);
        }
    }

    public Task<DeviceModel> GetDeviceAsync(string serialNumber)
    {
        ValidateSerialNumber(serialNumber);
        return repository.GetDeviceAsync(serialNumber);
    }

    public IAsyncEnumerable<DeviceModel> GetDevicesAsync(CancellationToken ct) =>
        repository.GetDevicesAsync(ct);

    public async Task SetOnlineState(string serialNumber, bool state)
    {
        ValidateSerialNumber(serialNumber);

        var device = await repository.GetDeviceAsync(serialNumber);

        if(device.IsOnline != state)
        {
            device.IsOnline = state;
            device.LastPowerChange = clock.GetUtcNow();

            await repository.SetDeviceAsync(device);

            logger.LogInformation("Device {_sn} is now {_state}.", serialNumber, state ? "online" : "offline");
        }
        else
        {
            logger.LogInformation("Device {_sn} was already {_state}.", serialNumber, state ? "online" : "offline");
        }
    }

    private void ValidateSerialNumber(string serialNumber)
    {
        if(serialNumber.Length != 12)
        {
            throw new DeviceSerialNumberInvalidException(serialNumber);
        }
    }
}
