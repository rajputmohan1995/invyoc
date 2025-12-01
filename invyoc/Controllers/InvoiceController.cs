using invyoc.Extensions;
using invyoc.Models;
using Microsoft.AspNetCore.Mvc;

namespace invyoc.Controllers;

public class InvoiceController : Controller
{
    private readonly IWebHostEnvironment _env;
    private readonly PdfService _pdfService;

    public InvoiceController(
        IWebHostEnvironment env,
        PdfService pdfService)
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

    [HttpPost]
    public PartialViewResult NewLineItem(InvoiceItemViewModel lineItem)
    {
        return PartialView("Partials/_LineItem", lineItem);
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
}
