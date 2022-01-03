using Hamwi.Shared.Entities.Interfaces;
using Hamwi.Shared.Filters.Entity.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Hamwi.Shared.Services.Hamwi.Interfaces
{
    public interface IHamwiService<T> where T : IEntity
    {
        Task<IEnumerable<T>> GetAsync();
        Task<IEnumerable<T>> GetAsync(IFilter<T> filter);
        Task<T> GetAsync(Guid id);
        Task<T> AddAsync(T entity);
        Task<T> UpdateAsync(T entity);
        Task<T> DeleteAsync(Guid id);
        Task<decimal?> CountAsync(IFilter<T> filter = null);
    }
}