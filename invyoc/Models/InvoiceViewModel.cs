using invyoc.Extensions;
using invyoc.Models.Enums;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace invyoc.Models;

public class InvoiceViewModel
{
    public InvoiceViewModel()
    {
        InvoiceNumber = "INV01";
        Currency = CurrencyType.INR.ToString();
        DueDate = DateTime.Now.AddDays(15);
        InvoiceDate = DateTime.Now;
        PaymentNotes = "It was great doing business with you.";
        PaymentTerms = "Please make the payment by the due date.";
    }

    [Required(ErrorMessage = "Invoice Number is required")]
    public string InvoiceNumber { get; set; } = string.Empty;

    [Required(ErrorMessage = "Invoice Date is required")]
    public DateTime InvoiceDate { get; set; }

    public string? PaymentTerms { get; set; }

    public string? PaymentNotes { get; set; }

    [Required(ErrorMessage = "Due Date is required")]
    public DateTime DueDate { get; set; }

    public string? PONumber { get; set; }

    public CompanyInfo Company { get; set; } = new();
    public ClientInfo BillTo { get; set; } = new();
    public AddressInfo ShipTo { get; set; } = new();

    public List<InvoiceItemViewModel> Items { get; set; } = [];

    public string Currency { get; set; } = string.Empty;

    public decimal Subtotal => Items.Sum(i => i.Amount).ToFormat();

    public List<SelectListItem> Currencies = PrimitiveTypeExtensions.ToSelectList<CurrencyType>();
}