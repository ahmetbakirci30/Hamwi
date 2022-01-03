using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Hamwi.MVC.Models;
using Hamwi.Shared.Entities.Statuses;
using Hamwi.Shared.Services.Client.Interfaces;
using System;
using System.Threading.Tasks;

namespace Hamwi.MVC.Controllers
{
    [Authorize]
    public class ImagesController : Controller
    {
        private readonly IHamwiServices _Hamwi;

        public ImagesController(IHamwiServices Hamwi)
            => _Hamwi = Hamwi;

        public ActionResult Index()
            => View();

        public async Task<ActionResult> Details(Guid id)
            => View(await _Hamwi.ImageService.GetAsync(id));

        public ActionResult Create()
            => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(ImageViewModel model)
        {
            try
            {
                if (model.ImageFile == null || model.ImageFile.Length <= 0)
                {
                    ModelState.AddModelError(string.Empty, "Please choose an image first!");
                    return View(model);
                }

                return ((await _Hamwi.ImageService.AddAsync(new Image
                {
                    ImagePath = await UploadFile(model),
                    CategoryId = model.CategoryId,
                    Active = model.Active
                })) != null) ? RedirectToAction(nameof(Index)) : View(model);
            }
            catch
            {
                return View();
            }
        }

        public async Task<ActionResult> Edit(Guid id)
        {
            var image = await _Hamwi.ImageService.GetAsync(id);

            return View((image == null) ? null : new ImageViewModel
            {
                Id = image.Id,
                CategoryId = image.CategoryId,
                ImagePath = image.ImagePath,
                Active = image.Active,
                ViewsCount = image.ViewsCount,
                LikesCount = image.LikesCount,
                SharesCount = image.SharesCount,
                DownloadsCount = image.DownloadsCount
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(ImageViewModel model)
        {
            try
            {
                return ((await _Hamwi.ImageService.UpdateAsync(new Image
                {
                    Id = model.Id,
                    CategoryId = model.CategoryId,
                    ImagePath = await UploadFile(model),
                    Active = model.Active,
                    ViewsCount = model.ViewsCount,
                    LikesCount = model.LikesCount,
                    SharesCount = model.SharesCount,
                    DownloadsCount = model.DownloadsCount
                })) != null) ? RedirectToAction(nameof(Index)) : View(model);
            }
            catch
            {
                return View();
            }
        }

        public async Task<ActionResult> Delete(Guid id)
            => View(await _Hamwi.ImageService.GetAsync(id));

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(Image image)
        {
            try
            {
                await _Hamwi.FileService.DeleteAsync(image.ImagePath);
                return ((await _Hamwi.ImageService.DeleteAsync(image.Id)) != null) ? RedirectToAction(nameof(Index)) : View(image);
            }
            catch
            {
                return View();
            }
        }

        private async Task<string> UploadFile(ImageViewModel model)
            => (model.ImageFile != null && model.ImageFile.Length > 0) ?
            await _Hamwi.FileService.UploadAsync(model.ImageFile) : model.ImagePath;
    }
}