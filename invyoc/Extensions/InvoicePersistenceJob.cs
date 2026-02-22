using invyoc.Models;
using invyoc.Services;

namespace invyoc.Extensions;

public class InvoicePersistenceJob(IInvoiceService invoiceService)
{
    public async Task SaveInvoiceAsync(InvoiceViewModel invoice)
    {
        if (invoice == null)
            throw new ArgumentNullException(nameof(invoice));

        await invoiceService.SaveAsync(invoice);
    }
}