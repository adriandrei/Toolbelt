using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Logging;
using System.Net;
using Toolbelt.Abstractions.Interfaces;
using Toolbelt.Cosmos.Models;

namespace Toolbelt.Cosmos.Services;

public class BaseCosmosRepository<T, K> : IBaseRepository<T, K> where T : CosmosEntity where K : CosmosRequest<T>
{
    protected readonly Container container;
    protected readonly ILogger<BaseCosmosRepository<T, K>> logger;

    private const int RETRY_IN_MILISECONDS = 500;
    private const int RETRY_NUMBER = 3;

    public BaseCosmosRepository(
        Container container,
        ILogger<BaseCosmosRepository<T, K>> logger)
    {
        this.container = container;
        this.logger = logger;
    }

    public async Task<T> CreateAsync(T entity)
    {
        try
        {
            entity.PartitionKey = entity.GetPartitionKey();
            var result = await this.container.CreateItemAsync(entity);

            return result.Resource;
        }
        catch (Exception ex)
        {
            this.logger.LogError(ex, ex.Message);
            throw;
        }
    }

    public async Task DeleteAsync(K request)
    {
        try
        {
            await this.container.DeleteItemAsync<T>(
                id: request.Id,
                partitionKey: new PartitionKey(request.PartitionKey),
                requestOptions: new ItemRequestOptions { IfMatchEtag = request.eTag },
                CancellationToken.None);

        }
        catch (Exception ex)
        {
            this.logger.LogError(ex, ex.Message);
            throw;
        }
    }

    public async Task<T> GetAsync(K request)
    {
        var result = await this.container.ReadItemAsync<T>(request.Id, new PartitionKey(request.PartitionKey));

        return result.Resource;
    }

    public async Task<T> UpdateAsync(K request, Action<T> updateAction)
    {
        int i = 0;
        while (i < RETRY_NUMBER)
        {
            try
            {
                var item = await this.GetAsync(request);
                updateAction(item);
                var updateResponse = await this.container.UpsertItemAsync(
                    item,
                    new PartitionKey(item.PartitionKey),
                    new ItemRequestOptions
                    {
                        IfMatchEtag = item.Etag
                    });

                return item;
            }
            catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.PreconditionFailed)
            {
                i++;
                await Task.Delay(RETRY_IN_MILISECONDS);
                continue;
            }
            catch (Exception)
            {
                throw;
            }
        }

        throw new Exception($"Attempted {RETRY_NUMBER} times to update Item but wasn't able to.");
    }
}
