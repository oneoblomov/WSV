using Microsoft.AspNetCore.Mvc;
using Web.Data;
using Web.Models;
using System.Linq;

namespace Web.ViewComponents
{
    public class KategoriViewComponent : ViewComponent
    {
        private readonly ECommerceContext _context;

        public KategoriViewComponent(ECommerceContext context)
        {
            _context = context;
        }

        public IViewComponentResult Invoke()
        {
            var kategoriler = _context.Kategoriler.ToList();
            return View(kategoriler);
        }
    }
}