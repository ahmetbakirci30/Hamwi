using Hamwi.Shared.Entities;
using Hamwi.Shared.Filters.Entity.Base;
using System.Linq;

namespace Hamwi.Shared.Filters.Entity
{
    public class NotificationFilter : FilterBase<Notification>
    {
        public string Title { get; set; }
        public string Text { get; set; }
        public decimal? ReceivedCount { get; set; }
        public decimal? OpenedCount { get; set; }

        public override IQueryable<Notification> Build(IQueryable<Notification> notifications, bool applyPagination = true)
        {
            notifications = string.IsNullOrWhiteSpace(Title) ? notifications :
                notifications.Where(category => category.Title.ToLower().Contains(Title.ToLower()));

            notifications = string.IsNullOrWhiteSpace(Text) ? notifications :
                notifications.Where(category => category.Text.ToLower().Contains(Text.ToLower()));

            notifications = (ReceivedCount.HasValue && ReceivedCount.Value >= 0) ?
                notifications.Where(category => category.ReceivedCount.Equals(ReceivedCount)) : notifications;

            notifications = (OpenedCount.HasValue && OpenedCount.Value >= 0) ?
                notifications.Where(category => category.OpenedCount.Equals(OpenedCount)) : notifications;

            return base.Build(notifications, applyPagination);
        }

        public override string ToString()
            => $"{base.ToString()}&{nameof(Title)}={Title}&{nameof(Text)}={Text}" +
            $"&{nameof(ReceivedCount)}={ReceivedCount}&{nameof(OpenedCount)}={OpenedCount}";
    }
}