
using Microsoft.Extensions.DependencyInjection;

namespace Device.Grpc;

public static class DeviceGrpcDependencyModule
{
    public static void RegisterClient(IServiceCollection services)
    {
        services.AddTransient<IDeviceGrpcClient, DeviceGrpcClient>();
    }
}
