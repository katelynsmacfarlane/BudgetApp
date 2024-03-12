using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BudgetApp.Models;
using IronPdf.Pages;
using PdfSharp.Pdf.Content.Objects;

namespace BudgetApp.Controllers
{
    public class TransactionsController : Controller
    {
        private readonly BudgetAppContext _context;
        public string FileNameOnServer { get; set; }
        public long FileContentLength { get; set; }
        public string FileContentType { get; set; }

        public TransactionsController(BudgetAppContext context, ILogger<HomeController> logger)
        {
            _context = context;
            FileNameOnServer = string.Empty;
            FileContentLength = 0;
            FileContentType = string.Empty;
        }

        // GET: Transactions
        public async Task<IActionResult> Index()
        {
            var budgetAppContext = _context.Transactions.Include(t => t.CategoryNameNavigation);
            return View(await budgetAppContext.ToListAsync());
        }

        // GET: Transactions/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var transaction = await _context.Transactions
                .Include(t => t.CategoryNameNavigation)
                .FirstOrDefaultAsync(m => m.TransId == id);
            if (transaction == null)
            {
                return NotFound();
            }

            return View(transaction);
        }

        // GET: Transactions/Create
        public IActionResult Create()
        {
            ViewData["CategoryName"] = new SelectList(_context.BudgetCategories, "CategoryName", "CategoryName");
            return View();
        }

        // POST: Transactions/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("TransId,TransName,Amount,CategoryName")] Transaction transaction)
        {
            if (ModelState.IsValid)
            {
                _context.Add(transaction);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CategoryName"] = new SelectList(_context.BudgetCategories, "CategoryName", "CategoryName", transaction.CategoryName);
            return View(transaction);
        }

        // GET: Transactions/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var transaction = await _context.Transactions.FindAsync(id);
            if (transaction == null)
            {
                return NotFound();
            }
            ViewData["CategoryName"] = new SelectList(_context.BudgetCategories, "CategoryName", "CategoryName", transaction.CategoryName);
            return View(transaction);
        }

        // POST: Transactions/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("TransId,TransName,Amount,CategoryName")] Transaction transaction)
        {
            if (id != transaction.TransId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(transaction);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TransactionExists(transaction.TransId))
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
            ViewData["CategoryName"] = new SelectList(_context.BudgetCategories, "CategoryName", "CategoryName", transaction.CategoryName);
            return View(transaction);
        }

        // GET: Transactions/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var transaction = await _context.Transactions
                .Include(t => t.CategoryNameNavigation)
                .FirstOrDefaultAsync(m => m.TransId == id);
            if (transaction == null)
            {
                return NotFound();
            }

            return View(transaction);
        }

        // POST: Transactions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var transaction = await _context.Transactions.FindAsync(id);
            if (transaction != null)
            {
                _context.Transactions.Remove(transaction);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }


        // GET: Transactions/Delete/5
        public async Task<IActionResult> FileUpload()
        {           
            return View();
        }

        // POST: Transactions/Delete/5
        [HttpPost, ActionName("FileUpload")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> FileUpload(IFormFile TransFile)
        {
            // User selected a file
            // Get a temporary path
            FileNameOnServer = Path.GetTempPath();
            // Add the file name to the path
            FileNameOnServer += TransFile.FileName;
            // Get the file's length
            FileContentLength = TransFile.Length;
            // Get the file's type
            FileContentType = TransFile.ContentType;

            // Create a stream to write the file to
            using var stream = System.IO.File.Create(FileNameOnServer);
            // Upload file and copy to the stream
            TransFile.CopyTo(stream);
            stream.Close();
            List<Transaction> transList = ReadPDF(FileNameOnServer);

            // Save to database
            foreach (Transaction trans in transList)
            {
                if (ModelState.IsValid)
                {
                    trans.CategoryName = LookupCategory(trans.TransName);
                    if (!TransactionExists(trans.TransName, trans.TransDate))
                    {
                        _context.Add(trans);
                    }
                }
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public List<Transaction> ReadPDF(string fileName)
        {
            //Get all text from file
            using PdfDocument PDF = PdfDocument.FromFile(fileName);
            string AllText = PDF.ExtractAllText();

            List<string> lineArr = AllText.Split("\n").ToList();
            int transStartIndex = lineArr.IndexOf("Transaction Merchant Name or Transaction Description $ Amount\r");
            List<string> transList = lineArr.Skip(transStartIndex + 1).ToList();
            List<Transaction> negatives = new List<Transaction>();
            List<string> positives = new List<string>();

            // Iterate through each line in PDF for transaction details
            string transLine = "";
            int transIndex = -1;
            while (!transLine.ToUpper().Contains("PLAN FEE") 
                && !transLine.ToUpper().Contains("PURCHASE INTEREST"))
            {
                if (!transLine.ToUpper().Contains("THANK YOU"))
                {
                    DateOnly transDate = default;
                    string transDateString = "";
                    int transDateIndex = 0;

                    Decimal transAmount = 0;                   
                    string transAmountString = "";
                    int transAmountIndex = 0;
                    
                    List<string> lineItems = transLine.Split(" ").ToList();
                    foreach (string lineItem in lineItems)
                    {
                        // Look for transaction date in line
                        if (transDate == default && DateOnly.TryParse(lineItem, out transDate) && lineItem.Contains("/"))
                        {
                            transDateString = lineItem;
                        }
                        // Look for dollar amount in line
                        if (Decimal.TryParse(lineItem, out transAmount) && lineItem.Contains("."))
                        {
                            transAmountString = lineItem;
                            break;
                        }
                    }

                    if (transAmount != 0)
                    {
                        Transaction newTrans = new Transaction();

                        // Date is first thing in row line and amount is the last thing
                        transDateIndex = transLine.IndexOf(transDateString);
                        transAmountIndex = transLine.IndexOf(transAmountString);
                        int transDateLength = transDateString.Length + 1;

                        newTrans.TransName = transLine
                            .Substring((transDateIndex + transDateLength), transAmountIndex - transDateLength)
                            .Trim();
                        newTrans.Amount = transAmount * -1; // Assume all transactions are negative for now
                        newTrans.TransDate = transDate;
                        negatives.Add(newTrans);
                    }
                }

                transIndex++;
                transLine = transList[transIndex];
            }

            return negatives;
        }

        public string LookupCategory(string transName)
        {
            CategoryMapping? cm = _context.CategoryMappings.
                FirstOrDefault(x => transName.ToLower().Contains(x.Keyword.ToLower()));

            if (cm != null)
            {
                return cm.CategoryName;
            }
            else
            {
                return "Miscellaneous";
            }
        }

        private bool TransactionExists(int id)
        {
            return _context.Transactions.Any(e => e.TransId == id);
        }
        
        private bool TransactionExists(string transName, DateOnly? transDate)
        {
            if (transDate == null)
            {
                return false;
            }
            return _context.Transactions.Any(e => e.TransName == transName 
                        && e.TransDate == transDate);
        }
    }
}
