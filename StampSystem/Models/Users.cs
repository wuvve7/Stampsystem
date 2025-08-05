using System.ComponentModel.DataAnnotations;
using StampSystem.Enums;

namespace StampSystem.Models
{
    public class Users
    {
        [Key]
        [Required]
        [StringLength(5, MinimumLength = 5, ErrorMessage = "Employee ID must be 5 digits")]

        public string EmployeeId { get; set; }

        [Required]
        public string FullName { get; set; }

        [Required]
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Department { get; set; }

        [Required]
        [StringLength(10, MinimumLength = 10, ErrorMessage = "National ID must be 10 digits")]

        public string NationalID { get; set; }

        [Required]
        public string Password { get; set; }

        public RoleType Role { get; set; }

        public ICollection<StampRequest> StampRequest { get; set; }

    }
}
