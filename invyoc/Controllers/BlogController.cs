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
}