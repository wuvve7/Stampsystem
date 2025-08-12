using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using StampSystem.Data;
using StampSystem.Models;
using StampSystem.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StampSystem.Controllers
{
    [Authorize(Roles = CD.Role_HR)]
    public class AdministrationsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdministrationsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Administrations
        public async Task<IActionResult> Index()
        {
            return View(await _context.Administrations.ToListAsync());
        }

        // GET: Administrations/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Administrations/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,AdministrationName")] Administration administration)
        {
            if (ModelState.IsValid)
            {
                _context.Add(administration);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(administration);
        }
    }

}
