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
    public class UnitsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public UnitsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Units
        public async Task<IActionResult> Index()
        {
            var units = _context.Units.Include(u => u.Section);
            return View(await units.ToListAsync());
        }

        // GET: Units/Create
        public IActionResult Create()
        {
            ViewData["SectionId"] = new SelectList(_context.Sections, "Id", "SectionName");
            return View();
        }

        // POST: Units/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,UnitName,HeadName,SectionId")] Unit unit)
        {
            if (ModelState.IsValid)
            {
                _context.Add(unit);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["SectionId"] = new SelectList(_context.Sections, "Id", "SectionName", unit.SectionId);
            return View(unit);
        }

        // GET: Units/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var unit = await _context.Units.FindAsync(id);
            if (unit == null)
                return NotFound();

            ViewData["SectionId"] = new SelectList(_context.Sections, "Id", "SectionName", unit.SectionId);
            return View(unit);
        }

        // POST: Units/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,UnitName,HeadName,SectionId")] Unit unit)
        {
            if (id != unit.Id)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(unit);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Units.Any(e => e.Id == unit.Id))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["SectionId"] = new SelectList(_context.Sections, "Id", "SectionName", unit.SectionId);
            return View(unit);
        }

        // GET: Units/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var unit = await _context.Units
                .Include(u => u.Section)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (unit == null)
                return NotFound();

            return View(unit);
        }

        // GET: Units/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var unit = await _context.Units
                .Include(u => u.Section)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (unit == null)
                return NotFound();

            return View(unit);
        }

        // POST: Units/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var unit = await _context.Units.FindAsync(id);
            if (unit != null)
            {
                _context.Units.Remove(unit);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
 
    }

}
