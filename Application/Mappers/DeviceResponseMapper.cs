using Application.DataModels;
using Device.Domain;

namespace Application.Mappers;

public static class DeviceResponseMapper
{
    public static DeviceResponse ToResponse(this DeviceModel model) => new()
    {
        IsOnline = model.IsOnline,
        SerialNumber = model.SerialNumber,
        FirstHeardFrom = model.FirstHeardFrom,
        LastPowerChange = model.LastPowerChange
    };
}
