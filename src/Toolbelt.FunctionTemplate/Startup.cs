using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.IO;
using Toolbelt.Cosmos;
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

        builder.Services
            .AddCosmos(options =>
            {
                options.CosmosEndpoint = config.GetValue<string>("Cosmos:Endpoint");
                options.CosmosKey = config.GetValue<string>("Cosmos:CosmosKey");
                options.DatabaseId = config.GetValue<string>("Cosmos:DatabaseId");
                options.ContainerName = config.GetValue<string>("Cosmos:ContainerName");
            });

    }
}