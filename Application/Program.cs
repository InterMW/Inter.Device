using Application;
using Application.Processors;
using Authorizer.GrpcClient;
using Device.Grpc;
using DomainService;
using DomainService.MessageStrategies;
using Infrastructure.MongoDB;
using Infrastructure.Mqtt;
using Infrastructure.Rabbit;
using Infrastructure.RepositoryCore;
using MelbergFramework.Core.Time;
using MelbergFramework.Infrastructure.Rabbit;
using Microsoft.Extensions.Options;
using MQTTnet;
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddCors(
    options =>
    {
        options.AddPolicy(name: "MyPolicy",
            builder => builder.AllowAnyHeader()
            .AllowAnyMethod()
            .SetIsOriginAllowed((host) => 
              {
                  var valuee = host switch 
                  {
                      "http://localhost:1808" or "https://wip.centurionx.net" or "https://www.centurionx.net" => true,
                     _ => false
                  };
                    return valuee;
              }
              )
        );
    }
);      

builder.Services.AddSingleton<IIpRepository,IpRepository>();
builder.Services.AddOptions<IpOptions>()
                .BindConfiguration(IpOptions.Section)
                .ValidateDataAnnotations();
builder.Services.AddOptions<MongoDBOptions>()
                .BindConfiguration(MongoDBOptions.Section)
                .ValidateDataAnnotations();

builder.Services.AddTransient<IMqttProvider , MqttProvider>();

builder.Services.AddTransient<IMessageStrategy, PlaneMessageStrategy>();
builder.Services.AddTransient<IMessageStrategy, LifeMessageStrategy>();
builder.Services.AddTransient<IMessageStrategy, CheckInStrategy>();
builder.Services.AddTransient<IMessageStrategy, InfoMessageStrategy>();

RabbitModule.RegisterPublisher<PlaneFrameMessage>(builder.Services);
builder.Services.AddTransient<IPlaneFramePublisher,PlaneFramePublisher>();
builder.Services.AddSingleton<IClock, Clock>();
builder.Services.AddSingleton<DeviceClient>();
builder.Services.AddTransient<IDeviceDomainService, DeviceDomainService>();
builder.Services.AddTransient<IDeviceRepository,PublicDeviceRepository>();
AuthorizerGrpcDependencyModule.RegisterClient(builder.Services);
builder.Services
        .AddGrpc(options => 
        {
            options.Interceptors.Add<ServerExceptionInterceptor>();
        });
builder.Services.AddControllers();
builder.Services.AddOptions<MqttOptions>()
            .BindConfiguration(MqttOptions.Section);
        builder.Services.AddSingleton<MqttClientOptions>(_ =>
                {
                    var option = _.GetService<IOptions<MqttOptions>>().Value;
                    var mqttFactory = new MqttClientFactory();
                    var tlsOptions = new MqttClientTlsOptionsBuilder().UseTls(false).Build();
                    var mqttClientOptions = new MqttClientOptionsBuilder()
                       .WithTcpServer(option.Host)
                        .WithTlsOptions(tlsOptions)
                        .WithProtocolVersion(MQTTnet.Formatter.MqttProtocolVersion.V311)
                        .WithCredentials(option.User, option.Password).Build();

                    return mqttClientOptions;
                });
builder.Services.AddHostedService<DeviceMessageProcessor>();


var app = builder.Build();

if(app.Environment.IsDevelopment())
{
  app.Configuration["Mqtt:Password"] = app.Configuration[ "mqtt_pass"];
  app.Configuration["Mqtt:User"] = app.Configuration[ "mqtt_user"];
  app.Configuration["Mqtt:Host"] = app.Configuration[ "mqtt_host"];
  app.Configuration["Rabbit:ClientDeclarations:Connections:0:Password"] = app.Configuration[ "rabbit_pass"];
}
app.UseRouting();
app.MapGrpcService<DeviceGrpcServer>().RequireHost("*:6000");
app.UseCors("MyPolicy");
app.MapControllers();

app.Run();
