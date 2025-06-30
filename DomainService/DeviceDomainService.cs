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
    Task<int> RegisterDeviceAsync(string serialNumber, string ipAddress);
}

public class DeviceDomainService : IDeviceDomainService
{
    private readonly IClock _clock;

    private readonly IDeviceRepository _repository;

    private readonly ILogger<DeviceDomainService> _logger;

    private readonly IIpRepository _ipRepository;

    public DeviceDomainService(
        IDeviceRepository repository,
        ILogger<DeviceDomainService> logger,
        IIpRepository ipRepository,
        IClock clock)
    {
        _logger = logger;
        _repository = repository;
        _clock = clock;
        _ipRepository = ipRepository;
    }

    public async Task SetPositionAsync(string serialNumber, float latitude, float longitude)
    {
        ValidateSerialNumber(serialNumber);
        var device = await _repository.GetDeviceAsync(serialNumber);

        device.Latitude = latitude;
        device.Longitude = longitude;

        await _repository.SetDeviceAsync(device);
    }

    public async Task<int> RegisterDeviceAsync(string serialNumber, string ipAddress)
    {
        ValidateSerialNumber(serialNumber);
        _logger.LogInformation("Registering {device} at {ip}", serialNumber, ipAddress);
        try
        {
            DeviceModel device;
            if (await _repository.DeviceExists(serialNumber))
            {
                device = await _repository.GetDeviceAsync(serialNumber);
                device.IsOnline = true;
            }
            else
            {
                device = new()
                {
                    SerialNumber = serialNumber,
                    IsOnline = true,
                    FirstHeardFrom = _clock.GetUtcNow(),
                    LastPowerChange = _clock.GetUtcNow(),
                };

                await _repository.CreateDeviceAsync(device);
            }

            if (device.IpAddress != ipAddress)
            {
                device.IpAddress = ipAddress;

                var info = await _ipRepository.Lookup(ipAddress);

                if (info is null)
                {
                    _logger.LogInformation("The ip address {ipAddress} for device {serialNumber} is somehow invalid");
                    device.Latitude = 0;
                    device.Longitude = 0;
                }

                device.Latitude = (float)info!.Latitude!;
                device.Longitude = (float)info!.Longitude!;
            }

            if (device.Port == 0)
            {
                device.Port = await GetNextPort();
            }

            await _repository.SetDeviceAsync(device);
            _logger.LogInformation("Registering {device} at {ip}:{port}", serialNumber, ipAddress, device.Port);

            return device.Port;
        }
        catch (System.Exception ex)
        {
            _logger.LogError(ex.Message);
            throw new DeviceCannotBeCreatedException(serialNumber);
        }
    }

    private async Task<int> GetNextPort()
    {
        var devices = _repository.GetDevicesAsync(CancellationToken.None);

        int offset = 8000;
        await foreach (var otherDevice in devices)
        {
            offset = Math.Max(offset, otherDevice.Port);

        }

        return offset + 1;
    }

    public async Task CreateDeviceAsync(string serialNumber)
    {
        ValidateSerialNumber(serialNumber);
        try
        {
            if (await _repository.DeviceExists(serialNumber))
            {
                throw new System.Exception("Device already exists");
            }

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

        if (device.IsOnline != state)
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
        if (serialNumber.Length != 12)
        {
            throw new DeviceSerialNumberInvalidException(serialNumber);
        }
    }
}
