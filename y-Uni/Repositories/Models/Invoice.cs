﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace Repositories.Models;

public partial class Invoice
{
    public Guid InvoiceId { get; set; }

    public decimal Amount { get; set; }

    public decimal? TaxAmount { get; set; }

    public decimal? DiscountAmount { get; set; }

    public decimal? TotalAmount { get; set; }

    public Guid? PaymentMethodId { get; set; }

    public string GatewayTransactionId { get; set; }

    public DateTime? CreatedDate { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public string InvoiceStatus { get; set; }

    public Guid? UserId { get; set; }

    public Guid? DiscountId { get; set; }

    public Guid? MembershipPlanId { get; set; }

    public virtual Discount Discount { get; set; }

    public virtual MembershipPlan MembershipPlan { get; set; }

    public virtual PaymentMethod PaymentMethod { get; set; }

    public virtual User User { get; set; }
}