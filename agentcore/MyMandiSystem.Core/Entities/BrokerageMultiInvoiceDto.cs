using System;

namespace MyMandiSystem.Core.Entities;

public class BrokerageMultiInvoiceSummaryDto
{
    public string MultiId { get; set; } = string.Empty;
    public string PurchaseId { get; set; } = string.Empty;
    public string SaleId { get; set; } = string.Empty;
    public DateTime Date { get; set; } = DateTime.Today;
    public string SupplierName { get; set; } = string.Empty;
    public string CustomerName { get; set; } = string.Empty;
    public decimal NetPurchase { get; set; } = 0;
    public decimal NetSale { get; set; } = 0;
    public decimal ProfitLoss { get; set; } = 0;
    public string PlsAccount { get; set; } = "40001";
}

