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
        public IActionResult Sepet()
        {
            var kullaniciId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (kullaniciId == null)
            {
                return RedirectToAction("Login");
            }

            var sepetItems = _context.SepetlerView
                .Where(s => s.KullaniciId == int.Parse(kullaniciId))
                .ToList();

            decimal totalPrice = 0;
            int totalItemCount = 0;
            using (var command = _context.Database.GetDbConnection().CreateCommand())
            {
                command.CommandText = "SELECT calculate_cart_total(@kullanici_id)";
                command.Parameters.Add(new NpgsqlParameter("@kullanici_id", int.Parse(kullaniciId)));

                _context.Database.OpenConnection();
                totalPrice = (decimal)command.ExecuteScalar();
                _context.Database.CloseConnection();
            }

            using (var command = _context.Database.GetDbConnection().CreateCommand())
            {
                command.CommandText = "SELECT calculate_cart_item_count(@kullanici_id)";
                command.Parameters.Add(new NpgsqlParameter("@kullanici_id", int.Parse(kullaniciId)));

                _context.Database.OpenConnection();
                totalItemCount = (int)command.ExecuteScalar();
                _context.Database.CloseConnection();
            }

            ViewBag.TotalPrice = totalPrice;
            ViewBag.TotalItemCount = totalItemCount;

            return View(sepetItems);
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

            using (var command = _context.Database.GetDbConnection().CreateCommand())
            {
                command.CommandText = "CALL add_to_cart(@kullanici_id, @urun_id)";
                command.Parameters.Add(new NpgsqlParameter("@kullanici_id", parsedKullaniciId));
                command.Parameters.Add(new NpgsqlParameter("@urun_id", urunId));

                _context.Database.OpenConnection();
                command.ExecuteNonQuery();
                _context.Database.CloseConnection();
            }

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

            if (!int.TryParse(kullaniciId, out int parsedKullaniciId))
            {
                _logger.LogWarning("Kullanıcı kimliği geçersiz, oturum açma ekranına yönlendiriliyor.");
                return RedirectToAction("Login");
            }

            using (var command = _context.Database.GetDbConnection().CreateCommand())
            {
                command.CommandText = "CALL remove_from_cart(@kullanici_id, @urun_id)";
                command.Parameters.Add(new NpgsqlParameter("@kullanici_id", parsedKullaniciId));
                command.Parameters.Add(new NpgsqlParameter("@urun_id", urunId));

                _context.Database.OpenConnection();
                command.ExecuteNonQuery();
                _context.Database.CloseConnection();
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
                .OrderBy(s => s.Urun.Ad)
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
                            SiparisDurumId = 1
                        };

                        _context.Siparisler.Add(siparis);
                        urun.Stok -= item.Miktar;

                        _context.Sepetler.Remove(item);
                    }
                    else
                    {
                        TempData["ErrorMessage"] = $"Ürün '{urun.Ad}' için yeterli stok bulunmamaktadır.";
                        return RedirectToAction("Sepet");
                    }
                }
            }
            _context.SaveChanges();
            return RedirectToAction("Siparis");
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

            var kullanici = _context.KullanicilarView.FirstOrDefault(k => k.Id == int.Parse(kullaniciId));
            if (kullanici == null)
            {
                return NotFound();
            }

            return View(kullanici);
        }

        public IActionResult Siparis()
        {
            var kullaniciId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (kullaniciId == null)
            {
                return RedirectToAction("Login");
            }

            var siparisler = _context.SiparislerView
                .Where(s => s.KullaniciId == int.Parse(kullaniciId))
                .ToList();

            return View(siparisler);
        }


    }
}