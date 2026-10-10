using Application.DataModels;
using Application.Mappers;
using Authorizer.GrpcClient;
using DomainService;
using Microsoft.AspNetCore.Mvc;

namespace Application.Controllers;

[ApiController]
[Route("device")]
public class DeviceController(
    IDeviceDomainService domainService
    )
{
    [HttpPost]
    [InterAuthorizer]
    [Route("restart/{serial}")]
    public async Task<bool> GetDevice([FromRoute] string serial)
    {
        Console.WriteLine("Restarting ");
        await domainService.RestartDevice(serial);
        return true;
    }

    [HttpGet]
    [InterAuthorizer]
    [Route("list")]
    public async Task<DeviceResponse[]> GetDevices(CancellationToken ct)  =>
        await domainService
                .GetDevicesAsync(ct)
                .Select(DeviceResponseMapper.ToResponse)
                .ToArrayAsync();
        //     var mess = new MqttApplicationMessageBuilder();
        //             mess.WithPayload("restart");
        //             mess.WithTopic("todevice/30EDA0E2549E");
        //             var res = await mqttClient.PublishAsync(mess.Build());

 //   [HttpPost]
 //   [Route("register")]
 //   public async Task<int> Register(DeviceRegistrationRequest request, CancellationToken ct) =>
 //       await domainService.RegisterDeviceAsync(request.SerialNumber, request.IPAddress);
}
