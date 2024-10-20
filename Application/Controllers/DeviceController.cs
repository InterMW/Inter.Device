using Device.Domain;
using DomainService;
using Microsoft.AspNetCore.Mvc;

namespace Application.Controllers;

[ApiController]
[Route("device")]
public 
class DeviceController(IDeviceDomainService domainService)
{
    [HttpGet]
    [Route("{serial}")]
    public Task<DeviceModel> GetDevice([FromRoute] string serial) =>
        domainService.GetDeviceAsync(serial);

    [HttpGet]
    [Route("list")]
    public async Task<string[]> GetDevices(CancellationToken ct) 
    {
        return await domainService.GetDevicesAsync(ct)
                    .Select(_ => _.SerialNumber)
                    .ToArrayAsync();
    }
}
