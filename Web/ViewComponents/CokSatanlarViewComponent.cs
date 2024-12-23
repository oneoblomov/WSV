using Microsoft.AspNetCore.Mvc;
using Web.Data;
using System.Linq;

namespace Web.ViewComponents
{
    public class CokSatanlarViewComponent : ViewComponent
    {
        private readonly ECommerceContext _context;

        public CokSatanlarViewComponent(ECommerceContext context)
        {
            _context = context;
        }

        public IViewComponentResult Invoke()
        {
            var cokSatanlar = _context.Urunler
                .OrderByDescending(u => u.GorSayi)
                .Take(8)
                .ToList();

            return View(cokSatanlar);
        }
    }
}