using Pustok_BackProject.Models;

namespace Pustok_BackProject.Interfaces
{
    public interface ILayoutService
    {
        Task<IDictionary<string,string>>GetSettings();
        Task<IEnumerable<Category>> GetCategories();
    }
}
