using Application;
using Device.Grpc;
using MelbergFramework.Application;

internal class Program
{
    private static async Task Main(string[] args)
    {
        ThreadPool.SetMinThreads(8, 8); //required
        await MelbergHost
            .CreateHost<AppRegistrator>()
            .AddServices(_ =>
                    {
                        _.AddControllers();
                        _.AddGrpc();
                    })
             .ConfigureApp(_ =>
                    {
                        _.UseSwagger();
                        _.UseSwaggerUI();
                        _.UseRouting();
                        _.MapControllers();
                        _.MapGrpcService<DeviceGrpcServer>().RequireHost("*:6000");
                    })
            .Build()
            .RunAsync();
    }
}
