using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StampSystem.Data;
using StampSystem.Models;

namespace StampSystem.Controllers
{
    public class HRController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public HRController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // عرض الطلبات المعلقة
        public IActionResult PendingRequests()
        {
            var requests = _context.RegistrationRequests
                .Include(r => r.Administration)
                .Include(r => r.Section)
                .Include(r => r.Unit)
                .Where(r => r.Status == "Pending")
                .ToList();
            return View(requests);
        }

        // الموافقة على طلب
        [HttpPost]
        public async Task<IActionResult> Approve(int id, string role)
        {
            var request = await _context.RegistrationRequests.FindAsync(id);
            if (request == null) return NotFound();

            request.Status = "Approved";

            var user = new ApplicationUser
            {
                UserName = request.NationalID,
                Email = request.Email,
                FullName = request.FullName,
                NationalID = request.NationalID,
                EmployeeId = request.EmployeeId,
                AdministrationId = request.AdministrationId,
                SectionId = request.SectionId,
                UnitId = request.UnitId,
                PhoneNumber = request.PhoneNumber,
                Status = "Approved"
            };

            var password = "Password123!"; // كلمة مرور مؤقتة
            var result = await _userManager.CreateAsync(user, password);

            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, role);
                _context.SaveChanges();
            }

            return RedirectToAction("PendingRequests");
        }

        // رفض الطلب
        [HttpPost]
        public IActionResult Reject(int id, string reason)
        {
            var request = _context.RegistrationRequests.Find(id);
            if (request == null) return NotFound();

            request.Status = "Rejected";
            request.RejectionReason = reason;
            _context.SaveChanges();

            return RedirectToAction("PendingRequests");
        }
    }
}
