using invyoc.Extensions;
using invyoc.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using System.Text.Json;

namespace invyoc.Controllers;

public class HomeController : Controller
{
    private readonly IWebHostEnvironment _env;
    private readonly PdfService _pdfService;
    private const string _lastSavedInvoiceCookieName = "FreeInvoiceLastSavedInvoice";
    private const string _lastSavedInvoiceLogo = "FreeInvoiceLastSavedInvoiceLogo";

    public HomeController(IWebHostEnvironment env, PdfService pdfService)
    {
        _env = env;
        _pdfService = pdfService;
    }

    [HttpGet]
    public IActionResult Index()
    {
        var invoiceVM = InvoiceViewModel.GetTempData();
        //InvoiceViewModel invoiceVM = new();
        return View(invoiceVM);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Index(InvoiceViewModel invoiceVM)
    {
        try
        {
            ArgumentNullException.ThrowIfNull(invoiceVM);

            if (!ModelState.IsValid)
            {
                return View(invoiceVM);
            }

            var invoiceLineNum = 0;
            invoiceVM.Items.ForEach(i => i.LineNumber = ++invoiceLineNum);

            SaveBillDetailsInServerAsJson(invoiceVM);

            var newInvoiceFileName = PrimitiveTypeExtensions.MakeValidFileName(
                invoiceVM.Company.Name + "_Invoice_" + invoiceVM.InvoiceNumber + ".pdf");

            string invoiceTemplatePath = Path.Combine(_env.WebRootPath, "templates", "type1.html");

            var pdfBytes = await _pdfService.GeneratePdf(
                ExportExtensions.GetHtmlContent(
                    invoiceVM,
                    invoiceTemplatePath));

            ViewBag.IsDownloadSuccess = true;

            if (invoiceVM.IsPreview)
            {
                return File(pdfBytes, "application/pdf");
            }

            return File(pdfBytes, "application/pdf", newInvoiceFileName);
        }
        catch (Exception)
        {
            ModelState.AddModelError("", "An error occurred while saving.");
            return View(invoiceVM);
        }
    }

    [HttpPost("preview-invoice")]
    [ValidateAntiForgeryToken]
    public IActionResult PreviewInvoice([FromForm] InvoiceViewModel invoiceVM)
    {
        try
        {
            var invoiceLineNum = 0;
            invoiceVM.Items.ForEach(i => i.LineNumber = ++invoiceLineNum);
            return PartialView("~/Views/Home/Partials/_InvoicePreviewModal.cshtml", invoiceVM);
        }
        catch (Exception)
        {
            throw;
        }
    }

    private void SaveBillDetailsInServerAsJson(InvoiceViewModel invoiceVM)
    {
        string savedInvoicePath = Path.Combine(
               _env.WebRootPath,
               $"invoiceJson\\invoices_{DateTime.UtcNow:MMM-yyyy}.json");

        PrimitiveTypeExtensions.AppendJsonObjectToFile(
            savedInvoicePath,
            new() { InvoiceVM = invoiceVM });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<FileStreamResult> OldDownload([FromForm] InvoiceViewModel invoiceVM)
    {
        try
        {
            var invoiceLineNum = 0;
            invoiceVM.Items.ForEach(i => i.LineNumber = ++invoiceLineNum);

            //if (!ObjectComparer.AreEqual(invoiceVM, InvoiceViewModel.GetTempData()))
            //{
            //}
            string savedInvoicePath = Path.Combine(_env.WebRootPath, $"invoiceJson\\invoices_{DateTime.UtcNow:MMM-yyyy}.json");
            PrimitiveTypeExtensions.AppendJsonObjectToFile(savedInvoicePath, new SavedInvoiceData() { InvoiceVM = invoiceVM });


            var newInvoiceFileName = PrimitiveTypeExtensions.MakeValidFileName(invoiceVM.Company.Name + "_Invoice_" + invoiceVM.InvoiceNumber + ".pdf");
            string invoiceTemplatePath = Path.Combine(_env.WebRootPath, "invoice-template.html");

            var pdfBytes = await _pdfService.GeneratePdf(ExportExtensions.GetHtmlContent(invoiceVM, invoiceTemplatePath));


            Response.Cookies.Append(_lastSavedInvoiceCookieName, JsonSerializer.Serialize(invoiceVM), new() { Expires = DateTime.Now.AddDays(60) });


            ViewBag.IsDownloadSuccess = true;

            var stream = new MemoryStream(pdfBytes);
            return new FileStreamResult(stream, new MediaTypeHeaderValue("application/pdf"))
            {
                FileDownloadName = newInvoiceFileName
            };
        }
        catch (Exception)
        {
            throw;
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Download([FromForm] InvoiceViewModel invoiceVM)
    {
        try
        {
            var invoiceLineNum = 0;
            invoiceVM.Items.ForEach(i => i.LineNumber = ++invoiceLineNum);

            //if (!ObjectComparer.AreEqual(invoiceVM, InvoiceViewModel.GetTempData()))
            //{
            //}
            string savedInvoicePath = Path.Combine(_env.WebRootPath, $"invoiceJson\\invoices_{DateTime.UtcNow:MMM-yyyy}.json");
            PrimitiveTypeExtensions.AppendJsonObjectToFile(savedInvoicePath, new SavedInvoiceData() { InvoiceVM = invoiceVM });


            var newInvoiceFileName = PrimitiveTypeExtensions.MakeValidFileName(invoiceVM.Company.Name + "_Invoice_" + invoiceVM.InvoiceNumber + ".pdf");
            string invoiceTemplatePath = Path.Combine(_env.WebRootPath, "invoice-template.html");

            var pdfBytes = await _pdfService.GeneratePdf(ExportExtensions.GetHtmlContent(invoiceVM, invoiceTemplatePath));


            Response.Cookies.Append(_lastSavedInvoiceCookieName, JsonSerializer.Serialize(invoiceVM), new() { Expires = DateTime.Now.AddDays(60) });
            if (!string.IsNullOrWhiteSpace(invoiceVM.Company.LogoBase64))
                Response.Cookies.Append(_lastSavedInvoiceLogo, invoiceVM.Company.LogoBase64, new() { Expires = DateTime.Now.AddDays(60) });


            ViewBag.IsDownloadSuccess = true;

            // Disable response buffering for large files
            Response.Headers.Append("Content-Disposition", "attachment; filename=" + newInvoiceFileName);
            Response.ContentType = "application/pdf";
            Response.ContentLength = pdfBytes.Length;

            // Write directly to response stream
            await Response.Body.WriteAsync(pdfBytes, 0, pdfBytes.Length);
            await Response.Body.FlushAsync();

            return new EmptyResult();
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