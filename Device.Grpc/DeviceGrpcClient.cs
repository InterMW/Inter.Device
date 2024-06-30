using System.Runtime.CompilerServices;
using Device.Domain;
using Grpc.Net.Client;
using Microsoft.Extensions.Configuration;
using static Device.DeviceService;

namespace Device.Grpc;

public interface IDeviceGrpcClient
{
    IAsyncEnumerable<DeviceModel> GetDevicesAsync(CancellationToken ct);
}

public class DeviceGrpcClient : IDeviceGrpcClient
{
    private readonly DeviceServiceClient _service;
    private readonly GrpcChannel _channel;

    public DeviceGrpcClient(IConfiguration configuration)
    {
        var uri = configuration.GetConnectionString("DevicesGrpc") ?? throw new Exception("Grpc Uri missing for DeviceGrpc");

        _channel = GrpcChannel.ForAddress(uri);

        _service = new DeviceServiceClient(_channel);
    }

    public async IAsyncEnumerable<DeviceModel> GetDevicesAsync( [EnumeratorCancellation] CancellationToken ct)
    {
        var result = _service.GetDevices(new Empty());
        while(await result.ResponseStream.MoveNext(ct))
        {
            yield return result.ResponseStream.Current.ToModel();
        }
    }
}
