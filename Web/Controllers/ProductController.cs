using Microsoft.AspNetCore.Mvc;
using Web.Data;
using Web.Models;
using System.Security.Claims;

namespace Web.Controllers
{
    public class ProductController : Controller
    {
        private readonly ECommerceContext _context;

        public ProductController(ECommerceContext context)
        {
            _context = context;
        }

        public IActionResult UrunDetay(int id)
        {
            var urun = _context.Urunler.FirstOrDefault(u => u.Id == id);
            if (urun == null)
            {
                return NotFound();
            }

            // Görüntülenme sayısını artır
            urun.GorSayi++;
            _context.SaveChanges();

            return View(urun);
        }

        [HttpPost]
        public IActionResult SepeteEkle(int urunId)
        {
            var kullaniciId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(kullaniciId))
            {
                return RedirectToAction("Login", "Account");
            }

            if (!int.TryParse(kullaniciId, out int parsedKullaniciId))
            {
                return RedirectToAction("Login", "Account");
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

            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public IActionResult SepettenSil(int urunId)
        {
            var kullaniciId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(kullaniciId))
            {
                return RedirectToAction("Login", "Account");
            }

            var sepetItem = _context.Sepetler.FirstOrDefault(s => s.K_id == int.Parse(kullaniciId) && s.U_id == urunId);
            if (sepetItem != null)
            {
                _context.Sepetler.Remove(sepetItem);
                _context.SaveChanges();
            }

            return RedirectToAction("Sepet", "Account");
        }
    }
}