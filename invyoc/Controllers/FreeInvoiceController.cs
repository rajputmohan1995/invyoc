using invyoc.Extensions;
using invyoc.Models;
using Microsoft.AspNetCore.Mvc;

namespace invyoc.Controllers;

//[Route("free-invoice")]
public class FreeInvoiceController : Controller
{
    private readonly IWebHostEnvironment _env;
    private readonly PdfService _pdfService;

    public FreeInvoiceController(IWebHostEnvironment env, PdfService pdfService)
    {
        _env = env;
        _pdfService = pdfService;
    }

    public IActionResult Index()
    {
        return View(InvoiceViewModel.GetTempData());
    }

    [HttpPost]
    public IActionResult Download(InvoiceViewModel invoiceVM)
    {
        var newInvoiceFileName = PrimitiveTypeExtensions.MakeValidFileName(invoiceVM.Company.Name + "_Invoice_" + invoiceVM.InvoiceNumber + ".pdf");
        string invoiceTemplatePath = Path.Combine(_env.WebRootPath, "invoice-template.html");

        var pdfBytes = _pdfService.GeneratePdf(ExportExtensions.GetHtmlContent(invoiceVM, invoiceTemplatePath));

        return File(pdfBytes, "application/pdf", newInvoiceFileName);
    }
}