using System.Linq.Expressions;
using Toolbelt.Abstractions.Entities;

namespace Toolbelt.Cosmos.Models;

public class CosmosRequest<T> : EntityRequest where T : CosmosEntity
{
    public CosmosRequest(string id, string partitionKey) : base(id)
    {
        PartitionKey = partitionKey;
    }

    public CosmosRequest(string id, string partitionKey, string etag) : this(id, partitionKey)
    {
        eTag = etag;
    }

    public CosmosRequest(string partitionKey, Expression<Func<T, bool>> expression)
    {
        PartitionKey = partitionKey;
        Expression = expression;
    }

    public Expression<Func<T, bool>>? Expression { get; set; }
    public string PartitionKey { get; set; }
    public string? eTag { get; set; }
}
