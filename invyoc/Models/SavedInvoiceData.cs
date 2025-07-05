namespace invyoc.Models;

public class SavedInvoiceData
{
    public int Id { get; set; }
    public InvoiceViewModel InvoiceVM { get; set; }
    public string Timestamp { get; set; }
}
