using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Hamwi.MVC.Models;
using Hamwi.Shared.Entities;
using Hamwi.Shared.Services.Client.Interfaces;
using System;
using System.Threading.Tasks;

namespace Hamwi.MVC.Controllers
{
    [Authorize]
    public class CategoriesController : Controller
    {
        private readonly IHamwiServices _Hamwi;

        public CategoriesController(IHamwiServices Hamwi)
            => _Hamwi = Hamwi;

        public ActionResult Index()
            => View();

        public async Task<ActionResult> Details(Guid id)
            => View(await _Hamwi.CategoryService.GetAsync(id));

        public ActionResult Create()
            => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(CategoryViewModel model)
        {
            try
            {
                if (model.CoverImageFile == null || model.CoverImageFile.Length <= 0)
                {
                    ModelState.AddModelError(string.Empty, "Please choose an cover image first!");
                    return View(model);
                }

                return ((await _Hamwi.CategoryService.AddAsync(new Category
                {
                    Name = model.Name,
                    CoverPath = await UploadFile(model),
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
            var category = await _Hamwi.CategoryService.GetAsync(id);

            return View((category == null) ? null : new CategoryViewModel
            {
                Id = category.Id,
                Name = category.Name,
                CoverPath = category.CoverPath,
                Active = category.Active,
                StatusCount = category.StatusCount
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(CategoryViewModel model)
        {
            try
            {
                return ((await _Hamwi.CategoryService.UpdateAsync(new Category
                {
                    Id = model.Id,
                    Name = model.Name,
                    CoverPath = await UploadFile(model),
                    Active = model.Active,
                    StatusCount = model.StatusCount
                })) != null) ? RedirectToAction(nameof(Index)) : View(model);
            }
            catch
            {
                return View();
            }
        }

        public async Task<ActionResult> Delete(Guid id)
            => View(await _Hamwi.CategoryService.GetAsync(id));

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(Category category)
        {
            try
            {
                await _Hamwi.FileService.DeleteAsync(category.CoverPath);
                return ((await _Hamwi.CategoryService.DeleteAsync(category.Id)) != null) ? RedirectToAction(nameof(Index)) : View(category);
            }
            catch
            {
                return View();
            }
        }

        private async Task<string> UploadFile(CategoryViewModel model)
            => (model.CoverImageFile != null && model.CoverImageFile.Length > 0) ?
            await _Hamwi.FileService.UploadAsync(model.CoverImageFile) : model.CoverPath;
    }
}