using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Game.Data;
using Game.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Game.Controllers
{
    [Authorize(Roles = "Admin")]
    public class CharactersController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CharactersController(ApplicationDbContext context)
        {
            _context = context;
        }

        [AllowAnonymous]
        public async Task<IActionResult> Index(string searchString, string sortOrder)
        {
            ViewData["NameSortParam"] = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewData["LevelSortParam"] = sortOrder == "level" ? "level_desc" : "level";
            ViewData["CurrentFilter"] = searchString;

            var characters = from c in _context.Characters
                             .Include(c => c.Level)
                             .Include(c => c.Player) // Добавляем загрузку данных игрока
                             select c;

            if (!String.IsNullOrEmpty(searchString))
            {
                characters = characters.Where(c => c.Name.Contains(searchString));
            }

            switch (sortOrder)
            {
                case "name_desc":
                    characters = characters.OrderByDescending(c => c.Name);
                    break;
                case "level":
                    characters = characters.OrderBy(c => c.Level.Name); // Сортировка по имени уровня
                    break;
                case "level_desc":
                    characters = characters.OrderByDescending(c => c.Level.Name); // Сортировка по имени уровня
                    break;
                default:
                    characters = characters.OrderBy(c => c.Name);
                    break;
            }

            return View(await characters.ToListAsync());
        }

        public IActionResult Create()
        {
            ViewBag.Players = new SelectList(_context.Players, "Id", "Name");
            ViewBag.Levels = new SelectList(_context.Levels, "Id", "Name");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Character character)
        {
           // if (ModelState.IsValid)
           // {
                _context.Add(character);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
           // }
            ViewBag.Players = new SelectList(_context.Players, "Id", "Name", character.PlayerId);
            ViewBag.Levels = new SelectList(_context.Levels, "Id", "Name", character.LevelId);
            return View(character);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var character = await _context.Characters.FindAsync(id);
            if (character == null) return NotFound();
            ViewBag.Players = new SelectList(_context.Players, "Id", "Name", character.PlayerId);
            ViewBag.Levels = new SelectList(_context.Levels, "Id", "Name", character.LevelId);
            return View(character);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Character character)
        {
            if (id != character.Id) return NotFound();
            if (ModelState.IsValid)
            {
                _context.Update(character);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewBag.Players = new SelectList(_context.Players, "Id", "Name", character.PlayerId);
            ViewBag.Levels = new SelectList(_context.Levels, "Id", "Name", character.LevelId);
            return View(character);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var character = await _context.Characters
                .Include(c => c.Player)
                .Include(c => c.Level)
                .FirstOrDefaultAsync(c => c.Id == id);
            if (character == null) return NotFound();
            return View(character);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var character = await _context.Characters.FindAsync(id);
            if (character != null)
            {
                _context.Characters.Remove(character);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}