using invyoc.Extensions;
using invyoc.Models;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace invyoc.Controllers;

public class HomeController : Controller
{
    private readonly IWebHostEnvironment _env;
    private readonly PdfService _pdfService;
    private const string _lastSavedInvoiceCookieName = "FreeInvoiceLastSavedInvoice";

    public HomeController(IWebHostEnvironment env, PdfService pdfService)
    {
        _env = env;
        _pdfService = pdfService;
    }

    [Route("")]
    public IActionResult Index()
    {
        var invoiceVM = InvoiceViewModel.GetTempData();
        var lastSavedInvoice = Request.Cookies[_lastSavedInvoiceCookieName];

        if (!string.IsNullOrWhiteSpace(lastSavedInvoice))
            invoiceVM = JsonSerializer.Deserialize<InvoiceViewModel>(lastSavedInvoice);

        return View(invoiceVM);
    }

    [HttpPost]
    public IActionResult Download(InvoiceViewModel invoiceVM)
    {
        try
        {
            string savedInvoicePath = Path.Combine(_env.WebRootPath, $"invoiceJson\\invoices_{DateTime.UtcNow:MMM-yyyy}.json");
            PrimitiveTypeExtensions.AppendJsonObjectToFile(savedInvoicePath, new SavedInvoiceData() { InvoiceVM = invoiceVM });

            var newInvoiceFileName = PrimitiveTypeExtensions.MakeValidFileName(invoiceVM.Company.Name + "_Invoice_" + invoiceVM.InvoiceNumber + ".pdf");
            string invoiceTemplatePath = Path.Combine(_env.WebRootPath, "invoice-template.html");

            var pdfBytes = _pdfService.GeneratePdf(ExportExtensions.GetHtmlContent(invoiceVM, invoiceTemplatePath));

            ViewBag.IsDownloadSuccess = true;
            var invoiceLineNum = 0;
            invoiceVM.Items.ForEach(i => i.LineNumber = ++invoiceLineNum);
            Response.Cookies.Append(_lastSavedInvoiceCookieName, JsonSerializer.Serialize(invoiceVM), new() { Expires = DateTime.Now.AddDays(60) });

            return File(pdfBytes, "application/pdf", newInvoiceFileName);
        }
        catch (Exception)
        {
            throw;
        }
    }


    #region Static Pages

    [Route("about")]
    public IActionResult About()
    {
        return View();
    }

    [Route("privacy-policy")]
    public IActionResult PrivacyPolicy()
    {
        return View();
    }

    [Route("terms")]
    public IActionResult Terms()
    {
        return View();
    }

    #endregion



    [Route("SavedInfo")]
    public IActionResult SavedInfo(string month, string year, string p)
    {
        if (p != "Lakshu@2022#")
            return NotFound();

        ViewBag.SavedInfoMonthYear = $"{month}-{year}";
        string savedInvoicePath = Path.Combine(_env.WebRootPath, $"invoiceJson\\invoices_{month}-{year}.json");
        var saveInvoices = PrimitiveTypeExtensions.GetAllContentFromJsonFile(savedInvoicePath);
        return View(saveInvoices);
    }

    [Route("shared-invoice-details")]
    public IActionResult InvoiceDetails(string invoiceData)
    {
        if (string.IsNullOrWhiteSpace(invoiceData))
            return RedirectToAction("Index");

        try
        {
            Response.Cookies.Append(_lastSavedInvoiceCookieName, invoiceData, new() { Expires = DateTime.Now.AddDays(60) });
            return RedirectToAction("Index");
        }
        catch
        {
            return RedirectToAction("Index");
        }
    }
}