using DomainService;
using Microsoft.AspNetCore.Mvc;

namespace Application.Controllers;

[ApiController]
[Route("device")]
public class DeviceController 
{
    private readonly IDeviceDomainService _domainService;

    public DeviceController(IDeviceDomainService domainService)
    {
        _domainService = domainService;
    }

    [HttpGet]
    [Route("device/{serial}")]
    public Task GetDevice([FromRoute] string serial) =>
        _domainService.GetDeviceAsync(serial);

    [HttpGet]
    [Route("device/list")]
    public async  Task<string[]> GetDevices(CancellationToken ct) 
    {
        return await _domainService.GetDevicesAsync(ct)
                    .Select(_ => _.SerialNumber)
                    .ToArrayAsync();
    }
}
