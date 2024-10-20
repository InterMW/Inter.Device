using System.Runtime.CompilerServices;
using Device.Domain;
using Infrastructure.RepositoryCore;
using MelbergFramework.Infrastructure.Couchbase;

namespace Infrastructure.Couchbase;

public class DeviceRepository : BaseRepository, IDeviceRepository
{
    public DeviceRepository(IBucketFactory factory) : base("device", factory){}

    public async IAsyncEnumerable<DeviceModel> GetDevicesAsync([EnumeratorCancellation] CancellationToken ct)
    {
        //This makes me very sad but I am tired and I don't want to have to write another package so I'll leave this as a todo
        //Using Collection.Scan is insanely unperformant (takes 10 seconds to return 10 items) 
        var quer = await Collection.Scope.Bucket.Cluster.QueryAsync<DeviceModel>("SELECT a.* from device._default.device as a");

        await foreach (var device in quer)
        {
            yield return device;
        };
    }

    public async Task<DeviceModel> GetDeviceAsync(string serialNumber) =>
        (await Collection.GetAsync(serialNumber)).ContentAs<DeviceModel>();

    public Task SetDeviceAsync(DeviceModel model) =>
        Collection.UpsertAsync(model.SerialNumber, model);

    public Task CreateDeviceAsync(DeviceModel model) =>
        Collection.InsertAsync(model.SerialNumber, model);
}
