using Application;
using Device.Grpc;
using MelbergFramework.Application;

internal class Program
{
    private static async Task Main(string[] args)
    {
        await MelbergHost
            .CreateHost<AppRegistrator>()
            .AddServices(_ =>
                    {
                        _.AddGrpc(options => 
                                {
                                    options.Interceptors.Add<ServerExceptionInterceptor>();
                                });
                    })
            .ConfigureApp(app =>
                    {
                        app.MapGrpcService<DeviceGrpcServer>().RequireHost("*:6000");
                    })
            .AddControllersCors()
            .Build()
            .RunAsync();
    }
}

public static class HostExtension
{
    public static MelbergHost AddControllersCors(this MelbergHost host)
    {
        host.ServiceActions += (IServiceCollection _) => {
            _.AddControllers();
            _.AddSwaggerGen();
        };

        host.AppActions += (WebApplication _) => {
            _.UseSwagger();
            _.UseSwaggerUI();
            _.UseCors(_ => _
                 .WithOrigins(["*.centurionx.net"])
                 .AllowAnyHeader()
                 .AllowAnyMethod()
                 .AllowCredentials()
             );
            _.MapControllers();
        };

        return host;
    }
}
