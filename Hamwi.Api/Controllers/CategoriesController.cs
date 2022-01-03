using Hamwi.Api.Controllers.Base;
using Hamwi.Shared.Entities;
using Hamwi.Shared.Filters.Entity;
using Hamwi.Shared.Repositories.Interfaces;

namespace Hamwi.Api.Controllers
{
    public class CategoriesController : ControllerBase<Category, CategoryFilter>
    {
        public CategoriesController(IGenericRepository<Category> repository) : base(repository) { }
    }
}