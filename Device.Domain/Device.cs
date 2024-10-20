namespace Device.Domain;

public class DeviceModel
{
    public string SerialNumber {get; set;} 
    public bool IsOnline {get; set;}
    public DateTime FirstHeardFrom {get; set;} = DateTime.UtcNow;
    public DateTime LastPowerChange {get; set;} = DateTime.MinValue;
}
