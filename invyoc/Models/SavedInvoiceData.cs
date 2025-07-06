namespace invyoc.Models;

public class SavedInvoiceData
{
    public int Id { get; set; }
    public InvoiceViewModel? InvoiceVM { get; set; }
    public DateTime Timestamp { get; set; }
}
