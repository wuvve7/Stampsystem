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
    // لو تبي تحدد فقط موارد بشرية تدخل، فعل هذا السطر
    //[Authorize(Roles = "HR")]
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
            var pendingUsers = _userManager.Users
            .Where(u => u.Status == "Pending")
            .ToList();

            return View(pendingUsers);
        }


        // الموافقة على طلب التسجيل
        [HttpPost]
        public async Task<IActionResult> Approve(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                TempData["ErrorMessage"] = "المستخدم غير موجود.";
                return RedirectToAction("Index");
            }

            user.Status = "Approved";
            user.RejectionReason = null; // نحذف أي سبب رفض سابق
            var result = await _userManager.UpdateAsync(user);

            if (!result.Succeeded)
            {
                // تعامل مع الخطأ حسب الحاجة
                ModelState.AddModelError("", "فشل تحديث حالة المستخدم");
                return RedirectToAction("Index");
            }
            TempData["SuccessMessage"] = $"تمت الموافقة على {user.FullName} بنجاح";
            return RedirectToAction("Index");
        }
               


            // رفض طلب التسجيل
        [HttpPost]
        public async Task<IActionResult> Reject(string userId, string reason)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                TempData["ErrorMessage"] = "المستخدم غير موجود";
                return RedirectToAction("Index");
            }

            user.Status = "Rejected";
            user.RejectionReason = string.IsNullOrWhiteSpace(reason) ? "لم يتم ذكر السبب" : reason;
            var result = await _userManager.UpdateAsync(user);

            if (!result.Succeeded)
            {
                TempData["ErrorMessage"] = "فشل تحديث حالة المستخدم";
                return RedirectToAction("Index");
            }
            TempData["ErrorMessage"] = $"تم رفض {user.FullName} - السبب: {user.RejectionReason}";
            return RedirectToAction("Index");
        }
        
        // صفحة تعرض رسالة انتظار الموافقة
        //[AllowAnonymous]
        public IActionResult RegistrationPending()
        {
            return View();
        }
    }
}
