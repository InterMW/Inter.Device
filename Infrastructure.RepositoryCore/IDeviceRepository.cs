using Device.Domain;

namespace Infrastructure.RepositoryCore;

public interface IDeviceRepository
{
    Task CreateDeviceAsync(DeviceModel model);
    Task SetDeviceAsync(DeviceModel model);
    Task<bool> DeviceExists(string serialNumber);
    Task<DeviceModel> GetDeviceAsync(string serialNumber);
    IAsyncEnumerable<DeviceModel> GetDevicesAsync(CancellationToken ct);
}
