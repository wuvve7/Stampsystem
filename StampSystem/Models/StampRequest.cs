namespace StampSystem.Models
{
    public class StampRequest
    {
        public int Id { get; set; }
        public string RequesterId { get; set; } // معرف المستخدم اللي قدم الطلب (ForeignKey)
        public ApplicationUser Requester { get; set; }

        public string ReferenceNumber { get; set; } // رقم مرجعي فريد (مثلاً GUID أو رقم متسلسل)

        public string StampType { get; set; } // "شخصي" أو "رسمي"
        public string Purpose { get; set; } // سبب الطلب

        public string Status { get; set; } // حالة الطلب (PendingApprovalByManager, PendingApprovalByHR, Approved, RejectedByManager, RejectedByHR)

        public string? ApprovalNotes { get; set; } // ملاحظات الرفض أو الموافقة
        public DateTime RequestDate { get; set; } = DateTime.Now;

        // روابط للإدارات والوحدات حسب الحاجة (اختياري)
        public int? AdministrationId { get; set; }
        public int? SectionId { get; set; }
        public int? UnitId { get; set; }
    }
}
