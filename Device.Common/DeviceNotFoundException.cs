namespace Device.Common;

public class DeviceNotFoundException : Exception
{
    public DeviceNotFoundException(string? message) : base(message) { }
}
