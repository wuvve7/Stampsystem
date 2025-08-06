using System.ComponentModel.DataAnnotations;

namespace StampSystem.Models
{
    public class Administration
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; } = string.Empty;

        [Required]
        public string ManagerName { get; set; } = string.Empty;

        public ICollection<Section>? Sections { get; set; }
    }

}
