using Hamwi.Shared.Entities.Interfaces;
using System.Linq;

namespace Hamwi.Shared.Filters.Pagination.Interfaces
{
    public interface IPaginationFilter<T> where T : IEntity
    {
        int PageNumber { get; set; }
        int PageSize { get; set; }

        IQueryable<T> ConfigurePagination(IQueryable<T> initialSet);
    }
}