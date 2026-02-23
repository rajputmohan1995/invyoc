using invyoc.Extensions;
using invyoc.Models;
using invyoc.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using System.Diagnostics;

namespace invyoc.Controllers;

public class HomeController(
    IWebHostEnvironment env,
    IMemoryCache cache,
    IInvoiceService invoiceService) : Controller
{
    private readonly IWebHostEnvironment _env = env;
    private readonly IMemoryCache _cache = cache;
    private readonly IInvoiceService _invoiceService = invoiceService;

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
    public async Task<IActionResult> SavedInfo(
        string p,
        int month,
        int year,
        string search = "",
        int pageNum = 1,
        int pageSize = 1000)
    {
        if (p != CommonSetting.PassKey)
            return NotFound();

        ViewBag.SavedInfoMonthYear = $"{month}-{year}";

        var savedInvoices = await _invoiceService.GetAllAsync(new CommonListVM(pageNum, pageSize, search, month, year));
        return View(savedInvoices);
    }
}