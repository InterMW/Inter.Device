using Device.Domain;
using Grpc.Core;

namespace Device.Grpc;

public class DeviceGrpcServer : DeviceService.DeviceServiceBase
{
    public override async Task GetDevices(Empty request, IServerStreamWriter<DeviceDto> responseStream, ServerCallContext context)
    {
        for(var i = 0; i < 100; i++)
        {
            await responseStream.WriteAsync(new DeviceModel(){SerialNumber = $"SN{i}", IsOnline = (i % 2) == 0}.ToDto());
        }
    }

    public override Task<Empty> SetAliveState(DeviceStateMessage request, ServerCallContext context)
    {
        return base.SetAliveState(request, context);
    }
}
