using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using StampSystem.Models;

namespace StampSystem.Models
{
    public class ApplicationUser:IdentityUser
    {
        [Required]
        public string FullName { get; set; }
        [Required]
        public string NationalID{ get; set; }
        [Required]
        public int EmployeeId { get; set; }
        [Required]
        public int? AdministrationId { get; set; } 
        public Administration Administration { get; set; }

        // ربط المستخدم بالقسم
        public int? SectionId { get; set; }
        public Section Section { get; set; }

        // ربط المستخدم بالوحدة
        public int? UnitId { get; set; }
        public string Unit { get; set; }

        public string Status { get; set; } = "Panding"; // حالة الطلبات المعلقة
        public object Role { get; internal set; }
    }
}
