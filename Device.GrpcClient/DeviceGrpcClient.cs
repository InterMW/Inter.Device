using System.Runtime.CompilerServices;
using Device.Domain;
using Google.Rpc;
using Grpc.Core;
using Grpc.Core.Interceptors;
using Grpc.Net.Client;
using Microsoft.Extensions.Configuration;
using static Device.DeviceService;

namespace Device.GrpcClient;

public interface IDeviceGrpcClient
{
    IAsyncEnumerable<DeviceModel> GetDevicesAsync(CancellationToken ct);
    Task CreateDeviceAsync(string serialNumber);
    Task<DeviceModel> GetDeviceAsync(string serialNumber);
}

public class DeviceGrpcClient : IDeviceGrpcClient
{
    private readonly DeviceServiceClient _service;
    private readonly GrpcChannel _channel;

    public DeviceGrpcClient(IConfiguration configuration)
    {
        var uri = configuration.GetConnectionString("DevicesGrpc") ?? throw new Exception("Grpc Uri missing for DeviceGrpc");

        var channel = GrpcChannel.ForAddress(uri);

        var _channel = channel.Intercept(new ClientExceptionInterceptor());

        _service = new DeviceServiceClient(_channel);
    }

    public async IAsyncEnumerable<DeviceModel> GetDevicesAsync([EnumeratorCancellation] CancellationToken ct)
    {
        var result = _service.GetDevices(new DeviceQueryMessage());

        while (true)
        {
            try
            {

                var next = await result.ResponseStream.MoveNext(ct);
                if (!next)
                {
                    break;
                }
            }
            catch (Exception ex)
            {
                throw Exceptor(ex);
            }
            yield return result.ResponseStream.Current.ToModel();
        }
    }

    public async Task<DeviceModel> GetDeviceAsync(string serialNumber)
    {
        var result = await _service.GetDeviceAsync(new DeviceRequestMessage() { Serial = serialNumber });
        return result.ToModel();
    }

    public async Task CreateDeviceAsync(string serialNumber)
    {
        await _service.CreateDeviceAsync(new DeviceCreateMessage() { Serial = serialNumber });
    }

    private Exception Exceptor(Exception ex)
    {
        if (ex is RpcException)
        {
            var richException = ((RpcException)ex).GetRpcStatus()?.GetDetail<ErrorInfo>();

            if (richException != null)
            {
                Type type = Type.GetType(richException.Domain);

                if (type != null)
                {
                    return (Exception)Activator.CreateInstance(type, richException.Reason) ?? new Exception(ex.Message);
                }
            }
            return new Exception(ex.Message);
        }

        return ex;
    }
}
