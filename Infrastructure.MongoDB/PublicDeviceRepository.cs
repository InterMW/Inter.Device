using Device.Domain;
using Device.Common;
using Infrastructure.RepositoryCore;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;

namespace Infrastructure.MongoDB;

public class PublicDeviceRepository(DeviceClient client) : IDeviceRepository
{

    private readonly IMongoCollection<PublicDeviceModel> _standardCollection
        = client.GetClient().GetDatabase("device").GetCollection<PublicDeviceModel>("standard");

    public Task CreateDeviceAsync(DeviceModel model) =>
        _standardCollection.InsertOneAsync(model.ToDto());

    public async Task<DeviceModel> GetDeviceAsync(string serialNumber)
    {
        var result = await _standardCollection
            .FindAsync(SerialNumberFilter(serialNumber));
        return (await result.FirstOrDefaultAsync())?.ToModel() ?? throw new DeviceNotFoundException(serialNumber);
    }

    public async IAsyncEnumerable<DeviceModel> GetDevicesAsync(CancellationToken ct)
    {
        foreach(var device in _standardCollection.AsQueryable())
        {
            await Task.CompletedTask;
            if(ct.IsCancellationRequested)
                yield break;

            yield return device.ToModel();
        }
    }

    public Task SetDeviceAsync(DeviceModel model) =>
        _standardCollection
        .ReplaceOneAsync(SerialNumberFilter(model.SerialNumber),model.ToDto());

    private static FilterDefinition<PublicDeviceModel> SerialNumberFilter(string serialNumber) => Builders<PublicDeviceModel>.Filter.Eq(_ => _.SerialNumber, serialNumber);

    public async Task<bool> DeviceExists(string serialNumber)
    {
        return (await _standardCollection.FindAsync(SerialNumberFilter(serialNumber))).Any();
    }

    private static ReplaceOptions options = new ()
        {
            BypassDocumentValidation = true
        };
}

public static class PublicDeviceModelMapper
{
    public static PublicDeviceModel ToDto(this DeviceModel model) => new()
    {
        SerialNumber = model.SerialNumber,
        IsOnline = model.IsOnline,
        FirstHeardFrom = model.FirstHeardFrom,
        LastPowerChange = model.LastPowerChange,
        Latitude = model.Latitude,
        Longitude = model.Longitude
    };

    public static DeviceModel ToModel(this PublicDeviceModel model) => new()
    {
        SerialNumber = model.SerialNumber,
        IsOnline = model.IsOnline,
        FirstHeardFrom = model.FirstHeardFrom,
        LastPowerChange = model.LastPowerChange,
        Latitude = model.Latitude,
        Longitude = model.Longitude
    };
}

public class PublicDeviceModel 
{
    [BsonIgnoreIfDefault]
    public ObjectId Id { get; set; }
    public string SerialNumber { get; set; } = "";
    public bool IsOnline { get; set; }
    public DateTime FirstHeardFrom {get; set;} = DateTime.UtcNow;
    public DateTime LastPowerChange {get; set;} = DateTime.MinValue;
    public float Latitude {get; set;}
    public float Longitude {get; set;}
}
