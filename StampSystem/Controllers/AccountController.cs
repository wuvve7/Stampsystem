using Microsoft.AspNetCore.Mvc;
using StampSystem.Data;
using StampSystem.Models;

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
        public IActionResult RegisterRequest(RegistrationRequest model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            model.Status = "Pending"; // ينتظر موافقة HR
            _context.RegistrationRequests.Add(model);
            _context.SaveChanges();

            TempData["Message"] = "تم إرسال طلبك بنجاح، يرجى انتظار موافقة الموارد البشرية.";
            return RedirectToAction("RegisterRequest");
        }
    }
}
