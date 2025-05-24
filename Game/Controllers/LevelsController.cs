using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Game.Data;
using Game.Models;

namespace Game.Controllers
{
    [Authorize(Roles = "Admin")]
    public class LevelsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public LevelsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [AllowAnonymous]
        public async Task<IActionResult> Index(string searchString, string sortOrder)
        {
            ViewData["NameSortParam"] = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewData["DifficultySortParam"] = sortOrder == "difficulty" ? "difficulty_desc" : "difficulty";
            ViewData["CurrentFilter"] = searchString;

            var levels = from l in _context.Levels
                         select l;

            if (!String.IsNullOrEmpty(searchString))
            {
                levels = levels.Where(l => l.Name.Contains(searchString));
            }

            switch (sortOrder)
            {
                case "name_desc":
                    levels = levels.OrderByDescending(l => l.Name);
                    break;
                case "difficulty":
                    levels = levels.OrderBy(l => l.Difficulty);
                    break;
                case "difficulty_desc":
                    levels = levels.OrderByDescending(l => l.Difficulty);
                    break;
                default:
                    levels = levels.OrderBy(l => l.Name);
                    break;
            }

            return View(await levels.ToListAsync());
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Level level)
        {
            if (ModelState.IsValid)
            {
                _context.Add(level);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(level);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var level = await _context.Levels.FindAsync(id);
            if (level == null) return NotFound();
            return View(level);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Level level)
        {
            if (id != level.Id) return NotFound();
            if (ModelState.IsValid)
            {
                _context.Update(level);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(level);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var level = await _context.Levels.FindAsync(id);
            if (level == null) return NotFound();
            return View(level);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var level = await _context.Levels.FindAsync(id);
            if (level != null)
            {
                _context.Levels.Remove(level);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}