using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BudgetApp.Models;

namespace BudgetApp.Controllers
{
    public class CategoryMappingsController : Controller
    {
        private readonly BudgetAppContext _context;

        public CategoryMappingsController(BudgetAppContext context)
        {
            _context = context;
        }

        // GET: CategoryMappings
        public async Task<IActionResult> Index()
        {
            var budgetAppContext = _context.CategoryMappings.Include(c => c.CategoryNameNavigation);
            return View(await budgetAppContext.ToListAsync());
        }

        // GET: CategoryMappings/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var categoryMapping = await _context.CategoryMappings
                .Include(c => c.CategoryNameNavigation)
                .FirstOrDefaultAsync(m => m.Cmid == id);
            if (categoryMapping == null)
            {
                return NotFound();
            }

            return View(categoryMapping);
        }

        // GET: CategoryMappings/Create
        public IActionResult Create()
        {
            ViewData["CategoryName"] = new SelectList(_context.BudgetCategories, "CategoryName", "CategoryName");
            return View();
        }

        // POST: CategoryMappings/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Cmid,Keyword,CategoryName")] CategoryMapping categoryMapping)
        {
            if (ModelState.IsValid)
            {
                _context.Add(categoryMapping);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CategoryName"] = new SelectList(_context.BudgetCategories, "CategoryName", "CategoryName", categoryMapping.CategoryName);
            return View(categoryMapping);
        }

        // GET: CategoryMappings/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var categoryMapping = await _context.CategoryMappings.FindAsync(id);
            if (categoryMapping == null)
            {
                return NotFound();
            }
            ViewData["CategoryName"] = new SelectList(_context.BudgetCategories, "CategoryName", "CategoryName", categoryMapping.CategoryName);
            return View(categoryMapping);
        }

        // POST: CategoryMappings/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Cmid,Keyword,CategoryName")] CategoryMapping categoryMapping)
        {
            if (id != categoryMapping.Cmid)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(categoryMapping);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CategoryMappingExists(categoryMapping.Cmid))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["CategoryName"] = new SelectList(_context.BudgetCategories, "CategoryName", "CategoryName", categoryMapping.CategoryName);
            return View(categoryMapping);
        }

        // GET: CategoryMappings/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var categoryMapping = await _context.CategoryMappings
                .Include(c => c.CategoryNameNavigation)
                .FirstOrDefaultAsync(m => m.Cmid == id);
            if (categoryMapping == null)
            {
                return NotFound();
            }

            return View(categoryMapping);
        }

        // POST: CategoryMappings/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var categoryMapping = await _context.CategoryMappings.FindAsync(id);
            if (categoryMapping != null)
            {
                _context.CategoryMappings.Remove(categoryMapping);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CategoryMappingExists(int id)
        {
            return _context.CategoryMappings.Any(e => e.Cmid == id);
        }
    }
}
