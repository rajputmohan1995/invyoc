using DinkToPdf;
using DinkToPdf.Contracts;

namespace invyoc.Extensions;

public class PdfService
{
    private readonly IConverter _converter;

    public PdfService(IConverter converter)
    {
        _converter = converter;
    }

    public byte[] GeneratePdf(string htmlContent)
    {
        var globalSettings = new GlobalSettings
        {
            PaperSize = PaperKind.A4,
            Orientation = Orientation.Portrait,
            Margins = new MarginSettings
            {
                Top = 10,
                Bottom = 10,
                Left = 0,
                Right = 0
            }
        };

        var objectSettings = new ObjectSettings
        {
            PagesCount = true,
            HtmlContent = htmlContent
        };

        var pdf = new HtmlToPdfDocument()
        {
            GlobalSettings = globalSettings,
            Objects = { objectSettings },
        };

        return _converter.Convert(pdf);
    }
}
