using System.ComponentModel.DataAnnotations;

namespace StampSystem.Models
{
    public class Unit
    {
        public int Id { get; set; }

        [Required]
        public string UnitName { get; set; }

        // رئيس الوحدة
        public string HeadName { get; set; }

        // الربط بالقسم
        [Required]
        public int SectionId { get; set; }
        public Section? Section { get; set; }
    }
}