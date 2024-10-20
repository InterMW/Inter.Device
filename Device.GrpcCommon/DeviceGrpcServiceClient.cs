using Grpc.Core;
using static Device.DeviceService;

namespace Device.GrpcCommon;

public class DeviceGrpcServiceClient : DeviceServiceClient
{
    public DeviceGrpcServiceClient(ChannelBase channel) : base(channel) { }
}
