using Toolbelt.Abstractions.Entities;

namespace Toolbelt.Abstractions.Interfaces;

public interface IBaseRepository<T, K> where T : Entity where K : EntityRequest
{
    Task<T> GetAsync(K request);
    Task<T> CreateAsync(T entity);
    Task<T> UpdateAsync(K request, Action<T> updateAction);
    Task DeleteAsync(K request);
}
