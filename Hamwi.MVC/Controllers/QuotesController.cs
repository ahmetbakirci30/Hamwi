using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Hamwi.Shared.Entities.Statuses;
using Hamwi.Shared.Services.Client.Interfaces;
using System;
using System.Threading.Tasks;

namespace Hamwi.MVC.Controllers
{
    [Authorize]
    public class QuotesController : Controller
    {
        private readonly IHamwiServices _Hamwi;

        public QuotesController(IHamwiServices Hamwi)
            => _Hamwi = Hamwi;

        public ActionResult Index()
            => View();

        public async Task<ActionResult> Details(Guid id)
            => View(await _Hamwi.QuoteService.GetAsync(id));

        public ActionResult Create()
            => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(Quote quote)
            => ((await _Hamwi.QuoteService.AddAsync(quote)) != null) ? RedirectToAction(nameof(Index)) : View(quote);


        public async Task<ActionResult> Edit(Guid id)
            => View(await _Hamwi.QuoteService.GetAsync(id));

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(Quote quote)
            => ((await _Hamwi.QuoteService.UpdateAsync(quote)) != null) ? RedirectToAction(nameof(Index)) : View(quote);


        public async Task<ActionResult> Delete(Guid id)
            => View(await _Hamwi.QuoteService.GetAsync(id));

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(Quote quote)
            => ((await _Hamwi.QuoteService.DeleteAsync(quote.Id)) != null) ? RedirectToAction(nameof(Index)) : View(quote);
    }
}