using System.Runtime.CompilerServices;
using Device.Common;
using Device.Domain;
using Google.Rpc;
using Grpc.Core;
using Grpc.Net.Client;
using Microsoft.Extensions.Configuration;
using static Device.DeviceService;

namespace Device.GrpcClient;

public interface IDeviceGrpcClient
{
    IAsyncEnumerable<DeviceModel> GetDevicesAsync(CancellationToken ct);
    Task CreateDeviceAsync(string serialNumber);
    Task SetDeviceLifeState(string serialNumber, bool isAlive);
    Task<DeviceModel> GetDeviceAsync(string serialNumber);
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

    public async IAsyncEnumerable<DeviceModel> GetDevicesAsync([EnumeratorCancellation] CancellationToken ct)
    {
        var result = _service.GetDevices(new DeviceQueryMessage());

        while (true)
        {
            DeviceModel current;
            try
            {
                var next = await result.ResponseStream.MoveNext(ct);
                if (!next)
                {
                    break;
                }
                current = result.ResponseStream.Current.ToModel();
            }
            catch (Exception ex)
            {
                throw Exceptor(ex);
            }
            yield return current;
        }
    }

    public async Task SetDeviceLifeState(string serialNumber, bool isAlive)
    {
        try
        {
            await _service.SetAliveStateAsync( new DeviceStateMessage(){ Serial = serialNumber, IsOnline = isAlive});
        }
        catch (Exception ex)
        {
            throw Exceptor(ex);
        }
    }

    public async Task<DeviceModel> GetDeviceAsync(string serialNumber)
    {
        try
        {
            var result = await _service.GetDeviceAsync(new DeviceRequestMessage() { Serial = serialNumber });
            return result.ToModel();
        }
        catch (Exception ex)
        {
            throw Exceptor(ex);
        }
    }

    public async Task CreateDeviceAsync(string serialNumber)
    {
        try
        {
            await _service.CreateDeviceAsync(new DeviceCreateMessage() { Serial = serialNumber });
        }
        catch (Exception ex)
        {
            throw Exceptor(ex);
        }
    }

    private Exception Exceptor(Exception ex)
    {
        if (ex is RpcException)
        {
            var rich = ((RpcException)ex).GetRpcStatus();
            if(rich.Code == 999)
            {
                var richException = rich?.GetDetail<ErrorInfo>();

                if (richException != null)
                {
                    switch (richException.Domain)
                    {
                        case DeviceSerialNumberInvalidException.Name:
                            return new DeviceSerialNumberInvalidException(richException.Reason);
                        case DeviceCannotBeCreatedException.Name:
                            return new DeviceCannotBeCreatedException(richException.Reason);
                    }
                }
                return new Exception("Undefined error occurred.");
            }
        }

        return ex;
    }
}
