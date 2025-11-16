using invyoc.Extensions;

namespace invyoc.Models;

public class InvoiceItemViewModel
{
    public int LineNumber { get; set; }
    public string? Description { get; set; }
    public string? HSN_SAC { get; set; }
    public int Quantity { get; set; }
    public decimal Rate { get; set; }
    public decimal SGST { get; set; }
    public decimal CGST { get; set; }
    public decimal Cess { get; set; }

    public decimal Amount => (Quantity * Rate).ToFormat();
}