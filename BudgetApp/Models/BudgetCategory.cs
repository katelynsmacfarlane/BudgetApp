using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace BudgetApp.Models;

public partial class BudgetCategory
{
    [DisplayName("Budget Category")]
    [Required]
    public string CategoryName { get; set; } = null!;

    [DisplayName("Budget Amount")]
    [Required]
    public decimal Amount { get; set; }

    public virtual ICollection<CategoryMapping> CategoryMappings { get; set; } = new List<CategoryMapping>();

    public virtual ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
}
