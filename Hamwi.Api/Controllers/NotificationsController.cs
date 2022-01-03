using Hamwi.Api.Controllers.Base;
using Hamwi.Shared.Entities;
using Hamwi.Shared.Filters.Entity;
using Hamwi.Shared.Repositories.Interfaces;

namespace Hamwi.Api.Controllers
{
    public class NotificationsController : ControllerBase<Notification, NotificationFilter>
    {
        public NotificationsController(IGenericRepository<Notification> repository) : base(repository) { }
    }
}