﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace Repositories.Models;

public partial class Expense
{
    public Guid ExpensesId { get; set; }

    public decimal Amount { get; set; }

    public string Description { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string Type { get; set; }

    public string Frequency { get; set; }

    public DateOnly? NextDueDate { get; set; }

    public Guid? ExCid { get; set; }

    public Guid? AccountId { get; set; }

    public Guid? UserId { get; set; }

    public virtual FinancialAccount Account { get; set; }

    public virtual ExpensesCategory ExC { get; set; }

    public virtual User User { get; set; }
}