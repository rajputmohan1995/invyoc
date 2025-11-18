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

    public static InvoiceViewModel GetTempData()
    {
        var tempObj = new InvoiceViewModel
        {
            Company = new CompanyInfo()
            {
                Name = "MU Core Private Limited",
                CompanyAddress = new()
                {
                    Address = "103, Shivalay Shivsukhnagar Society, Vastral Road",
                    City = "Ahmedabad",
                    State = "GJ - 382418",
                    Country = "India",
                    ContactNum = "+91-7567087674"
                },
                Email = "contact@mucore.com",
                GSTNo = "24AAACH7409R2Z6"
            },

            BillTo = new ClientInfo()
            {
                Name = "55 Technologies Private Limited",
                ClientAddress = new()
                {
                    Address = "J-2, Jhalana Institutional Area, Jhalana Doongri",
                    City = "Jaipur",
                    State = "RJ - 302004",
                    Country = "India",
                    ContactNum = "+91-97733 85303"
                },
                GSTNo = "08AAACH7409R1Z1"
            },

            ShipTo = new()
            {
                Address = "J-2, Jhalana Institutional Area, Jhalana Doongri",
                City = "Jaipur",
                State = "RJ - 302004",
                Country = "India",
                ContactNum = "+91-97733 85303"
            },

            InvoiceNumber = "INV001",
            InvoiceDate = DateTime.Now.Date,
            DueDate = DateTime.Now.AddDays(30).Date,
            PONumber = $"PO-1234",

            Currency = PrimitiveTypeExtensions.GetEnumDescription(CurrencyType.INR),
            Currencies = PrimitiveTypeExtensions.ToSelectList<CurrencyType>(),

            Items =
            [
                new()
                {
                    LineNumber = 1,
                    Description = "Item 1",
                    HSN_SAC = "03031400",
                    Quantity = 10,
                    Rate = 1000
                },
                new()
                {
                    LineNumber = 2,
                    Description = "Item 2",
                    HSN_SAC = "05011401",
                    Quantity = 2,
                    Rate = 5000,
                    SGST = 2.5m,
                    CGST = 2.5m,
                    Cess = 2
                },
                new()
                {
                    LineNumber = 3,
                    Description = "Item 3",
                    HSN_SAC = "09902398",
                    Quantity = 1,
                    Rate = 10000,
                    SGST = 2.5m,
                    CGST = 2.5m,
                    Cess = 2
                }
            ],

            PaymentNotes = "It was great doing business with you.",
            PaymentTerms = "Please make the payment by the due date."
        };

        return tempObj;
    }
}