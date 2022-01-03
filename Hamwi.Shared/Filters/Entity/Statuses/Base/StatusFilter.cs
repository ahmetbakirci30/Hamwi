using Hamwi.Shared.Entities.Statuses.Base;
using Hamwi.Shared.Filters.Entity.Base;
using System;
using System.Linq;

namespace Hamwi.Shared.Filters.Entity.Statuses.Base
{
    public abstract class StatusFilter<T> : FilterBase<T> where T : StatusBase, new()
    {
        public decimal? ViewsCount { get; set; }
        public decimal? MinViewsCount { get; set; }
        public decimal? MaxViewsCount { get; set; }
        public decimal? LikesCount { get; set; }
        public decimal? MinLikesCount { get; set; }
        public decimal? MaxLikesCount { get; set; }
        public decimal? SharesCount { get; set; }
        public decimal? MinSharesCount { get; set; }
        public decimal? MaxSharesCount { get; set; }
        public decimal? DownloadsCount { get; set; }
        public decimal? MinDownloadsCount { get; set; }
        public decimal? MaxDownloadsCount { get; set; }
        public Guid? CategoryId { get; set; }

        public override IQueryable<T> Build(IQueryable<T> initialSet, bool applyPagination = true)
        {
            initialSet = (ViewsCount.HasValue && ViewsCount.Value >= 0) ?
                initialSet.Where(status => status.ViewsCount.Equals(ViewsCount)) : initialSet;

            initialSet = (MinViewsCount.HasValue && MinViewsCount.Value >= 0) ?
                initialSet.Where(status => status.ViewsCount >= MinViewsCount) : initialSet;

            initialSet = (MaxViewsCount.HasValue && MaxViewsCount.Value >= 0) ?
                initialSet.Where(status => status.ViewsCount <= MaxViewsCount) : initialSet;

            initialSet = (LikesCount.HasValue && LikesCount.Value >= 0) ?
                initialSet.Where(status => status.LikesCount.Equals(LikesCount)) : initialSet;

            initialSet = (MinLikesCount.HasValue && MinLikesCount.Value >= 0) ?
                initialSet.Where(status => status.LikesCount >= MinLikesCount) : initialSet;

            initialSet = (MaxLikesCount.HasValue && MaxLikesCount.Value >= 0) ?
                initialSet.Where(status => status.LikesCount <= MaxLikesCount) : initialSet;

            initialSet = (SharesCount.HasValue && SharesCount.Value >= 0) ?
                initialSet.Where(status => status.SharesCount.Equals(SharesCount)) : initialSet;

            initialSet = (MinSharesCount.HasValue && MinSharesCount.Value >= 0) ?
                initialSet.Where(status => status.SharesCount >= MinSharesCount) : initialSet;

            initialSet = (MaxSharesCount.HasValue && MaxSharesCount.Value >= 0) ?
                initialSet.Where(status => status.SharesCount <= MaxSharesCount) : initialSet;

            initialSet = (DownloadsCount.HasValue && DownloadsCount.Value >= 0) ?
                initialSet.Where(status => status.DownloadsCount.Equals(DownloadsCount)) : initialSet;

            initialSet = (MinDownloadsCount.HasValue && MinDownloadsCount.Value >= 0) ?
                initialSet.Where(status => status.DownloadsCount >= MinDownloadsCount) : initialSet;

            initialSet = (MaxDownloadsCount.HasValue && MaxDownloadsCount.Value >= 0) ?
                initialSet.Where(status => status.DownloadsCount <= MaxDownloadsCount) : initialSet;

            initialSet = (CategoryId.HasValue && (!CategoryId.Value.Equals(Guid.Empty))) ?
                initialSet.Where(status => status.CategoryId.Equals(CategoryId)) : initialSet;

            return base.Build(initialSet, applyPagination);
        }

        public override string ToString()
            => $"{base.ToString()}&{nameof(ViewsCount)}={ViewsCount}&{nameof(MinViewsCount)}={MinViewsCount}" +
            $"&{nameof(MaxViewsCount)}={MaxViewsCount}&{nameof(LikesCount)}={LikesCount}&{nameof(MinLikesCount)}={MinLikesCount}" +
            $"&{nameof(MaxLikesCount)}={MaxLikesCount}&{nameof(SharesCount)}={SharesCount}&{nameof(MinSharesCount)}={MinSharesCount}" +
            $"&{nameof(MaxSharesCount)}={MaxSharesCount}&{nameof(DownloadsCount)}={DownloadsCount}&{nameof(MinDownloadsCount)}={MinDownloadsCount}" +
            $"&{nameof(MaxDownloadsCount)}={MaxDownloadsCount}&{nameof(CategoryId)}={CategoryId}";
    }
}