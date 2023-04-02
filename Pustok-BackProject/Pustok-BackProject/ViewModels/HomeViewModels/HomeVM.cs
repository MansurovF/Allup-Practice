using Pustok_BackProject.Models;

namespace Pustok_BackProject.ViewModels.HomeViewModels
{
    public class HomeVM
    {
        public IEnumerable<Slider> Sliders{ get; set; }
        public IEnumerable<Category> Categories{ get; set; }
        public IEnumerable<Product> FeaturedProducts { get; set; }
        public IEnumerable<Product> NewArrivals { get; set; }
        public IEnumerable<Product> MostviewProducts { get; set; }
    }
}
