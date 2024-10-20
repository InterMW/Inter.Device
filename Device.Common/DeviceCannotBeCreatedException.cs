namespace Device.Common;

public class DeviceCannotBeCreatedException : Exception
{
    public DeviceCannotBeCreatedException(string? message) : base(message) { }
}
