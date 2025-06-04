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
        var result = ExportExtensions.ConvertToPDF(invoiceVM, _env.WebRootPath);

        var fileBytes = System.IO.File.ReadAllBytes(result.Item1);
        return File(fileBytes, "application/pdf", result.Item2);
    }
}