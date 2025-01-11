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

            IQueryable<UrunView> urunlerQuery = _context.UrunlerView;

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

            var urunler = _context.UrunlerView
                .Where(u => u.KategoriId == id)
                .OrderBy(u => u.Id)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            if (!urunler.Any())
            {
                TempData["UyariMesaji"] = "Bu kategoride ürün bulunmamaktadır.";
            }

            var totalItems = _context.UrunlerView.Count(u => u.KategoriId == id);
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

            var urunler = _context.UrunlerView
                .Where(u => u.Ad.ToLower().Contains(query) || u.Aciklama.ToLower().Contains(query))
                .OrderBy(u => u.Id)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var totalItems = _context.UrunlerView.Count(u => u.Ad.ToLower().Contains(query) || u.Aciklama.ToLower().Contains(query));
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

            _context.Urunler
                .Where(u => u.Id == id)
                .ExecuteUpdate(u => u.SetProperty(p => p.GorSayi, p => p.GorSayi + 1));

            return View(urun);
        }
    }
}