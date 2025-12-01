using invyoc.Extensions;
using invyoc.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace invyoc.Controllers;

public class HomeController : Controller
{
    private readonly IWebHostEnvironment _env;

    public HomeController(IWebHostEnvironment env)
    {
        _env = env;
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

    [ResponseCache(Duration = 0,
        Location = ResponseCacheLocation.None,
        NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
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
}