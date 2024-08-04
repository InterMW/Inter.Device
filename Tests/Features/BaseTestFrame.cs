using Application;
using LightBDD.MsTest3;
using MelbergFramework.Application;
using MelbergFramework.ComponentTesting.Couchbase;
using MelbergFramework.Core.ComponentTesting;
using MelbergFramework.Core.DependencyInjection;
using MelbergFramework.Core.Time;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Tests.Features;

public class BaseTestFrame : FeatureFixture
{
    public BaseTestFrame()
    {
        App = MelbergHost
                .CreateHost<AppRegistrator>()
                .AddServices(_ => 
                {
                    _.OverrideWithSingleton<IClock,MockClock>();
                    _.OverrideCouchbaseDatabase();
                })
                .AddControllers()
                .Build();

    }

    public WebApplication App;

    public T GetClass<T>() => (T)App
        .Services
        .GetRequiredService(typeof(T));
}
