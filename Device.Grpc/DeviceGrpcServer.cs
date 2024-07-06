using Device.Domain;
using DomainService;
using Grpc.Core;

namespace Device.Grpc;

public class DeviceGrpcServer : DeviceService.DeviceServiceBase
{
    private readonly IDeviceDomainService _domainService;

    public DeviceGrpcServer(IDeviceDomainService domainService)
    {
        _domainService = domainService;
    }

    public override async Task<DeviceDto> GetDevice(DeviceRequestMessage request, ServerCallContext context)
    {
        var result = await _domainService.GetDeviceAsync(request.Serial);
        return result.ToDto();
    }

    public override async Task GetDevices(Empty request, IServerStreamWriter<DeviceDto> responseStream, ServerCallContext context)
    {
        for(var i = 0; i < 100; i++)
        {
            await responseStream.WriteAsync(new DeviceModel(){SerialNumber = $"SN{i}", IsOnline = (i % 2) == 0}.ToDto());
        }
    }


    public override async Task<Empty> SetAliveState(DeviceStateMessage request, ServerCallContext context)
    {
        await _domainService.SetOnlineState(request.Serial,
                request.IsOnline);

        return new Empty();
    }

    public override async Task<DeviceDto> CreateDevice(DeviceCreateMessage request, ServerCallContext context)
    {
        await _domainService.CreateDeviceAsync(request.Serial);

        retur
    }
    // The following are defined becuase I use the "generate overrides"
    // function to fill out the generated stuff
    public override string? ToString() => base.ToString();

    public override bool Equals(object? obj) => base.Equals(obj);
    
    public override int GetHashCode() => base.GetHashCode();

}
