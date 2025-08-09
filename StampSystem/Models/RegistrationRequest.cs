using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

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
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [Phone]
        public string PhoneNumber { get; set; }
        [Required]
        public int EmployeeId { get; set; }

        [Required]
        public int AdministrationId { get; set; }
        public Administration? Administration { get; set; }

        public int SectionId { get; set; }
        public Section? Section { get; set; }

        public int UnitId { get; set; }
        public Unit? Unit { get; set; }
        public string? AdministrationName { get; set; }
        public string? SectionName { get; set; }
        public string? UnitName { get; set; }


        public string Status { get; set; } = "Pending";
        public string Role { get; set; }
        public string? RegistrationReason { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public string? RejectionReason { get; set; } // سبب الرفض
    }

}


