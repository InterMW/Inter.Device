using System.Runtime.CompilerServices;
using Couchbase.KeyValue.RangeScan;
using Device.Domain;
using Infrastructure.RepositoryCore;
using MelbergFramework.Infrastructure.Couchbase;

namespace Infrastructure.Couchbase;

public class DeviceRepository : BaseRepository, IDeviceRepository
{
    public DeviceRepository(string name, IBucketFactory factory) : base(name, factory)
    {
    }

    public async IAsyncEnumerable<DeviceModel> GetDevicesAsync([EnumeratorCancellation] CancellationToken ct)
    {
        await foreach (var device in Collection.ScanAsync(new RangeScan()))
        {
            yield return device.ContentAs<DeviceModel>();
        };
    }

    public async Task<DeviceModel> GetDeviceAsync(string serialNumber) =>
        (await Collection.GetAsync(serialNumber)).ContentAs<DeviceModel>();

    public Task SetDeviceAsync(DeviceModel model) =>
        Collection.UpsertAsync(model.SerialNumber, model);

    public Task CreateDeviceAsync(DeviceModel model) =>
        Collection.InsertAsync(model.SerialNumber, model);
}
