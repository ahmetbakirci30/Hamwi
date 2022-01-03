using Microsoft.AspNetCore.Mvc;
using Hamwi.MVC.Models;
using Hamwi.Shared.Filters.Entity.Statuses;
using Hamwi.Shared.Services.Client.Interfaces;
using System.IO;
using System.Net.Mime;
using System.Threading.Tasks;

namespace Hamwi.MVC.Controllers
{
    public class HomeController : Controller
    {
        private readonly IHamwiServices _Hamwi;

        public HomeController(IHamwiServices Hamwi)
            => _Hamwi = Hamwi;

        public async Task<IActionResult> Index()
            => View(new HomeViewModel
            {
                Videos = await _Hamwi.VideoService.GetAsync(new VideoFilter { PageSize = 50, Shuffle = true }),
                Images = await _Hamwi.ImageService.GetAsync(new ImageFilter { PageSize = 50, Shuffle = true }),
                Quotes = await _Hamwi.QuoteService.GetAsync(new QuoteFilter { PageSize = 50, Shuffle = true })
            });

        public async Task<IActionResult> Download(string path)
            => File(await _Hamwi.FileService.DownloadAsync(path), MediaTypeNames.Application.Octet, Path.GetFileName(path));
    }
}