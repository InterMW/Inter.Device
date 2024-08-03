using DomainService;
using Infrastructure.Couchbase;
using Infrastructure.RepositoryCore;
using MelbergFramework.Application;
using MelbergFramework.Infrastructure.Couchbase;

namespace Application;

public class AppRegistrator : Registrator
{

    public override void RegisterServices(IServiceCollection services)
    {
        services.AddTransient<IDeviceDomainService, DeviceDomainService>();
        CouchbaseModule.RegisterCouchbaseBucket<IDeviceRepository,DeviceRepository>(services);
    }

}
