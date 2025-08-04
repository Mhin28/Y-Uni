using System;
using System.Collections.Generic;
using Repositories.Models;

namespace Repositories.ViewModels.BudgetModel
{
    /// <summary>
    /// DTO for budget carry-over summary between months
    /// Supports the frontend lock feature for budget amounts
    /// </summary>
    public class BudgetCarryOverSummaryDto
    {
        public Guid UserId { get; set; }
        public int FromYear { get; set; }
        public int FromMonth { get; set; }
        public int ToYear { get; set; }
        public int ToMonth { get; set; }
        public List<BudgetCarryOverItemDto> AvailableBudgets { get; set; } = new List<BudgetCarryOverItemDto>();
        public int TotalAvailableBudgets { get; set; }
        public int TotalCarriedOverBudgets { get; set; }
        public decimal TotalCarryOverAmount { get; set; }
    }

    /// <summary>
    /// Individual budget item for carry-over
    /// </summary>
    public class BudgetCarryOverItemDto
    {
        public Guid BudgetId { get; set; }
        public Guid CategoryId { get; set; }
        public string CategoryName { get; set; }
        public decimal BudgetAmount { get; set; }
        public bool IsAlreadyCarriedOver { get; set; }
        public bool IsLocked { get; set; } // Frontend can set this for UI state
    }

    /// <summary>
    /// Request model for bulk budget carry-over
    /// </summary>
    public class BudgetCarryOverRequestDto
    {
        public List<Guid> BudgetIds { get; set; } = new List<Guid>();
        public int TargetYear { get; set; }
        public int TargetMonth { get; set; }
        public bool OverwriteExisting { get; set; } = false;
    }

    /// <summary>
    /// Response model for budget carry-over operation
    /// </summary>
    public class BudgetCarryOverResponseDto
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
        public int SuccessfulCopies { get; set; }
        public int SkippedCopies { get; set; }
        public List<string> Errors { get; set; } = new List<string>();
        public List<Budget> CreatedBudgets { get; set; } = new List<Budget>();
    }
}