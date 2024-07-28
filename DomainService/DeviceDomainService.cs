using Device.Common;
using Device.Domain;
using Infrastructure.RepositoryCore;

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

    public DeviceDomainService(IDeviceRepository repository)
    {
        _repository = repository;
    }

    public async Task CreateDeviceAsync(string serialNumber)
    {
        ValidateSerialNumber(serialNumber);
        try
        {
            await _repository.CreateDeviceAsync(new DeviceModel() { IsOnline = false, SerialNumber = serialNumber });
        }
        catch (System.Exception)
        {
            throw new DeviceCannotBeCreatedException();
        }
    }

    public Task<DeviceModel> GetDeviceAsync(string serialNumber)
    {
        if(serialNumber.Length != 12)
        {
            throw new DeviceSerialNumberInvalidException();
        }
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
            throw new DeviceSerialNumberInvalidException();
        }
    }
}
