using Device.Common;
using Device.Domain;
using Infrastructure.RepositoryCore;
using MelbergFramework.Core.Time;
using Microsoft.Extensions.Logging;

namespace DomainService;

public interface IDeviceDomainService
{
    Task<DeviceModel> GetDeviceAsync(string serialNumber);
    IAsyncEnumerable<DeviceModel> GetDevicesAsync(CancellationToken ct);
    Task SetOnlineState(string serialNumber, bool state);
    Task CreateDeviceAsync(string serialNumber);
}

public class DeviceDomainService : IDeviceDomainService
{
    private readonly IDeviceRepository _repository;
    private readonly ILogger<DeviceDomainService> _logger;
    private readonly IClock _clock;

    public DeviceDomainService(
        IDeviceRepository repository,
        IClock clock,
        ILogger<DeviceDomainService> logger
        )
    {
        _repository = repository;
        _logger = logger;
        _clock = clock;
    }

    public async Task CreateDeviceAsync(string serialNumber)
    {
        ValidateSerialNumber(serialNumber);
        try
        {
            await _repository.CreateDeviceAsync(new DeviceModel() 
            { 
                IsOnline = false,
                SerialNumber = serialNumber,
                LastPowerChange = _clock.GetUtcNow()
            });
        }
        catch (System.Exception ex)
        {
            _logger.LogError(ex.Message);
            throw new DeviceCannotBeCreatedException(serialNumber);
        }
    }

    public Task<DeviceModel> GetDeviceAsync(string serialNumber)
    {
        ValidateSerialNumber(serialNumber);
        return _repository.GetDeviceAsync(serialNumber);
    }

    public IAsyncEnumerable<DeviceModel> GetDevicesAsync(CancellationToken ct) =>
        _repository.GetDevicesAsync(ct);

    public async Task SetOnlineState(string serialNumber, bool state)
    {
        ValidateSerialNumber(serialNumber);

        var device = await _repository.GetDeviceAsync(serialNumber);

        if(device.IsOnline != state)
        {
            device.IsOnline = state;
            device.LastPowerChange = _clock.GetUtcNow();

            await _repository.SetDeviceAsync(device);

            _logger.LogInformation("Device {_sn} is now {_state}.", serialNumber, state ? "online" : "offline");
        }
        else
        {
            _logger.LogInformation("Device {_sn} was already {_state}.", serialNumber, state ? "online" : "offline");
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
