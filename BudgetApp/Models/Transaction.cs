using System;
using System.Collections.Generic;

namespace BudgetApp.Models;

public partial class Transaction
{
    public int TransId { get; set; }

    public string TransName { get; set; } = null!;

    public decimal Amount { get; set; }

    public string? CategoryName { get; set; }

    public virtual BudgetCategory? CategoryNameNavigation { get; set; }

    public DateOnly? TransDate { get; set; }
}
