using IPData;
using IPData.Models;
using Microsoft.Extensions.Options;
using System.ComponentModel.DataAnnotations;

namespace Infrastructure.RepositoryCore;

public interface IIpRepository
{
    public Task<IPLookupResult?> Lookup(string ipAddress);
}

public class IpRepository : IIpRepository
{
    private readonly IPDataClient _client;

    public IpRepository(IOptions<IpOptions> options)
    {
        _client = new(options.Value.Key);
    }

    public async Task<IPLookupResult?> Lookup(string ipAddress) => await _client.Lookup(ipAddress);
}

public class IpOptions
{
    public const string Section = "IpKey";

    [Required]
    public string Key {get; set;} = "";
}
