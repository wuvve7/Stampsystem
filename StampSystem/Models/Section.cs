using System.ComponentModel.DataAnnotations;

namespace StampSystem.Models
{
    public class Section
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; } = string.Empty;


        public string HeadName { get; set; } = string.Empty;

        // الربط بالإدارة
        public int AdministrationId { get; set; }
        public Administration? Administration { get; set; }

        // علاقات: تحتوي على وحدات
        public ICollection<Unit> Units { get; set; } = new List<Unit>();
    }
}
