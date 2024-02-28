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
    public class BudgetCategoriesController : Controller
    {
        private readonly BudgetAppContext _context;

        public BudgetCategoriesController(BudgetAppContext context)
        {
            _context = context;
        }

        // GET: BudgetCategories
        public async Task<IActionResult> Index()
        {
            return View(await _context.BudgetCategories.ToListAsync());
        }

        // GET: BudgetCategories/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var budgetCategory = await _context.BudgetCategories
                .FirstOrDefaultAsync(m => m.CategoryName == id);
            if (budgetCategory == null)
            {
                return NotFound();
            }

            return View(budgetCategory);
        }

        // GET: BudgetCategories/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: BudgetCategories/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CategoryName,Amount")] BudgetCategory budgetCategory)
        {
            if (ModelState.IsValid)
            {
                _context.Add(budgetCategory);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(budgetCategory);
        }

        // GET: BudgetCategories/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var budgetCategory = await _context.BudgetCategories.FindAsync(id);
            if (budgetCategory == null)
            {
                return NotFound();
            }
            return View(budgetCategory);
        }

        // POST: BudgetCategories/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("CategoryName,Amount")] BudgetCategory budgetCategory)
        {
            if (id != budgetCategory.CategoryName)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(budgetCategory);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BudgetCategoryExists(budgetCategory.CategoryName))
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
            return View(budgetCategory);
        }

        // GET: BudgetCategories/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var budgetCategory = await _context.BudgetCategories
                .FirstOrDefaultAsync(m => m.CategoryName == id);
            if (budgetCategory == null)
            {
                return NotFound();
            }

            return View(budgetCategory);
        }

        // POST: BudgetCategories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var budgetCategory = await _context.BudgetCategories.FindAsync(id);
            if (budgetCategory != null)
            {
                _context.BudgetCategories.Remove(budgetCategory);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BudgetCategoryExists(string id)
        {
            return _context.BudgetCategories.Any(e => e.CategoryName == id);
        }
    }
}
