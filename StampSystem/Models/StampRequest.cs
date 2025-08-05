using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using StampSystem.Enums;

namespace StampSystem.Models
{
    public class StampRequest
    {
        [Key]
        public int RequestId { get; set; }
        [Required] 
        public string StampType { get; set; }

        [Required]
        public string Reason { get; set; }

        public StatusType Status { get; set; } = StatusType.New;

        public bool? IsApprovedByDirector { get; set; }
        public string DirectorRejectionReason { get; set; }

        public bool? IsApprovedByHR { get; set; }
        public string HRRejectionReason { get; set; }

        [ForeignKey("Users")]
        public string EmployeeId { get; set; }

        public Users Users { get; set; }
    }
}
