using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pustok_BackProject.DataAccessLayer;
using Pustok_BackProject.Models;
using Pustok_BackProject.ViewModels.HomeViewModels;

namespace Pustok_BackProject.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppDbContext _context;

        public HomeController(AppDbContext context)
        {
            _context = context;
        }


        public async Task<IActionResult> Index()
        {
            IEnumerable<Slider> sliders = await  _context.Sliders.Where(s => s.IsDeleted == false).ToListAsync();
            IEnumerable<Category> categories = await _context.Categories.Where(s => s.IsDeleted == false).ToListAsync();

            HomeVM homeVM = new HomeVM
            {
                Sliders = sliders,
                Categories = categories,
                FeaturedProducts = await _context.Products.Where(c => c.IsDeleted == false && c.IsFeatured).ToListAsync(),
                NewArrivals = await _context.Products.Where(c => c.IsDeleted == false && c.IsNewArrival).ToListAsync(),
                MostviewProducts = await _context.Products.Where(c => c.IsDeleted == false && c.IsMostviewProducts).ToListAsync()

            };
            return View(sliders);
        }
    }
}
