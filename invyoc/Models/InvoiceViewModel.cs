using invyoc.Extensions;
using invyoc.Models.Enums;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace invyoc.Models
{
    public class InvoiceViewModel
    {
        public string InvoiceNumber { get; set; }
        public DateTime InvoiceDate { get; set; }
        public string PaymentTerms { get; set; }
        public DateTime DueDate { get; set; }
        public string PONumber { get; set; }

        public CompanyInfo Company { get; set; } = new();
        public ClientInfo BillTo { get; set; } = new();
        public ClientInfo ShipTo { get; set; } = new();

        public List<InvoiceItemViewModel> Items { get; set; } = [];

        public decimal DiscountPercentage { get; set; }
        public decimal TaxPercentage { get; set; }

        public string Currency { get; set; }

        public decimal Subtotal => Items.Sum(i => i.Amount).ToFormat();
        public decimal Discount => (Subtotal * (DiscountPercentage / 100)).ToFormat();
        public decimal Tax => ((Subtotal - Discount) * (TaxPercentage / 100)).ToFormat();
        public decimal Total => (Subtotal - Discount + Tax).ToFormat();

        public List<SelectListItem> Currencies = PrimitiveTypeExtensions.ToSelectList<CurrencyType>();

        public string PaymentNotes { get; set; }

        public static InvoiceViewModel GetTempData()
        {
            var tempObj = new InvoiceViewModel
            {
                Company = new CompanyInfo()
                {
                    Name = "Swift Solutions Pvt. Ltd.",
                    Address = "22/B Industrial Estate, Whitefield, Bengaluru - 560066",
                    Phone = "+91 98765 43210",
                    Email = "contact@swiftsolutions.in",
                    GSTNo = "24AAACH7409R2Z6"
                },

                BillTo = new ClientInfo()
                {
                    Name = "Arjun Verma",
                    Address = "502, Green Heights Apartments, Mumbai - 400076",
                },

                ShipTo = new ClientInfo()
                {
                    Name = "Arjun Verma",
                    Address = "502, Green Heights Apartments, Mumbai - 400076",
                },

                InvoiceNumber = "0001",
                InvoiceDate = DateTime.Now,
                PaymentTerms = "Net 30",
                DueDate = DateTime.Now.AddDays(30),
                PONumber = $"PO-{(new Random()).Next(111, 999)}",

                Currency = PrimitiveTypeExtensions.GetEnumDescription(CurrencyType.INR),
                Currencies = PrimitiveTypeExtensions.ToSelectList<CurrencyType>(),

                Items =
                [
                    new() { LineNumber = 1, Description = "Website Design", Quantity = 1, Rate = 25000 },
                    new() { LineNumber = 2, Description = "Monthly Hosting", Quantity = 12, Rate = 500 },
                    new() { LineNumber = 3, Description = "Domain Renewal", Quantity = 3, Rate = 1500 },
                ],

                DiscountPercentage = 0,
                TaxPercentage = 0,

                PaymentNotes = "Thank you for doing business with us.\r\nPayment terms: to be received within 30 days."
            };

            return tempObj;
        }
    }
}
