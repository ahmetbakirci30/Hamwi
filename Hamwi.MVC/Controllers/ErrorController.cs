using Microsoft.AspNetCore.Mvc;

namespace Hamwi.MVC.Controllers
{
    public class ErrorController : Controller
    {
        public IActionResult Index()
            => View();

        public IActionResult PageNotFound()
            => View("_PageNotFound");
    }
}