using System.ComponentModel.DataAnnotations;

namespace StampSystem.Models
{
    public class UserRegistration
    {
        public int Id { get; set; }

        [Required]
        public string FullName { get; set; }

        [Required]
        public string EmployeeNumber { get; set; }

        [Required]
        public string IDNumber { get; set; }

        [Required]
        [Phone]
        public string PhoneNumber { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Role { get; set; } // "مدير إدارة" أو "رئيس قسم" أو "رئيس وحدة"

        public int? AdministrationId { get; set; }
        public Administration? Administration { get; set; }

        public int? SectionId { get; set; }
        public Section? Section { get; set; }

        public int? UnitId { get; set; }
        public Unit? Unit { get; set; }

        public string Status { get; set; } = "Pending"; // أو Approved / Rejected

        public string? RejectionReason { get; set; }
    }
}
