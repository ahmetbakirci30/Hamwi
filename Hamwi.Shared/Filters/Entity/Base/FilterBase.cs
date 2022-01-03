using Hamwi.Shared.Entities.Base;
using Hamwi.Shared.Filters.Entity.Interfaces;
using Hamwi.Shared.Filters.Pagination;
using System;
using System.Linq;

namespace Hamwi.Shared.Filters.Entity.Base
{
    public abstract class FilterBase<T> : PaginationFilter<T>, IFilter<T> where T : EntityBase, new()
    {
        public Guid? Id { get; set; }
        public DateTime? Date { get; set; }
        public DateTime? FirstDate { get; set; }
        public DateTime? LastDate { get; set; }
        public bool? Shuffle { get; set; }
        public bool Active { get; set; } = true;

        public virtual IQueryable<T> Build(IQueryable<T> initialSet, bool applyPagination = true)
        {
            initialSet = (Id.HasValue && (Id.Value != Guid.Empty)) ?
                initialSet.Where(entity => entity.Id == Id) : initialSet;

            initialSet = (Date.HasValue && Date.Value > DateTime.MinValue && Date.Value < DateTime.MaxValue) ?
                initialSet.Where(entity => entity.Date == Date) : initialSet;

            initialSet = (FirstDate.HasValue && FirstDate.Value > DateTime.MinValue && FirstDate.Value < DateTime.MaxValue) ?
                initialSet.Where(entity => entity.Date >= FirstDate) : initialSet;

            initialSet = (LastDate.HasValue && LastDate.Value > DateTime.MinValue && LastDate.Value < DateTime.MaxValue) ?
                initialSet.Where(entity => entity.Date <= LastDate) : initialSet;

            initialSet = (Shuffle.HasValue && Shuffle.Value.Equals(true)) ?
                initialSet.OrderBy(_ => Guid.NewGuid()) : initialSet;

            initialSet = initialSet.Where(entity => entity.Active == Active);

            return applyPagination ? ConfigurePagination(initialSet) : initialSet;
        }

        public override string ToString()
            => $"?{nameof(PageNumber)}={PageNumber}&{nameof(PageSize)}={PageSize}&{nameof(Id)}={Id}" +
            $"&{nameof(Date)}={Date}&{nameof(FirstDate)}={FirstDate}&{nameof(LastDate)}={LastDate}" +
            $"&{nameof(Shuffle)}={Shuffle}&{nameof(Active)}={Active}";
    }
}