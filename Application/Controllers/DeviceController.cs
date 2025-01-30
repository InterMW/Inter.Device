using Application.DataModels;
using Application.Mappers;
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
    public async Task<DeviceResponse> GetDevice([FromRoute] string serial)
    {
        var device = await domainService.GetDeviceAsync(serial);
        return device.ToResponse();
    }

    [HttpGet]
    [Route("list")]
    public async Task<DeviceResponse[]> GetDevices(CancellationToken ct) =>
        await domainService
                .GetDevicesAsync(ct)
                .Select(DeviceResponseMapper.ToResponse)
                .ToArrayAsync();
}
