using Hamwi.Shared.Entities;
using Hamwi.Shared.Filters.Entity.Base;
using System.Linq;

namespace Hamwi.Shared.Filters.Entity
{
    public class CategoryFilter : FilterBase<Category>
    {
        public string Name { get; set; }
        public string CoverPath { get; set; }
        public decimal? StatusCount { get; set; }
        public decimal? MinStatusCount { get; set; }
        public decimal? MaxStatusCount { get; set; }

        public override IQueryable<Category> Build(IQueryable<Category> categories, bool applyPagination = true)
        {
            categories = string.IsNullOrWhiteSpace(Name) ? categories :
                categories.Where(category => category.Name.ToLower().Contains(Name.ToLower()));

            categories = string.IsNullOrWhiteSpace(CoverPath) ? categories :
                categories.Where(category => category.CoverPath.ToLower().Contains(CoverPath.ToLower()));

            categories = (StatusCount.HasValue && StatusCount.Value >= 0) ?
                categories.Where(category => category.StatusCount.Equals(StatusCount)) : categories;

            categories = (MinStatusCount.HasValue && MinStatusCount.Value >= 0) ?
                categories.Where(category => category.StatusCount >= MinStatusCount) : categories;

            categories = (MaxStatusCount.HasValue && MaxStatusCount.Value >= 0) ?
                categories.Where(category => category.StatusCount <= MaxStatusCount) : categories;

            return base.Build(categories, applyPagination);
        }

        public override string ToString()
            => $"{base.ToString()}&{nameof(Name)}={Name}&{nameof(CoverPath)}={CoverPath}" +
            $"&{nameof(StatusCount)}={StatusCount}&{nameof(MinStatusCount)}={MinStatusCount}" +
            $"&{nameof(MaxStatusCount)}={MaxStatusCount}";
    }
}