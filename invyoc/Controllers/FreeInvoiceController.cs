using invyoc.Extensions;
using invyoc.Models;
using Microsoft.AspNetCore.Mvc;

namespace invyoc.Controllers;

[Route("free-invoice")]
public class FreeInvoiceController : Controller
{
    private readonly IWebHostEnvironment _env;

    public FreeInvoiceController(IWebHostEnvironment env) => _env = env;

    public IActionResult Index()
    {
        return View(InvoiceViewModel.GetTempData());
    }

    [HttpPost]
    public IActionResult Download(InvoiceViewModel invoiceVM)
    {
        string invoiceTemplatePath = Path.Combine(_env.WebRootPath, "free-invoice.html");
        string newInvoicePath = Path.Combine(_env.WebRootPath, "exports");
        var newInvoiceFileName = PrimitiveTypeExtensions.MakeValidFileName(invoiceVM.Company.Name + "_Invoice_" + invoiceVM.InvoiceNumber + ".pdf");

        ExportExtensions.ConvertToPDF(invoiceVM, invoiceTemplatePath, newInvoicePath, newInvoiceFileName);

        var fileBytes = System.IO.File.ReadAllBytes(Path.Combine(newInvoicePath, newInvoiceFileName));
        return File(fileBytes, "application/pdf", newInvoiceFileName);
    }
}