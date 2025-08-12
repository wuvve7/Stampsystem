using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using StampSystem.Data;
using StampSystem.Models;
using StampSystem.Utility;
using System.Linq;
using System.Threading.Tasks;

namespace StampSystem.Controllers
{
    [Authorize(Roles = CD.Role_HR)]
    public class SectionsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SectionsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Sections
        public async Task<IActionResult> Index()
        {
            var sections = _context.Sections.Include(s => s.Administration);
            return View(await sections.ToListAsync());
        }

        // GET: Sections/Create
        public IActionResult Create()
        {
            ViewData["AdministrationId"] = new SelectList(_context.Administrations, "Id", "AdministrationName");
            return View();
        }

        // POST: Sections/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,SectionName,HeadName,AdministrationId")] Section section)
        {
            if (ModelState.IsValid)
            {
                _context.Add(section);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["AdministrationId"] = new SelectList(_context.Administrations, "Id", "AdministrationName", section.AdministrationId);
            return View(section);
        }

        // GET: Sections/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var section = await _context.Sections.FindAsync(id);
            if (section == null)
                return NotFound();

            ViewData["AdministrationId"] = new SelectList(_context.Administrations, "Id", "AdministrationName", section.AdministrationId);
            return View(section);
        }

        // POST: Sections/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,SectionName,HeadName,AdministrationId")] Section section)
        {
            if (id != section.Id)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(section);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SectionExists(section.Id))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["AdministrationId"] = new SelectList(_context.Administrations, "Id", "AdministrationName", section.AdministrationId);
            return View(section);
        }

        private bool SectionExists(int id)
        {
            return _context.Sections.Any(e => e.Id == id);
        }

        // GET: Sections/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var section = await _context.Sections
                .Include(s => s.Administration)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (section == null)
                return NotFound();

            return View(section);
        }

        // GET: Sections/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var section = await _context.Sections
                .Include(s => s.Administration)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (section == null)
                return NotFound();

            return View(section);
        }

        // POST: Sections/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var section = await _context.Sections.FindAsync(id);
            if (section != null)
            {
                _context.Sections.Remove(section);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

    }

}
