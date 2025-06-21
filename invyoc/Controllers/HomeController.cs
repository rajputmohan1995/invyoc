using invyoc.Extensions;
using invyoc.Models;
using Microsoft.AspNetCore.Mvc;

namespace invyoc.Controllers;

public class HomeController : Controller
{
    private readonly IWebHostEnvironment _env;
    private readonly PdfService _pdfService;

    public HomeController(IWebHostEnvironment env, PdfService pdfService)
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
        try
        {
            var newInvoiceFileName = PrimitiveTypeExtensions.MakeValidFileName(invoiceVM.Company.Name + "_Invoice_" + invoiceVM.InvoiceNumber + ".pdf");
            string invoiceTemplatePath = Path.Combine(_env.WebRootPath, "invoice-template.html");

            var pdfBytes = _pdfService.GeneratePdf(ExportExtensions.GetHtmlContent(invoiceVM, invoiceTemplatePath));

            ViewBag.IsDownloadSuccess = true;

            return File(pdfBytes, "application/pdf", newInvoiceFileName);
        }
        catch (Exception)
        {
            throw;
        }
    }


    #region Static Pages
    public IActionResult About()
    {
        return View();
    }

    public IActionResult PrivacyPolicy()
    {
        return View();
    }

    public IActionResult Terms()
    {
        return View();
    }
    #endregion
}