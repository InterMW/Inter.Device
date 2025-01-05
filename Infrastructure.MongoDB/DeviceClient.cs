using Microsoft.Extensions.Options;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Driver;

namespace Infrastructure.MongoDB;

public class DeviceClient
{
    private readonly MongoClient _client;
    public DeviceClient(IOptions<MongoDBOptions> options)
    {
        _client = new MongoClient(options.Value.PublicDeviceRepo);

        var camelCaseConvention = new ConventionPack { new CamelCaseElementNameConvention() };
        ConventionRegistry.Register("CamelCase", camelCaseConvention, type => true);

        try
        {
            _client.GetDatabase("device").CreateCollectionAsync("standard").Wait();
        }
        catch (System.Exception)
        {
        }
        
    }
    public MongoClient GetClient() => _client;
}


