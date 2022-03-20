using System.Linq.Expressions;
using Toolbelt.Abstractions.Interfaces;
using Toolbelt.Cosmos.Models;

namespace Toolbelt.Cosmos.Interfaces;

public interface ICosmosRepository<T, K> : IBaseRepository<T, K> where T : CosmosEntity where K : CosmosRequest<T>
{
    Task<List<T>> ListAsync(string partitionKey, Expression<Func<T, bool>> expression);
    Task<int> CountAsync(string partitionKey, Expression<Func<T, bool>> expression);
}
