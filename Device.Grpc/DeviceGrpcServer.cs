using Device.GrpcCommon;
using DomainService;
using Grpc.Core;

namespace Device.Grpc;

public class DeviceGrpcServer : DeviceServiceBaseCommon
{
    private readonly IDeviceDomainService _domainService;

    public DeviceGrpcServer(IDeviceDomainService domainService)
    {
        _domainService = domainService;
    }

    public override async Task<Empty> SetPosition(DevicePositionMessage request, ServerCallContext context)
    {
        await _domainService.SetPositionAsync(request.Serial, request.Latitude, request.Longitude);

        return new Empty();
    }

    public override async Task<Empty> CreateDevice(DeviceCreateMessage request, ServerCallContext context)
    {
        //await _domainService.CreateDeviceAsync(request.Serial);
        await Task.CompletedTask;

        return new Empty();
    }

    public override async Task<DeviceDto> GetDevice(DeviceRequestMessage request, ServerCallContext context)
    {
        var result = await _domainService.GetDeviceAsync(request.Serial);
        return result.ToDto();
    }

    public override async Task GetDevices(DeviceQueryMessage request, IServerStreamWriter<DeviceDto> responseStream, ServerCallContext context)
    {
        await foreach (var device in _domainService.GetDevicesAsync(context.CancellationToken))
        {
            await responseStream.WriteAsync(device.ToDto());
        }
    }

    public override async Task<Empty> SetAliveState(DeviceStateMessage request, ServerCallContext context)
    {
        await _domainService.SetOnlineState(request.Serial, request.IsOnline);

        return new Empty();
    }


    // The following are defined becuase I use the "generate overrides"
    // function to fill out the generated stuff
    public override string? ToString() => base.ToString();

    public override int GetHashCode() => base.GetHashCode();
}
