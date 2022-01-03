using Hamwi.Api.Controllers.Base;
using Hamwi.Shared.Entities.Statuses;
using Hamwi.Shared.Filters.Entity.Statuses;
using Hamwi.Shared.Repositories.Interfaces;

namespace Hamwi.Api.Controllers.Statuses
{
    public class ImagesController : ControllerBase<Image, ImageFilter>
    {
        public ImagesController(IGenericRepository<Image> repository) : base(repository) { }
    }
}