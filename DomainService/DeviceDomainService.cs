using Device.Common;
using Device.Domain;
using Infrastructure.RepositoryCore;
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

    public DeviceDomainService(IDeviceRepository repository, ILogger<DeviceDomainService> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task CreateDeviceAsync(string serialNumber)
    {
        ValidateSerialNumber(serialNumber);
        try
        {
            await _repository.CreateDeviceAsync(new DeviceModel() { IsOnline = false, SerialNumber = serialNumber });
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
        var device = await _repository.GetDeviceAsync(serialNumber);
        //handle errors and such

        device.IsOnline = state;

        await _repository.SetDeviceAsync(device);
    }

    private void ValidateSerialNumber(string serialNumber)
    {
        if(serialNumber.Length != 12)
        {
            throw new DeviceSerialNumberInvalidException(serialNumber);
        }
    }
}
