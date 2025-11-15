using invyoc.Extensions;
using invyoc.Models.Enums;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace invyoc.Models;

public class InvoiceViewModel
{
    public InvoiceViewModel()
    {
        InvoiceNumber = "INV01";
        Currency = CurrencyType.INR.ToString();
        DueDate = DateTime.Now.AddDays(15);
        InvoiceDate = DateTime.Now;
    }

    public string InvoiceNumber { get; set; } = string.Empty;
    public DateTime InvoiceDate { get; set; }
    public string? PaymentTerms { get; set; }
    public DateTime DueDate { get; set; }
    public string? PONumber { get; set; }

    public CompanyInfo Company { get; set; } = new();
    public ClientInfo BillTo { get; set; } = new();
    public ClientInfo ShipTo { get; set; } = new();

    public List<InvoiceItemViewModel> Items { get; set; } = [];

    public decimal DiscountPercentage { get; set; }
    public decimal TaxPercentage { get; set; }

    public string Currency { get; set; } = string.Empty;

    public decimal Subtotal => Items.Sum(i => i.Amount).ToFormat();
    public decimal Discount => (Subtotal * (DiscountPercentage / 100)).ToFormat();
    public decimal Tax => ((Subtotal - Discount) * (TaxPercentage / 100)).ToFormat();
    public decimal Total => (Subtotal - Discount + Tax).ToFormat();

    public List<SelectListItem> Currencies = PrimitiveTypeExtensions.ToSelectList<CurrencyType>();

    public string? PaymentNotes { get; set; }

    public static InvoiceViewModel GetTempData()
    {
        var tempObj = new InvoiceViewModel
        {
            Company = new CompanyInfo()
            {
                Name = "Invoice From",
                Address = "Invoice from company full address",
                Phone = "+91 98765 43210",
                Email = "contact@billfrom.com",
                GSTNo = "Tax Identification Number (TIN)"
            },

            BillTo = new ClientInfo()
            {
                Name = "Bill To Company Name",
                Address = "Bill To full address + other info",
                ContactNum = "+91 2222 1111"
            },

            ShipTo = new ClientInfo()
            {
                Name = "Ship To Company Name",
                Address = "Ship To full address + other info",
                ContactNum = "+91 2222 1111"
            },

            InvoiceNumber = "0001",
            InvoiceDate = DateTime.Now.Date,
            PaymentTerms = "Net 30 Days",
            DueDate = DateTime.Now.AddDays(30).Date,
            PONumber = $"PO-1234",

            Currency = PrimitiveTypeExtensions.GetEnumDescription(CurrencyType.INR),
            Currencies = PrimitiveTypeExtensions.ToSelectList<CurrencyType>(),

            Items =
            [
                new() { LineNumber = 1, Description = "Item 1", Quantity = 1, Rate = 100 },
            ],

            DiscountPercentage = 0,
            TaxPercentage = 0,

            PaymentNotes = "Thank you for doing business with us.\r\nPayment terms: to be received within 30 days."
        };

        return tempObj;
    }

}