using System;
using System.Collections.Generic;

namespace BudgetApp.Models;

public partial class CategoryMapping
{
    public int Cmid { get; set; }

    public string Keyword { get; set; } = null!;

    public string CategoryName { get; set; } = null!;

    public virtual BudgetCategory CategoryNameNavigation { get; set; } = null!;
}
