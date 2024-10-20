using System.Diagnostics;
using Device.GrpcClient;

var builder = WebApplication.CreateBuilder(args);
DeviceGrpcDependencyModule.RegisterClient(builder.Services);
var app = builder.Build();

var j = app.Services.GetService<IDeviceGrpcClient>();
var sn = "aaaaaaaaaaaa";
await j.SetDeviceLifeState(sn, true);

var device = await j.GetDeviceAsync(sn);

Console.WriteLine(device.IsOnline);

await j.SetDeviceLifeState(sn, false);

device = await j.GetDeviceAsync(sn);

Console.WriteLine(device.IsOnline);


await foreach( var dev in 
        j.GetDevicesAsync(CancellationToken.None))
{
    Console.WriteLine(dev.SerialNumber, dev.IsOnline);
}
await j.CreateDeviceAsync("aaaaaaaaaaaa");

//app.MapGet("/", () => "Hello World!");

//app.Run();
