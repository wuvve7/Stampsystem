using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StampSystem.Data;
using StampSystem.Models;
using static Azure.Core.HttpHeader;


namespace StampSystem.Controllers
{
    [Authorize]
    public class StampRequestsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public StampRequestsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // صفحة تقديم طلب ختم
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        // حفظ طلب جديد
        [HttpPost]
        public async Task<IActionResult> Create(string stampType, string purpose)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return Unauthorized();

            // تحديد الحالة الابتدائية حسب دور المستخدم
            string initialStatus = "PendingApprovalByManager"; // رئيس قسم/وحدة

            // إذا المستخدم مدير إدارة، الحالة تذهب مباشرة للموارد البشرية
            var roles = await _userManager.GetRolesAsync(user);
            if (roles.Contains("DivisionDirector"))
            {
                initialStatus = "PendingApprovalByHR";
            }
            else if (roles.Contains("SectionHead") || roles.Contains("UnitHead")) // رئيس قسم أو وحدة
            {
                initialStatus = "PendingApprovalByManager";
            }

            var request = new StampRequest
            {
                RequesterId = user.Id,
                ReferenceNumber = Guid.NewGuid().ToString().Substring(0, 8).ToUpper(),
                StampType = stampType,
                Purpose = purpose,
                Status = initialStatus,
                RequestDate = DateTime.Now,
                ApprovalNotes = ""
            };

            _context.StampRequests.Add(request);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = $"تم تقديم طلبك بنجاح. الرقم المرجعي: {request.ReferenceNumber}";
            return RedirectToAction("MyRequests");
        }

        // عرض طلبات المستخدم الحالي
        public async Task<IActionResult> MyRequests()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return Unauthorized();

            var requests = await _context.StampRequests
                .Where(r => r.RequesterId == user.Id)
                .ToListAsync();

            return View(requests);
        }

        // عرض طلبات المدير (في انتظار الموافقة منه)
        [Authorize(Roles = "DivisionDirector")]
        public async Task<IActionResult> PendingApprovalByManager()
        {
            var requests = await _context.StampRequests
                .Where(r => r.Status == "PendingApprovalByManager")
                .Include(r => r.Requester)
                .ToListAsync();

            return View(requests);
        }

        // موافقة المدير على طلب
        [Authorize(Roles = "DivisionDirector")]
        [HttpPost]
        public async Task<IActionResult> ApproveByManager(int id, string notes)
        {
            var request = await _context.StampRequests.FindAsync(id);
            if (request == null) return NotFound();

            request.Status = "PendingApprovalByHR";
            request.ApprovalNotes = string.IsNullOrWhiteSpace(notes) ? "تمت الموافقة بدون ملاحظات" : notes;

            await _context.SaveChangesAsync();

            // هنا ممكن تضيف إرسال إشعار لموارد بشرية (اختياري)
            TempData["SuccessMessage"] = $"تمت الموافقة على الطلب رقم {request.ReferenceNumber} وإرساله للموارد البشرية.";

            return RedirectToAction(nameof(PendingApprovalByManager));
        }

        // رفض المدير على طلب
        [Authorize(Roles = "DivisionDirector")]
        [HttpPost]
        public async Task<IActionResult> RejectByManager(int id, string reason)
        {
            var request = await _context.StampRequests.FindAsync(id);
            if (request == null) return NotFound();

            request.Status = "RejectedByManager";
            request.ApprovalNotes = reason;

            await _context.SaveChangesAsync();

            // إرسال إشعار للموظف (اختياري)
            TempData["ErrorMessage"] = $"تم رفض الطلب رقم {request.ReferenceNumber} بسبب: {reason}";

            return RedirectToAction(nameof(PendingApprovalByManager));
        }

        // عرض طلبات الموارد البشرية (في انتظار الموافقة)
        [Authorize(Roles = "HR")]
        public async Task<IActionResult> PendingApprovalByHR()
        {
            var requests = await _context.StampRequests
                .Where(r => r.Status == "PendingApprovalByHR")
                .Include(r => r.Requester)
                .ToListAsync();

            return View(requests);
        }

        // موافقة الموارد البشرية
        [Authorize(Roles = "HR")]
        [HttpPost]
        public async Task<IActionResult> ApproveByHR(int id, string notes)
        {
            var request = await _context.StampRequests.FindAsync(id);
            if (request == null) return NotFound();

            request.Status = "Approved";
            request.ApprovalNotes = string.IsNullOrWhiteSpace(notes) ? "تمت الموافقة بدون ملاحظات" : notes;

            await _context.SaveChangesAsync();

            // إرسال إشعار للموظف (اختياري)
            TempData["SuccessMessage"] = $"تمت الموافقة النهائية على الطلب رقم {request.ReferenceNumber}.";

            return RedirectToAction(nameof(PendingApprovalByHR));
        }

        // رفض الموارد البشرية
        [Authorize(Roles = "HR")]
        [HttpPost]
        public async Task<IActionResult> RejectByHR(int id, string reason)
        {
            var request = await _context.StampRequests.FindAsync(id);
            if (request == null) return NotFound();

            request.Status = "RejectedByHR";
            request.ApprovalNotes = reason;

            await _context.SaveChangesAsync();

            // إرسال إشعار للموظف (اختياري)
            TempData["ErrorMessage"] = $"تم رفض الطلب رقم {request.ReferenceNumber} بسبب: {reason}";

            return RedirectToAction(nameof(PendingApprovalByHR));
        }
        public async Task<IActionResult> Details(int id)
        {
            var request = await _context.StampRequests
                .Include(r => r.Requester) // لو عندك علاقات
                .FirstOrDefaultAsync(r => r.Id == id);

            if (request == null) return NotFound();

            return View(request);
        }

        // عرض تفاصيل طلب معين
        [Authorize(Roles = "HR")]
        [HttpPost]
        public async Task<IActionResult> MarkAsInPreparation(int id)
        {
            var request = await _context.StampRequests.FindAsync(id);
            if (request == null) return NotFound();

            request.Status = "InPreparation"; // انتظار التجهيز
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = $"Request {request.ReferenceNumber} marked as In Preparation.";
            return RedirectToAction("CompletedRequests"); // صفحة الطلبات المكتملة
        }

        [Authorize(Roles = "HR")]
        [HttpPost]
        public async Task<IActionResult> MarkAsReadyForPickup(int id)
        {
            var request = await _context.StampRequests.FindAsync(id);
            if (request == null) return NotFound();

            request.Status = "ReadyForPickup"; // انتظار التسليم
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = $"Request {request.ReferenceNumber} marked as Ready for Pickup.";
            return RedirectToAction("CompletedRequests");
        }

        [Authorize(Roles = "HR")]
        [HttpPost]
        public async Task<IActionResult> MarkAsDelivered(int id)
        {
            var request = await _context.StampRequests.FindAsync(id);
            if (request == null) return NotFound();

            request.Status = "Delivered"; // تم التسليم
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = $"Request {request.ReferenceNumber} marked as Delivered.";
            return RedirectToAction("CompletedRequests");
        }
        [Authorize(Roles = "HR")]
        public async Task<IActionResult> CompletedRequests()
        {
            var requests = await _context.StampRequests
                .Where(r => r.Status == "InPreparation" || r.Status == "ReadyForPickup" || r.Status == "Delivered")
                .Include(r => r.Requester)
                .ToListAsync();

            return View(requests);
        }

    }
}
