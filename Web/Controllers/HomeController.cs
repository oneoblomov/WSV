using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Web.Data;
using Web.Models;
using System.Security.Claims;
using System.Diagnostics;

namespace Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ECommerceContext _context;

        public HomeController(ILogger<HomeController> logger, ECommerceContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index(int? kategoriId = null, string query = null, int page = 1, int pageSize = 12)
        {
            ViewData["ShowKategori"] = true;
            ViewData["name"] = "Urunler";

            IQueryable<Urun> urunlerQuery = _context.Urunler;

            if (kategoriId.HasValue)
            {
                urunlerQuery = urunlerQuery.Where(u => u.KategoriId == kategoriId.Value);
                var kategori = _context.Kategoriler.FirstOrDefault(k => k.Id == kategoriId.Value);
                if (kategori != null)
                {
                    ViewData["name"] = kategori.Ad;
                    ViewData["KategoriId"] = kategoriId.Value;
                }
            }

            if (!string.IsNullOrEmpty(query))
            {
                query = query.ToLower();
                urunlerQuery = urunlerQuery.Where(u => u.Ad.ToLower().Contains(query) || u.Aciklama.ToLower().Contains(query));
                ViewData["name"] = $"{ViewData["name"]} için Arama Sonuçları: {query}";
                ViewBag.Query = query;
            }

            var urunler = urunlerQuery
                .OrderBy(u => u.Id)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var totalItems = urunlerQuery.Count();
            ViewBag.TotalPages = (int)Math.Ceiling((double)totalItems / pageSize);
            ViewBag.CurrentPage = page;

            if (!urunler.Any())
            {
                TempData["UyariMesaji"] = "Ürün bulunmamaktadır.";
            }


            return View(urunler);
        }
        public IActionResult Kategori(int id, int page = 1, int pageSize = 12)
        {
            var kategori = _context.Kategoriler.FirstOrDefault(k => k.Id == id);
            if (kategori == null)
            {
                return NotFound();
            }

            ViewData["ShowKategori"] = true;
            ViewData["name"] = kategori.Ad;
            ViewData["KategoriId"] = id;

            var urunler = _context.Urunler
                .Where(u => u.KategoriId == id)
                .OrderBy(u => u.Id)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            if (!urunler.Any())
            {
                TempData["UyariMesaji"] = "Bu kategoride ürün bulunmamaktadır.";
            }

            var totalItems = _context.Urunler.Count(u => u.KategoriId == id);
            ViewBag.TotalPages = (int)Math.Ceiling((double)totalItems / pageSize);
            ViewBag.CurrentPage = page;

            return View("Index", urunler);
        }

        public IActionResult Arama(string query, int page = 1, int pageSize = 12)
        {
            if (string.IsNullOrEmpty(query))
            {
                return RedirectToAction("Index");
            }

            query = query.ToLower();

            var urunler = _context.Urunler
                .Where(u => u.Ad.ToLower().Contains(query) || u.Aciklama.ToLower().Contains(query))
                .OrderBy(u => u.Id)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var totalItems = _context.Urunler.Count(u => u.Ad.ToLower().Contains(query) || u.Aciklama.ToLower().Contains(query));
            ViewBag.TotalPages = (int)Math.Ceiling((double)totalItems / pageSize);
            ViewBag.CurrentPage = page;
            ViewBag.Query = query;

            return View("Index", urunler);
        }

        public IActionResult UrunDetay(int id)
        {
            var urun = _context.Urunler.FirstOrDefault(u => u.Id == id);
            if (urun == null)
            {
                return NotFound();
            }

            // Görüntülenme sayısını veritabanında doğrudan artır
            _context.Urunler
                .Where(u => u.Id == id)
                .ExecuteUpdate(u => u.SetProperty(p => p.GorSayi, p => p.GorSayi + 1));

            // Güncel ürünü tekrar çek
            return View(urun);
        }

        [HttpPost]
        public IActionResult SepeteEkle(int urunId)
        {
            var kullaniciId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(kullaniciId))
            {
                _logger.LogWarning("Kullanıcı kimliği bulunamadı, oturum açma ekranına yönlendiriliyor.");
                return RedirectToAction("Login", "Home");
            }

            if (!int.TryParse(kullaniciId, out int parsedKullaniciId))
            {
                _logger.LogWarning("Kullanıcı kimliği geçersiz, oturum açma ekranına yönlendiriliyor.");
                return RedirectToAction("Login", "Home");
            }

            var urun = _context.Urunler.FirstOrDefault(u => u.Id == urunId);
            if (urun == null)
            {
                return NotFound();
            }

            var sepet = _context.Sepetler.FirstOrDefault(s => s.K_id == parsedKullaniciId && s.U_id == urunId);

            if (sepet == null)
            {
                sepet = new Sepet
                {
                    K_id = parsedKullaniciId,
                    U_id = urunId,
                    Miktar = 1,
                };
                _context.Sepetler.Add(sepet);
            }
            else
            {
                sepet.Miktar++;
            }

            _context.SaveChanges();

            return RedirectToAction("Index");
        }
        [HttpPost]
        public IActionResult SepettenSil(int urunId)
        {
            var kullaniciId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(kullaniciId))
            {
                return RedirectToAction("Login");
            }

            var sepetItem = _context.Sepetler.FirstOrDefault(s => s.K_id == int.Parse(kullaniciId) && s.U_id == urunId);
            if (sepetItem != null)
            {
                _context.Sepetler.Remove(sepetItem);
                _context.SaveChanges();
            }

            return RedirectToAction("Sepet");
        }

        [HttpPost]
        public IActionResult SepetiOnayla()
        {
            var kullaniciId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(kullaniciId))
            {
                return RedirectToAction("Login");
            }

            var sepetItems = _context.Sepetler
                .Include(s => s.Urun)
                .Where(s => s.K_id == int.Parse(kullaniciId))
                .OrderBy(s => s.Urun.Ad) // Ürünleri ada göre sıralayın (isteğe bağlı)
                .ToList();

            foreach (var item in sepetItems)
            {
                var urun = _context.Urunler.FirstOrDefault(u => u.Id == item.U_id);
                if (urun != null)
                {
                    if (urun.Stok >= item.Miktar)
                    {
                        var siparis = new Siparis
                        {
                            KullaniciId = item.K_id,
                            UrunId = item.U_id,
                            Miktar = item.Miktar,
                            Fiyat = item.Urun.Fiyat * item.Miktar,
                            Tarih = DateTime.UtcNow,
                            SiparisDurumId = 1 // Varsayılan değer
                        };

                        _context.Siparisler.Add(siparis);

                        // Ürünün stok miktarını azalt
                        urun.Stok -= item.Miktar;

                        _context.Sepetler.Remove(item);
                    }
                    else
                    {
                        // Stok yetersizse hata mesajı göster
                        TempData["ErrorMessage"] = $"Ürün '{urun.Ad}' için yeterli stok bulunmamaktadır.";
                        return RedirectToAction("Sepet");
                    }
                }
            }

            _context.SaveChanges();

            return RedirectToAction("Siparis");
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

        [HttpGet]
        public IActionResult Sepet()
        {
            var kullaniciId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (kullaniciId == null)
            {
                return RedirectToAction("Login");
            }

            var sepetItems = _context.Sepetler
                .Include(s => s.Urun)
                .Where(s => s.K_id == int.Parse(kullaniciId))
                .ToList();

            return View(sepetItems);
        }

        public IActionResult Siparis()
        {
            var kullaniciId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (kullaniciId == null)
            {
                return RedirectToAction("Login");
            }

            var siparisler = _context.Siparisler
                .Include(s => s.Urun)
                .Include(s => s.SiparisDurum) // SiparisDurum ilişkisini dahil edin
                .Where(s => s.KullaniciId == int.Parse(kullaniciId))
                .ToList();

            return View(siparisler);
        }
    }
}