using Microsoft.AspNetCore.Mvc;
using StampSystem.Data;
using StampSystem.Models;
using System.Threading.Tasks;

namespace StampSystem.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AccountController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult RegisterRequest()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterModel model)
        {
            if (ModelState.IsValid)
            {
                var registrationRequest = new RegistrationRequest
                {
                    FullName = model.Input.FullName,
                    NationalID = model.Input.NationalID,
                    Email = model.Input.Email,
                    PhoneNumber = model.Input.PhoneNumber,
                    EmployeeId = model.Input.EmployeeId,
                    AdministrationId = model.Input.AdministrationId ?? 0,
                    SectionId = model.Input.SectionId ?? 0,
                    Role = model.Input.Role,
                    Status = "Pending",
                    AdministrationName = model.Input.AdministrationName,
                    SectionName = model.Input.SectionName,
                    UnitName = model.Input.UnitName
                };

                _context.RegistrationRequests.Add(registrationRequest);
                await _context.SaveChangesAsync();

                return RedirectToAction("RegistrationPending");
            }

            return View(model);
        }

        // هذا أكشن بديل لتسجيل طلب بدون استخدام RegisterModel، لو حبيت تستخدمه
        [HttpPost]
        public IActionResult RegisterRequest(RegistrationRequest model)
        {
            if (!ModelState.IsValid)
                return View(model);

            model.Status = "Pending"; // وضع الطلب بانتظار الموافقة
            _context.RegistrationRequests.Add(model);
            _context.SaveChanges();

            TempData["Message"] = "تم إرسال طلبك بنجاح، يرجى انتظار موافقة الموارد البشرية.";
            return RedirectToAction("RegisterRequest");
        }
    }
}
