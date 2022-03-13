using System.IO;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Toolbelt.Shared;

[assembly: FunctionsStartup(typeof(Toolbelt.FunctionTemplate.Startup))]
namespace Toolbelt.FunctionTemplate;

public class Startup : FunctionsStartup
{
    public override void ConfigureAppConfiguration(IFunctionsConfigurationBuilder builder)
    {
        var context = builder.GetContext();

        builder.ConfigurationBuilder
            .AddJsonFile(
                path: Path.Combine(context.ApplicationRootPath, $"appsettings.json"), false);
        
        base.ConfigureAppConfiguration(builder);
    }

    public override void Configure(IFunctionsHostBuilder builder)
    {
        var serviceProvider = builder.Services.BuildServiceProvider();
        var config = serviceProvider.GetService<IConfiguration>();

        builder.Services
            .AddLogging()
            .AddShared(options =>
        {
            var secretKey = config.GetValue<string>("SecretKey");
            options.SomeKey = secretKey;
        });

    }
}