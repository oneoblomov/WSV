using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Web.Data;
using Web.Models;
using System.Security.Claims;
using System.Diagnostics;
using Npgsql;

namespace Web.Controllers
{
    public partial class HomeController : Controller
    {
        [HttpGet]
        public IActionResult Login()
        {
            ViewData["ShowCokSatanlar"] = true;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            ViewData["ShowCokSatanlar"] = true;
            if (ModelState.IsValid)
            {
                var normalizedEmail = model.Email.ToLower();
                var kullanici = _context.Kullanicilar
                    .FirstOrDefault(k => k.Email.ToLower() == normalizedEmail && k.Sifre == model.Password);

                if (kullanici != null)
                {
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.NameIdentifier, kullanici.Id.ToString()),
                        new Claim(ClaimTypes.Name, kullanici.Email)
                    };

                    var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));

                    return RedirectToAction("Index");
                }
                else
                {
                    ModelState.AddModelError("", "Geçersiz kullanıcı adı veya şifre");
                }
            }
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index");
        }
    }
}