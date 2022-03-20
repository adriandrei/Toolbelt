using Microsoft.Extensions.DependencyInjection;
using Toolbelt.Shared.Services;

namespace Toolbelt.Shared;

public class Options
{
    public string? SomeKey { get; set; }
}

public static class Startup
{
    public static IServiceCollection AddShared(
        this IServiceCollection services,
        Action<Options> configure)
    {
        var options = new Options();
        configure(options);

        services
            .AddSingleton(p => options)
            .AddScoped<IMyService, MyService>()
            .AddScoped<IMyOtherService, MyOtherService>();
        return services;
    }
}