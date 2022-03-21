using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.DependencyInjection;
using Toolbelt.Abstractions.Interfaces;
using Toolbelt.Cosmos.Interfaces;
using Toolbelt.Cosmos.Services;

namespace Toolbelt.Cosmos;

public class Options
{
    public string CosmosEndpoint { get; set; }
    public string CosmosKey { get; set; }
    public string DatabaseId { get; set; }
    public string ContainerName { get; set; }
}
public static class Startup
{
    public static IServiceCollection AddCosmos(this IServiceCollection services, Action<Options> configure)
    {
        services.AddSingleton(provider =>
        {
            var options = new Options();
            configure(options);

            return options;
        });

        services.AddSingleton(provider =>
        {
            var options = provider.GetService<Options>();
            var cosmosClient = new CosmosClient(options.CosmosEndpoint, options.CosmosKey);

            EnsureDbIsInitialized(cosmosClient, options.DatabaseId, options.ContainerName)
                .GetAwaiter()
                .GetResult();

            var container = cosmosClient.GetContainer(options.DatabaseId, options.ContainerName);

            return container;
        });

        services.AddSingleton(typeof(IBaseRepository<,>), typeof(BaseCosmosRepository<,>));
        services.AddSingleton(typeof(ICosmosRepository<,>), typeof(CosmosRepository<,>));

        return services;
    }

    public static async Task EnsureDbIsInitialized(
            CosmosClient cosmosClient,
            string dbName,
            string collectionName)
    {
        await CreateDatabaseIfNotExistsAsync(cosmosClient, dbName);
        await CreateCollectionIfNotExistsAsync(cosmosClient, dbName, collectionName);
    }
    private static async Task CreateDatabaseIfNotExistsAsync(CosmosClient client, string databaseId)
    {
        await client.CreateDatabaseIfNotExistsAsync(databaseId);
    }
    private static async Task CreateCollectionIfNotExistsAsync(CosmosClient client, string databaseId, string collectionId)
    {
        var db = client.GetDatabase(databaseId);
        await db.CreateContainerIfNotExistsAsync(collectionId, "/partitionKey");
    }
}
