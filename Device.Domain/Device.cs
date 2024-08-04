namespace Device.Domain;

public class DeviceModel
{
    public string SerialNumber {get; set;} 
    public bool IsOnline {get; set;}
    public DateTime LastPowerChange {get; set;} = DateTime.MinValue;
}
