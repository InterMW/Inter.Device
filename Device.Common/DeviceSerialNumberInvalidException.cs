namespace Device.Common;

public class DeviceSerialNumberInvalidException : Exception
{
    public const string Name = "DeviceSerialNumberInvalidException";
    public DeviceSerialNumberInvalidException(string? message) : base(message)
    {
    }
}
