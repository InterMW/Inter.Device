using System.Runtime.CompilerServices;
using Device.Domain;

namespace Infrastructure.RepositoryCore;

public interface IDeviceRepository
{
    Task CreateDeviceAsync(DeviceModel model);
    Task SetDeviceAsync(DeviceModel model);
    Task<DeviceModel> GetDeviceAsync(string serialNumber);
    IAsyncEnumerable<DeviceModel> GetDevicesAsync([EnumeratorCancellation] CancellationToken ct);
}
