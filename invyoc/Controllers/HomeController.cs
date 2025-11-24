using invyoc.Extensions;
using invyoc.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace invyoc.Controllers;

public class HomeController : Controller
{
    private readonly IWebHostEnvironment _env;
    private readonly PdfService _pdfService;
    private const string _lastSavedInvoiceCookieName = "LastSavedInvoice";
    private const string _lastSavedInvoiceLogo = "LastSavedInvoiceLogo";

    public HomeController(IWebHostEnvironment env, PdfService pdfService)
    {
        _env = env;
        _pdfService = pdfService;
    }

    [HttpGet]
    public IActionResult Index()
    {
        InvoiceViewModel invoiceVM = new();
        //invoiceVM = InvoiceViewModel.GetTempData();
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

            string invoiceTemplatePath = Path.Combine(_env.WebRootPath, "templates", "type1.html");

            var pdfBytes = await _pdfService.GeneratePdf(
                ExportExtensions.GetHtmlContent(
                    invoiceVM,
                    invoiceTemplatePath));

            var newInvoiceFileName = PrimitiveTypeExtensions.MakeValidFileName($"{invoiceVM.InvoiceNumber}_{DateTime.Now:ddMMyyyyHHmmss}.pdf");
            string invoiceFilePath = Path.Combine(_env.WebRootPath, "output");


            EmptyPdfOutputFolder(invoiceFilePath);

            System.IO.File.WriteAllBytes(
                Path.Combine(invoiceFilePath, newInvoiceFileName),
                pdfBytes);

            return RedirectToAction("Download",
                new
                {
                    pdfFilePath = invoiceFilePath,
                    fileName = newInvoiceFileName,
                    isPreview = invoiceVM.IsPreview
                });
        }
        catch (Exception ex)
        {
            var exceptionLogPath = Path.Combine(_env.WebRootPath, "globalException.json");
            ExceptionLogger.LogException(ex, exceptionLogPath);
            throw;
        }
    }

    [HttpGet]
    public async Task<IActionResult> Download(string pdfFilePath, string fileName, bool isPreview)
    {
        var pdfFileFullPath = Path.Combine(pdfFilePath, fileName);

        if (!System.IO.File.Exists(pdfFileFullPath))
            throw new FileNotFoundException("Unable to generate PDF file.", pdfFileFullPath);

        var pdfBytes = await System.IO.File.ReadAllBytesAsync(pdfFileFullPath);

        if (isPreview)
            return File(pdfBytes, "application/pdf");

        return File(pdfBytes, "application/pdf", fileName);
    }

    [ResponseCache(Duration = 0,
        Location = ResponseCacheLocation.None,
        NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }

    private void EmptyPdfOutputFolder(string outputPath)
    {
        DirectoryInfo di = new(outputPath);

        foreach (FileInfo file in di.GetFiles())
        {
            file.Delete();
        }
        foreach (DirectoryInfo dir in di.GetDirectories())
        {
            dir.Delete(true);
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