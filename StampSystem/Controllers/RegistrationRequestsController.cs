using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StampSystem.Data;
using StampSystem.Models;
using System.Linq;
using System.Threading.Tasks;

namespace StampSystem.Controllers
{
    // فعّل هذا السطر لو تريد تقييد الولوج للـ HR فقط
     [Authorize(Roles = "HR")]

    public class RegistrationRequestsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public RegistrationRequestsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // عرض طلبات التسجيل المعلقة
        public IActionResult Index()
        {
            var pendingRequests = _context.RegistrationRequests
                .Where(r => r.Status == "Pending")
                .Include(r => r.Administration)
                .Include(r => r.Section)
                .Include(r => r.Unit)
                .ToList();

            return View(pendingRequests);
        }

        // الموافقة على طلب التسجيل
        [HttpPost]
        public async Task<IActionResult> Approve(int id)
        {
            var request = _context.RegistrationRequests
                .Include(r => r.Administration)
                .Include(r => r.Section)
                .Include(r => r.Unit)
                .FirstOrDefault(r => r.Id == id);

            if (request == null) return NotFound();

            // تحقق إذا كان المستخدم موجود مسبقاً
            var existingUser = await _userManager.FindByEmailAsync(request.Email);
            if (existingUser != null)
            {
                existingUser.Status = "Approved";
                await _userManager.UpdateAsync(existingUser);

                if (!await _userManager.IsInRoleAsync(existingUser, request.Role))
                {
                    await _userManager.AddToRoleAsync(existingUser, request.Role);
                }
            }
            else
            {
                var newUser = new ApplicationUser
                {
                    UserName = request.Email,
                    Email = request.Email,
                    FullName = request.FullName,
                    NationalID = request.NationalID,
                    PhoneNumber = request.PhoneNumber,
                    EmployeeId = request.EmployeeId,
                    AdministrationId = request.AdministrationId,
                    SectionId = request.SectionId,
                    UnitId = request.UnitId,
                    Role = request.Role,
                    Status = "Approved"
                };

                var result = await _userManager.CreateAsync(newUser, "DefaultPassword123!");
                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(newUser, request.Role);
                }
                else
                {
                    // لو صار خطأ في إنشاء المستخدم، يمكنك هنا التعامل مع الخطأ أو عرض رسالة
                }
            }

            request.Status = "Approved";
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        // رفض طلب التسجيل
        [HttpPost]
        public async Task<IActionResult> Reject(int id, string reason)
        {
            var request = await _context.RegistrationRequests.FirstOrDefaultAsync(r => r.Id == id);

            if (request == null) return NotFound();

            if (request.Status == "Approved" || request.Status == "Rejected")
                return RedirectToAction("Index");

            request.Status = "Rejected";
            request.RejectionReason = string.IsNullOrWhiteSpace(reason) ? "لم يتم ذكر السبب" : reason;

            var existingUser = await _userManager.FindByEmailAsync(request.Email);
            if (existingUser != null)
            {
                existingUser.Status = "Rejected";
                await _userManager.UpdateAsync(existingUser);
            }

            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        // صفحة تعرض رسالة انتظار الموافقة
        [AllowAnonymous]
        public IActionResult RegistrationPending()
        {
            return View();
        }
    }
}
