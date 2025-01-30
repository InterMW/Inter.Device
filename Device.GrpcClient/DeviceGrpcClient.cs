using System.Runtime.CompilerServices;
using Device.Common;
using Device.Domain;
using Device.GrpcCommon;
using Google.Rpc;
using Grpc.Core;
using Grpc.Net.Client;
using Microsoft.Extensions.Configuration;

namespace Device.GrpcClient;

public interface IDeviceGrpcClient
{
    IAsyncEnumerable<DeviceModel> GetDevicesAsync(CancellationToken ct);
    Task CreateDeviceAsync(string serialNumber);
    Task SetDeviceLifeState(string serialNumber, bool isAlive);
    Task<DeviceModel> GetDeviceAsync(string serialNumber);
    Task SetDeviceLocation(string serialNumber, float latitude, float longitude);
}

public class DeviceGrpcClient : IDeviceGrpcClient
{
    private readonly DeviceGrpcServiceClient _service;
    private readonly GrpcChannel _channel;

    public DeviceGrpcClient(IConfiguration configuration)
    {
        var uri = configuration.GetConnectionString("DevicesGrpc") ?? throw new Exception("Grpc Uri missing for DeviceGrpc");

        _channel = GrpcChannel.ForAddress(uri);

        _service = new DeviceGrpcServiceClient(_channel);
    }
    
    public async Task SetDeviceLocation(string serialNumber, float latitude, float longitude)
    {
        try
        {
            await _service.SetPositionAsync(new DevicePositionMessage(){Serial = serialNumber, Latitude = latitude, Longitude = longitude});
        }
        catch (Exception ex)
        {
            throw Exceptor(ex);
        }
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
        var richException = ex as RpcException;
        if (richException is not null)
        {

            var richExceptionStatus = richException.GetRpcStatus();
            if(richExceptionStatus?.Code == 999)
            {
                var richExceptionDetail = richExceptionStatus.GetDetail<ErrorInfo>();

                if (richExceptionDetail is not null)
                {
                    switch (richExceptionDetail.Domain)
                    {
                        case nameof(DeviceSerialNumberInvalidException):
                            return new DeviceSerialNumberInvalidException(richExceptionDetail.Reason);
                        case nameof(DeviceCannotBeCreatedException):
                            return new DeviceCannotBeCreatedException(richExceptionDetail.Reason);
                        case nameof(DeviceNotFoundException):
                            return new DeviceNotFoundException(richExceptionDetail.Reason);
                    }
                }

                return new Exception("Undefined error occurred.");
            }
        }

        return ex;
    }
}
