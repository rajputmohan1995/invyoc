using invyoc.Models;
using iText.Html2pdf;
using iText.Kernel.Pdf;
using iText.Kernel.Geom;

namespace invyoc.Extensions;

public static class ExportExtensions
{
    //public static void ConvertHTMLToPDF(string fileName, string htmlContent)
    //{
    //    var converter = new SynchronizedConverter(new PdfTools());
    //    var doc = new HtmlToPdfDocument()
    //    {
    //        GlobalSettings =
    //        {
    //            PaperSize = PaperKind.A4,
    //            Orientation = Orientation.Portrait,
    //            Out = fileName
    //        },
    //        Objects =
    //        {
    //            new ObjectSettings
    //            {
    //                HtmlContent = htmlContent
    //            }
    //        }
    //    };

    //    converter.Convert(doc);
    //}

    //public static void ConvertHTMLToDoc(string fileName, string htmlContent)
    //{
    //    using var stream = new MemoryStream();
    //    using (var wordDoc = WordprocessingDocument.Create(stream, DocumentFormat.OpenXml.WordprocessingDocumentType.Document))
    //    {
    //        var mainPart = wordDoc.AddMainDocumentPart();
    //        mainPart.Document = new Document(new Body());

    //        var converter = new HtmlConverter(mainPart);
    //        var paragraphs = converter.Parse(htmlContent);
    //        var body = mainPart.Document.Body;

    //        foreach (var p in paragraphs)
    //        {
    //            body.Append(p);
    //        }
    //    }

    //    File.WriteAllBytes(fileName, stream.ToArray());
    //}

    public static void ConvertToPDF(InvoiceViewModel invoiceVM,
        string invoiceTemplatePath,
        string newInvoicePath,
        string newInvoiceFileName)
    {
        if (!Directory.Exists(newInvoicePath))
            Directory.CreateDirectory(newInvoicePath);

        DirectoryInfo tempInvoiceFolder = new(newInvoicePath);
        tempInvoiceFolder.Empty();

        using (StreamWriter outputFile = new(invoiceTemplatePath))
        {
            outputFile.Write("invoiceContent");
        }

        string newInvoiceFullPath = System.IO.Path.Combine(newInvoicePath, newInvoiceFileName);
        CreatePdf(invoiceTemplatePath, newInvoiceFullPath);

        Directory.Delete(newInvoiceFullPath, true);
    }

    private static void CreatePdf(string htmlFilePath, string pdfFilePath)
    {
        // Create a PDF writer with A4 page size
        using PdfWriter writer = new(pdfFilePath, new WriterProperties().SetPdfVersion(PdfVersion.PDF_2_0));
        writer.SetCloseStream(false); // We need to close the writer manually after conversion

        // Set page size to A4
        PdfDocument pdf = new(writer);
        pdf.SetDefaultPageSize(PageSize.A4);

        // Create converter
        ConverterProperties properties = new();
        properties.SetBaseUri(Directory.GetCurrentDirectory() + "/");

        using (FileStream fileStream = new(htmlFilePath, FileMode.Open))
        {
            HtmlConverter.ConvertToPdf(fileStream, pdf, properties);
        }

        // Close the PDF writer
        pdf.Close();
        writer.Close();
    }

    private static void Empty(this DirectoryInfo directory)
    {
        foreach (FileInfo file in directory.GetFiles())
            file.Delete();

        foreach (DirectoryInfo subDirectory in directory.GetDirectories())
            subDirectory.Delete(true);
    }

    private string GetHtmlContent(InvoiceViewModel invoiceVM, string invoiceTemplatePath)
    {
        string htmlContent = File.ReadAllText(invoiceTemplatePath);

        htmlContent.Replace("")
    }
}