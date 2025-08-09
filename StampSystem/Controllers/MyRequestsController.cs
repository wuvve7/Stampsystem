using Microsoft.AspNetCore.Mvc;
using StampSystem.Data;

namespace StampSystem.Controllers
{
    public class MyRequestsController : Controller
    {
            private readonly ApplicationDbContext _context;

            public MyRequestsController(ApplicationDbContext context)
            {
                _context = context;
            }

            public IActionResult Index(string nationalId)
            {
                var requests = _context.RegistrationRequests
                    .Where(r => r.NationalID == nationalId)
                    .ToList();

                return View(requests);
            }
        }

    }

