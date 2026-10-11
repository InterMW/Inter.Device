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
        LastHeardFrom = model.LastHeardFrom,
        LastPowerChange = model.LastPowerChange,
        Latitude = model.Latitude,
        Longitude = model.Longitude,
        IpAddress = model.IpAddress,
    };
}
