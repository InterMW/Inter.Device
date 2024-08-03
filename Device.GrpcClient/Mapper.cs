using Device.Domain;

namespace Device.GrpcClient;

public static class DeviceMapper
{
    public static DeviceDto ToDto(this DeviceModel model) =>
        new()
        {
            Serial = model.SerialNumber,
            IsOnline = model.IsOnline
        };

    public static DeviceModel ToModel(this DeviceDto dto) =>
        new()
        {
            SerialNumber = dto.Serial,
             IsOnline = dto.IsOnline
        };
}
