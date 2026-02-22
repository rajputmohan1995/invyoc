using invyoc.Models;

namespace invyoc.Services;

public interface IInvoiceService
{
    public Task SaveAsync(InvoiceViewModel invoice);
    Task<List<SavedInvoiceData>> GetAllAsync(CommonListVM commonListVM);
    Task<SavedInvoiceData> GetByIdAsync(int id);
}