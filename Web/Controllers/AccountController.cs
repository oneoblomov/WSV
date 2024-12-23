using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Web.Data;
using Web.Models;
using System.Security.Claims;

namespace Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly ECommerceContext _context;

        public AccountController(ECommerceContext context)
        {
            _context = context;
        }

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
                var kullanici = _context.Kullanicilar
                    .FirstOrDefault(k => k.Email == model.Email && k.Sifre == model.Password);

                if (kullanici != null)
                {
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.NameIdentifier, kullanici.Id.ToString()),
                        new Claim(ClaimTypes.Name, kullanici.Email)
                    };

                    var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));

                    return RedirectToAction("Index", "Home");
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
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult Profile()
        {
            ViewData["ShowCokSatanlar"] = true;

            var kullaniciId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (kullaniciId == null)
            {
                return RedirectToAction("Login");
            }

            var kullanici = _context.Kullanicilar.FirstOrDefault(k => k.Id == int.Parse(kullaniciId));
            if (kullanici == null)
            {
                return NotFound();
            }

            return View(kullanici);
        }
    }
}