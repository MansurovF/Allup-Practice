using System.ComponentModel.DataAnnotations;

namespace Pustok_BackProject.Models
{
    public class Tag:BaseEntity
    {
        [StringLength(255)]
        public string Name { get; set; }
        public IEnumerable<ProductTag>? ProductTags { get; set; }
    }
}
