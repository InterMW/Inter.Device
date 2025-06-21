using System.Text.Json.Serialization;

namespace Application.DataModels;

public class DeviceRegistrationRequest
{
    [JsonPropertyName("serialNumber")]
    public string SerialNumber {get; set;} = "";

    [JsonPropertyName("ipAddress")]
    public string IPAddress {get; set;} = "";

}
