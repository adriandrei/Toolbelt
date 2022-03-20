using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Linq;
using Microsoft.Extensions.Logging;
using System.Linq.Expressions;
using Toolbelt.Cosmos.Interfaces;
using Toolbelt.Cosmos.Models;

namespace Toolbelt.Cosmos.Services;

public class CosmosRepository<T, K> : BaseCosmosRepository<T, K>, ICosmosRepository<T, K>
    where T : CosmosEntity
    where K : CosmosRequest<T>
{
    public CosmosRepository(Container container, ILogger<BaseCosmosRepository<T, K>> logger) : base(container, logger)
    {
    }

    public async Task<int> CountAsync(string partitionKey, Expression<Func<T, bool>> expression)
    {
        if (string.IsNullOrWhiteSpace(partitionKey))
            throw new ArgumentNullException(nameof(partitionKey));

        var requestOptions = new QueryRequestOptions();
        requestOptions.EnableScanInQuery = true;

        var queryDefinition = container.GetItemLinqQueryable<T>().Where(expression).ToQueryDefinition();
        var queryText = queryDefinition.QueryText.Replace("VALUE root", "VALUE COUNT(1)", StringComparison.OrdinalIgnoreCase);
        var queryIterator = container.GetItemQueryIterator<int>(queryText, null, requestOptions);
        var response = await queryIterator.ReadNextAsync();
        return response.Resource.FirstOrDefault();
    }

    public async Task<List<T>> ListAsync(string partitionKey, Expression<Func<T, bool>> expression)
    {
        if (string.IsNullOrWhiteSpace(partitionKey))
            throw new ArgumentNullException(nameof(partitionKey));

        var result = new List<T>();
        using (FeedIterator<T> iterator = this.container.GetItemLinqQueryable<T>()
            .Where(expression)
            .ToFeedIterator())
        {
            while (iterator.HasMoreResults)
            {
                foreach (var item in await iterator.ReadNextAsync())
                {
                    result.Add(item);
                }
            }
        }

        return result;
    }
}
