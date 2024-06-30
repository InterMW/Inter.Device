using Device.Grpc;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddGrpc();
var app = builder.Build();

app.MapGet("/device", () => "Hello World!");

app.MapGrpcService<DeviceGrpcServer>().RequireHost("*:6000");

app.Run();
