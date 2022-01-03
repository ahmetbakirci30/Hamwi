using Hamwi.Shared.Entities.Interfaces;
using System.Linq;

namespace Hamwi.Shared.Filters.Entity.Interfaces
{
    public interface IFilter<T> where T : IEntity
    {
        IQueryable<T> Build(IQueryable<T> initialSet, bool applyPagination = true);
    }
}