using IpData;
using IpData.Models;
using Microsoft.Extensions.Options;
using System.ComponentModel.DataAnnotations;

namespace Infrastructure.RepositoryCore;

public interface IIpRepository
{
    public Task<IpInfo?> Lookup(string ipAddress);
}

public class IpRepository : IIpRepository
{
    private readonly IpDataClient _client;

    public IpRepository(IOptions<IpOptions> options)
    {
        _client = new(options.Value.Key);
    }

    public async Task<IpInfo?> Lookup(string ipAddress) => await _client.Lookup(ipAddress);
}

public class IpOptions
{
    public const string Section = "IpKey";

    [Required]
    public string Key {get; set;} = "";
}
