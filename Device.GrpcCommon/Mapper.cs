using Device.Domain;

namespace Device.GrpcCommon;

public static class DeviceMapper
{
    public static DeviceDto ToDto(this DeviceModel model) =>
        new()
        {
            Serial = model.SerialNumber,
            IsOnline = model.IsOnline,
            LastStateChange = Google.Protobuf.WellKnownTypes.TimeExtensions.ToTimestamp(model.LastPowerChange),
            FirstHeardFrom = Google.Protobuf.WellKnownTypes.TimeExtensions.ToTimestamp(model.FirstHeardFrom),
        };

    public static DeviceModel ToModel(this DeviceDto dto) =>
        new()
        {
            SerialNumber = dto.Serial,
            IsOnline = dto.IsOnline,
            FirstHeardFrom = dto.FirstHeardFrom.ToDateTime(),
            LastPowerChange = dto.LastStateChange.ToDateTime()
        };
}
