using System.Text.Json.Serialization;

namespace Application.DataModels;

public class DeviceResponse
{
    [JsonPropertyName("serialNumber")]
    public string SerialNumber {get; set;} = "";
    
    [JsonPropertyName("isOnline")]
    public bool IsOnline {get; set;}

    [JsonPropertyName("firstHeardFrom")]
    public DateTime FirstHeardFrom {get; set;} = DateTime.UtcNow;

    [JsonPropertyName("lastPowerChange")]
    public DateTime LastPowerChange {get; set;} = DateTime.MinValue;
}
