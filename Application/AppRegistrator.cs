using DomainService;
using Infrastructure.MongoDB;
using Infrastructure.RepositoryCore;
using MelbergFramework.Application;
using MelbergFramework.Core.Time;

namespace Application;

public class AppRegistrator : Registrator
{

    public override void RegisterServices(IServiceCollection services)
    {
        services.AddTransient<IDeviceDomainService, DeviceDomainService>();
        services.AddTransient<IDeviceRepository,PublicDeviceRepository>();

        services.AddSingleton<IClock, Clock>();
        services.AddSingleton<DeviceClient>();

        services.AddOptions<MongoDBOptions>()
            .BindConfiguration(MongoDBOptions.Section)
            .ValidateDataAnnotations();
    }
}
