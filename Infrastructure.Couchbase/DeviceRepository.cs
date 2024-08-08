using System.Diagnostics;
using System.Runtime.CompilerServices;
using Couchbase.KeyValue.RangeScan;
using Device.Domain;
using Infrastructure.RepositoryCore;
using MelbergFramework.Infrastructure.Couchbase;

namespace Infrastructure.Couchbase;

public class DeviceRepository : BaseRepository, IDeviceRepository
{
    public DeviceRepository(IBucketFactory factory) : base("device", factory){}

    public async IAsyncEnumerable<DeviceModel> GetDevicesAsync([EnumeratorCancellation] CancellationToken ct)
    {
        var stop = new Stopwatch();
        stop.Start();
        var quer = Collection.ScanAsync(new RangeScan());
        Console.WriteLine(stop.ElapsedMilliseconds);
        await foreach (var device in quer)
        {
            var j = device.ContentAs<DeviceModel>();
            Console.WriteLine(stop.ElapsedMilliseconds);
            yield return j;
        };
    }

    public async Task<DeviceModel> GetDeviceAsync(string serialNumber) =>
        (await Collection.GetAsync(serialNumber)).ContentAs<DeviceModel>();

    public Task SetDeviceAsync(DeviceModel model) =>
        Collection.UpsertAsync(model.SerialNumber, model);

    public Task CreateDeviceAsync(DeviceModel model) =>
        Collection.InsertAsync(model.SerialNumber, model);
}
