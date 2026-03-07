using Microsoft.AspNetCore.Mvc;

namespace invyoc.Controllers;

[Route("gst-invoice-format")]
public class BlogController : Controller
{
    [Route("excel")]
    public IActionResult Excel()
    {
        return View();
    }

    [Route("pdf")]
    public IActionResult Pdf()
    {
        return View();
    }

    [Route("services")]
    public IActionResult Services()
    {
        return View();
    }

    [Route("/gst-invoice-format-india")]
    public IActionResult GSTInvoiceFormatIndia()
    {
        return View();
    }

    [Route("/gst-invoice-for-freelancers")]
    public IActionResult GSTInvoiceForFreelancers()
    {
        return View();
    }

    [Route("/gst-invoice-for-small-business")]
    public IActionResult GSTInvoiceForSmallBusiness()
    {
        return View();
    }
}