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
            .AddControllers()
            .Build()
            .RunAsync();
    }
}
