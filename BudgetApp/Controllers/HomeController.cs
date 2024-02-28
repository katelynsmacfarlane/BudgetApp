using BudgetApp.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace BudgetApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            BudgetCategory budgCatIns = new BudgetCategory
            {
                CategoryName = "Fast Food",
                Amount = (decimal)100.00
            };

            using (BudgetAppContext dc = new BudgetAppContext())
            {

                //dc.BudgetCategories.Add(budgCatIns);
                //dc.SaveChanges();

                List<BudgetCategory> budgetCats = dc.BudgetCategories.ToList();
            }

            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
