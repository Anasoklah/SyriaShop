using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;

namespace SyriaShop.IntegrationTests;

public sealed class CatalogApiFactory : WebApplicationFactory<Program>
{
    public TestClock Clock { get; } = new();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Development");
        builder.ConfigureTestServices(services =>
        {
            var timeProviderRegistration = services.Single(service =>
                service.ServiceType == typeof(TimeProvider));

            services.Remove(timeProviderRegistration);
            services.AddSingleton<TimeProvider>(Clock);
        });
    }
}
