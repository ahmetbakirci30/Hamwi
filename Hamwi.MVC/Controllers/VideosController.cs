using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Hamwi.MVC.Models;
using Hamwi.Shared.Entities.Statuses;
using Hamwi.Shared.Services.Client.Interfaces;
using System;
using System.Threading.Tasks;

namespace Hamwi.MVC.Controllers
{
    [Authorize]
    public class VideosController : Controller
    {
        private readonly IHamwiServices _Hamwi;

        public VideosController(IHamwiServices Hamwi)
            => _Hamwi = Hamwi;

        public ActionResult Index()
            => View();

        public async Task<ActionResult> Details(Guid id)
            => View(await _Hamwi.VideoService.GetAsync(id));

        public ActionResult Create()
            => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(VideoViewModel model)
        {
            try
            {
                if (model.CoverFile == null || model.CoverFile.Length <= 0 || model.VideoFile == null || model.VideoFile.Length <= 0)
                {
                    ModelState.AddModelError(string.Empty, "Please choose a video and a cover image of it first!");
                    return View(model);
                }

                return ((await _Hamwi.VideoService.AddAsync(new Video
                {
                    Title = model.Title,
                    VideoPath = await UploadFile(model.VideoFile, model.VideoPath),
                    CoverPath = await UploadFile(model.CoverFile, model.CoverPath),
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
            var video = await _Hamwi.VideoService.GetAsync(id);

            return View((video == null) ? null : new VideoViewModel
            {
                Id = video.Id,
                Title = video.Title,
                CategoryId = video.CategoryId,
                VideoPath = video.VideoPath,
                CoverPath = video.CoverPath,
                Active = video.Active,
                ViewsCount = video.ViewsCount,
                LikesCount = video.LikesCount,
                SharesCount = video.SharesCount,
                DownloadsCount = video.DownloadsCount
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(VideoViewModel model)
        {
            try
            {
                return ((await _Hamwi.VideoService.UpdateAsync(new Video
                {
                    Id = model.Id,
                    Title = model.Title,
                    CategoryId = model.CategoryId,
                    VideoPath = await UploadFile(model.VideoFile, model.VideoPath),
                    CoverPath = await UploadFile(model.CoverFile, model.CoverPath),
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
            => View(await _Hamwi.VideoService.GetAsync(id));

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(Video video)
        {
            try
            {
                await _Hamwi.FileService.DeleteAsync(video.CoverPath);
                await _Hamwi.FileService.DeleteAsync(video.VideoPath);
                return ((await _Hamwi.VideoService.DeleteAsync(video.Id)) != null) ? RedirectToAction(nameof(Index)) : View(video);
            }
            catch
            {
                return View();
            }
        }

        private async Task<string> UploadFile(IFormFile file, string path)
            => (file != null && file.Length > 0) ? await _Hamwi.FileService.UploadAsync(file) : path;
    }
}