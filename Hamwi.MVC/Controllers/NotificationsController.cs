using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Hamwi.Shared.Entities;
using Hamwi.Shared.Services.Client.Interfaces;
using System;
using System.Threading.Tasks;

namespace Hamwi.MVC.Controllers
{
    [Authorize]
    public class NotificationsController : Controller
    {
        private readonly IHamwiServices _Hamwi;

        public NotificationsController(IHamwiServices Hamwi)
            => _Hamwi = Hamwi;

        public ActionResult Index()
            => View();

        public async Task<ActionResult> Details(Guid id)
            => View(await _Hamwi.NotificationService.GetAsync(id));

        public ActionResult Create()
            => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(Notification notification)
            => ((await _Hamwi.NotificationService.AddAsync(notification)) != null) ? RedirectToAction(nameof(Index)) : View(notification);


        public async Task<ActionResult> Edit(Guid id)
            => View(await _Hamwi.NotificationService.GetAsync(id));

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(Notification notification)
            => ((await _Hamwi.NotificationService.UpdateAsync(notification)) != null) ? RedirectToAction(nameof(Index)) : View(notification);


        public async Task<ActionResult> Delete(Guid id)
            => View(await _Hamwi.NotificationService.GetAsync(id));

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(Notification notification)
            => ((await _Hamwi.NotificationService.DeleteAsync(notification.Id)) != null) ? RedirectToAction(nameof(Index)) : View(notification);
    }
}