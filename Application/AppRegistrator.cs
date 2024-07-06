using Infrastructure.Couchbase;
using Infrastructure.RepositoryCore;
using MelbergFramework.Application;
using MelbergFramework.Infrastructure.Couchbase;

namespace Application;

public class AppRegistrator : Registrator
{

    public override void RegisterServices(IServiceCollection services)
    {
        CouchbaseModule.RegisterCouchbaseBucket<IDeviceRepository,DeviceRepository>(services);
    }

}
