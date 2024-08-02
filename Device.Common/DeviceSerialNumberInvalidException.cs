namespace Device.Common;

public class DeviceSerialNumberInvalidException : Exception
{
    public DeviceSerialNumberInvalidException(string? message) : base(message)
    {
    }
}
