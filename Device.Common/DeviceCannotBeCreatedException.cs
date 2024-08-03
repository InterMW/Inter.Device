namespace Device.Common;

public class DeviceCannotBeCreatedException : Exception
{
    public const string Name = "DeviceCannotBeCreatedException";
    public DeviceCannotBeCreatedException(string? message) : base(message) { }
}
