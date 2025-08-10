using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StampSystem.Models
{
    public class RegistrationRequest
    {
        public int Id { get; set; } // مفتاح رئيسي تلقائي (Auto-increment)
        [Required]
        public string FullName { get; set; }
        [Required]
        public string NationalID { get; set; }
        [Required] 
        public string Email { get; set; }
        [Required]
        public string PhoneNumber { get; set; }

        [Required]
        public int EmployeeId { get; set; }

        public int? AdministrationId { get; set; }
        public Administration? Administration { get; set; }

        // ربط المستخدم بالقسم
        public int? SectionId { get; set; }
        public Section? Section { get; set; }

        // ربط المستخدم بالوحدة
        public int? UnitId { get; set; }
        public Unit? Unit { get; set; }
        [NotMapped]
        public string? AdministrationName { get; set; }
        [NotMapped]
        public string? SectionName { get; set; }
        [NotMapped]
        public string? UnitName { get; set; }
        [Required(ErrorMessage = "Role is required.")]
        public string?Role { get; set; } = string.Empty;


        public string Status { get; set; } = "Pending";
        
        public string? RegistrationReason { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public string? RejectionReason { get; set; } // سبب الرفض
    }

}


