﻿namespace Device.Domain;

public class DeviceModel
{
    public string SerialNumber { get; set; } = "";
    public string IpAddress { get; set; } = "";
    public int Port { get; set; } = 0;
    public bool IsOnline { get; set; } = false;
    public DateTime FirstHeardFrom { get; set; } = DateTime.UtcNow;
    public DateTime LastPowerChange { get; set; } = DateTime.MinValue;
    public float Latitude { get; set; }
    public float Longitude { get; set; }
}
