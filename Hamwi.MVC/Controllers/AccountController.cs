using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Hamwi.MVC.Models;
using System.Threading.Tasks;

namespace Hamwi.MVC.Controllers
{
    public class AccountController : Controller
    {
        private readonly SignInManager<IdentityUser> _signInManager;

        public AccountController(SignInManager<IdentityUser> signInManager) => _signInManager = signInManager;

        public IActionResult Login() => View();

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, false);
                if (result.Succeeded) return (string.IsNullOrWhiteSpace(returnUrl) || (!Url.IsLocalUrl(returnUrl))) ? RedirectToAction("index", "home") : Redirect(returnUrl);

                ModelState.AddModelError(string.Empty, "Invalid Login Attempt!");
            }

            return View(model);
        }
    }
}