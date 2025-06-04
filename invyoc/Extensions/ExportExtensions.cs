using invyoc.Models;
using iText.Html2pdf;
using iText.Kernel.Pdf;
using iText.Kernel.Geom;

namespace invyoc.Extensions;

public static class ExportExtensions
{
    public static (string, string) ConvertToPDF(InvoiceViewModel invoiceVM, string webRootPath)
    {
        string newInvoicePath = System.IO.Path.Combine(webRootPath, "exports");

        if (!Directory.Exists(newInvoicePath))
            Directory.CreateDirectory(newInvoicePath);

        DirectoryInfo tempInvoiceFolder = new(newInvoicePath);
        tempInvoiceFolder.Empty();


        var newInvoiceFileName = PrimitiveTypeExtensions.MakeValidFileName(
            invoiceVM.Company.Name + "_Invoice_" + invoiceVM.InvoiceNumber + ".pdf");

        var dynamicInvoiceHtmlPath = System.IO.Path.Combine(newInvoicePath, "htmls");
        string dynmaicInvoiceHtmlFileName = System.IO.Path.Combine(dynamicInvoiceHtmlPath, "default.html");

        if (!Directory.Exists(dynamicInvoiceHtmlPath))
            Directory.CreateDirectory(dynamicInvoiceHtmlPath);

        DirectoryInfo tempInvoiceHtmlFolder = new(dynamicInvoiceHtmlPath);
        tempInvoiceHtmlFolder.Empty();


        string invoiceTemplatePath = System.IO.Path.Combine(webRootPath, "free-invoice.html");
        using (StreamWriter outputFile = new(dynmaicInvoiceHtmlFileName))
        {
            outputFile.Write(GetHtmlContent(invoiceVM, invoiceTemplatePath));
        }


        string newInvoiceFullPath = System.IO.Path.Combine(newInvoicePath, newInvoiceFileName);
        CreatePdf(dynmaicInvoiceHtmlFileName, newInvoiceFullPath);

        return (newInvoiceFullPath, newInvoiceFileName);
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

    private static string GetHtmlContent(InvoiceViewModel invoiceVM, string invoiceTemplatePath)
    {
        //string htmlContent = File.ReadAllText(invoiceTemplatePath);
        string htmlContent = GetInvoiceTemplateContent();

        htmlContent = htmlContent.Replace("[{CompanyName}]", invoiceVM.Company.Name);
        htmlContent = htmlContent.Replace("[{CompanyAddress}]", invoiceVM.Company.Address);
        htmlContent = htmlContent.Replace("[{CompanyEmail}]", invoiceVM.Company.Email);
        htmlContent = htmlContent.Replace("[{CompanyPhone}]", invoiceVM.Company.Phone);

        htmlContent = htmlContent.Replace("[{BillToName}]", invoiceVM.BillTo.Name);
        htmlContent = htmlContent.Replace("[{BillToAddress}]", invoiceVM.BillTo.Address);

        htmlContent = htmlContent.Replace("[{ShipToName}]", invoiceVM.ShipTo.Name);
        htmlContent = htmlContent.Replace("[{ShipToAddress}]", invoiceVM.ShipTo.Address);

        htmlContent = htmlContent.Replace("[{InvoiceNumber}]", invoiceVM.InvoiceNumber);
        htmlContent = htmlContent.Replace("[{InvoiceDate}]", PrimitiveTypeExtensions.ToDateStr(invoiceVM.InvoiceDate));
        htmlContent = htmlContent.Replace("[{PaymentTerms}]", invoiceVM.PaymentTerms);
        htmlContent = htmlContent.Replace("[{DueDate}]", PrimitiveTypeExtensions.ToDateStr(invoiceVM.DueDate));
        htmlContent = htmlContent.Replace("[{PONumber}]", invoiceVM.PONumber);
        htmlContent = htmlContent.Replace("[{PaymentNotes}]", invoiceVM.PaymentNotes);

        var invoiceItems = "";
        var invoiceItemTotal = 0m;
        var invoiceItemIndex = 1;
        foreach (var item in invoiceVM.Items)
        {
            var itemTotal = (item.Rate * item.Quantity).ToFormat();

            var invoiceItemTemplate = "<tr>\r\n<td><span name=\"Items[i].LineNumber\">[{Items[i].LineNumber}]</span></td>\r\n<td>\r\n<input type=\"text\" class=\"form-control form-control-sm item-desc\" name=\"Items[i].Description\" value=\"[{Items[i].Description}]\">\r\n</td>\r\n<td>\r\n<input type=\"text\" class=\"form-control form-control-sm item-qty\" name=\"Items[i].Quantity\" value=\"[{Items[i].Quantity}]\">\r\n</td>\r\n<td>\r\n<input type=\"text\" class=\"form-control form-control-sm item-price\" name=\"Items[i].Rate\" value=\"[{Items[i].Rate}]\">\r\n</td>\r\n<td class=\"item-total\">[{Items[i].Total}]</td>\r\n</tr>";
            invoiceItemTemplate = invoiceItemTemplate.Replace("[{Items[i].LineNumber}]", invoiceItemIndex.ToString());
            invoiceItemTemplate = invoiceItemTemplate.Replace("[{Items[i].Description}]", item.Description);
            invoiceItemTemplate = invoiceItemTemplate.Replace("[{Items[i].Quantity}]", item.Quantity.ToString());
            invoiceItemTemplate = invoiceItemTemplate.Replace("[{Items[i].Rate}]", invoiceVM.Currency + item.Rate.ToString());
            invoiceItemTemplate = invoiceItemTemplate.Replace("[{Items[i].Total}]", invoiceVM.Currency + itemTotal.ToString());

            invoiceItemIndex++;
            invoiceItemTotal += itemTotal;
            invoiceItems += invoiceItemTemplate;
        }

        htmlContent = htmlContent.Replace("[{InvoiceItems}]", invoiceItems);

        htmlContent = htmlContent.Replace("[{DiscountPercentage}]", invoiceVM.DiscountPercentage.ToString() + "%");
        htmlContent = htmlContent.Replace("[{TaxPercentage}]", invoiceVM.TaxPercentage.ToString() + "%");

        htmlContent = htmlContent.Replace("[{SubTotal}]", invoiceVM.Currency + invoiceItemTotal.ToString());

        var discountAmount = ((invoiceItemTotal * invoiceVM.DiscountPercentage) / 100).ToFormat();
        htmlContent = htmlContent.Replace("[{DiscountAmount}]", invoiceVM.Currency + discountAmount.ToString());

        var taxAmount = (((invoiceItemTotal - discountAmount) * invoiceVM.TaxPercentage) / 100).ToFormat();
        htmlContent = htmlContent.Replace("[{TaxAmount}]", invoiceVM.Currency + taxAmount.ToString());

        var finalAmount = (invoiceItemTotal - discountAmount + taxAmount).ToFormat();
        htmlContent = htmlContent.Replace("[{FinalAmount}]", invoiceVM.Currency + finalAmount.ToString());

        return htmlContent;
    }

    private static string GetInvoiceTemplateContent()
    {
        return @"<head>
    <meta charset=""UTF-8"">
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
    <link href=""https://cdn.jsdelivr.net/npm/bootstrap@5.3.0/dist/css/bootstrap.min.css"" rel=""stylesheet"">

    <style>
        html, html body {
            font-family: Verdana, Geneva, Tahoma, sans-serif;
        }

        .invoice-main-details input {
            max-width: 150px;
        }

        .table th, .table td {
            vertical-align: middle;
        }

        .invoice-box {
            margin: auto;
            padding: 30px;
            border: 1px solid #eee;
            box-shadow: 0 0 10px rgba(0,0,0,.15);
        }


        .sticky-top {
            top: 20px;
        }

        span.invoice-total-details {
            min-width: 100px;
            display: inline-block;
        }

        div#invoiceNotes, div#invoiceTerms {
            position: absolute;
            bottom: 0;
        }

        div#divInvoiceTotals input {
            max-width: 100px;
        }

        div#divInvoiceTotals p {
            margin-bottom: 0;
        }
    </style>

</head>

<body class=""pt-4 pb-4"">


    <div class=""container"">
        <div class=""row"">

            <div class=""col-lg-12 invoice-box"">

                <!--Header-->
                <div class=""row"">
                    <div class=""col-9"">
                        <h2>
                            <input type=""text"" class=""form-control border-0 fs-2 p-0 ""
                                   placeholder=""Company Name"" name=""Company.Name"" value=""[{CompanyName}]"">
                        </h2>
                        <div class=""row"">
                            <div class=""col"">
                                <input type=""text"" class=""form-control form-control-sm border-0 p-0 mb-1 ""
                                       name=""Company.Address"" value=""[{CompanyAddress}]"">
                            </div>
                        </div>
                        <div class=""row"">
                            <div class=""col-3"">
                                <input type=""tel"" class=""form-control form-control-sm border-0 p-0 ""
                                       name=""Company.Phone"" value=""[{CompanyPhone}]"">
                            </div>
                            <div class=""col-4"">
                                <input type=""email"" class=""form-control form-control-sm border-0 p-0 ""
                                       name=""Company.Email"" value=""[{CompanyEmail}]"">
                            </div>
                            <div class=""offset-col-6""></div>
                        </div>

                    </div>

                    <div class=""col-3"">
                        <div class=""d-flex align-items-center gap-2 justify-content-end text-end"">
                            <h4>
                                Invoice#
                                <span contenteditable=""true"" name=""InvoiceNumber"">[{InvoiceNumber}]</span>
                            </h4>
                        </div>
                    </div>

                </div>

                <hr>
                <!--Header-->



                <div class=""row mb-4"">
                    <div class=""col-sm-4"">
                        <h5>Bill To</h5>
                        <input type=""text"" class=""form-control fw-bold mb-1"" name=""BillTo.Name"" value=""[{BillToName}]"" />
                        <textarea class=""form-control"" rows=""3"" name=""BillTo.Address"">[{BillToAddress}]</textarea>
                    </div>
                    <div class=""col-sm-4"">
                        <h5>Ship To <small>(optional)</small></h5>
                        <input type=""text"" class=""form-control fw-bold mb-1"" name=""ShipTo.Name"" value=""[{ShipToName}]"" />
                        <textarea class=""form-control"" rows=""3"" name=""ShipTo.Address"">[{ShipToAddress}]</textarea>
                    </div>

                    <div class=""col-sm-4"">
                        <div class=""d-flex flex-column gap-2 invoice-main-details"">

                            <div class=""d-flex align-items-center gap-2 justify-content-end text-end"">
                                <label class=""form-label mb-0"">Date</label>
                                <input type=""text"" class=""form-control form-control-sm"" name=""InvoiceDate"" value=""[{InvoiceDate}]"">
                            </div>
                            <div class=""d-flex align-items-center gap-2 justify-content-end text-end"">
                                <label class=""form-label mb-0"">Payment Terms</label>
                                <input type=""text"" class=""form-control form-control-sm"" name=""PaymentTerms"" value=""[{PaymentTerms}]"">
                            </div>
                            <div class=""d-flex align-items-center gap-2 justify-content-end text-end"">
                                <label class=""form-label mb-0"">Due Date</label>
                                <input type=""text"" class=""form-control form-control-sm"" name=""DueDate"" value=""[{DueDate}]"">
                            </div>
                            <div class=""d-flex align-items-center gap-2 justify-content-end text-end"">
                                <label class=""form-label mb-0"">PO Number</label>
                                <input type=""text"" class=""form-control form-control-sm"" name=""PONumber"" value=""[{PONumber}]"">
                            </div>
                        </div>
                    </div>
                </div>



                <div class=""row"">
                    <table class=""table table-sm table-bordered text-center"" id=""invoiceTable"">
                        <thead class=""table-light"">
                            <tr>
                                <th width=""5%"">#</th>
                                <th width=""60%"" class=""text-start"">Item Description</th>
                                <th width=""8%"">Quantity</th>
                                <th width=""15%"">Rate</th>
                                <th>Amount</th>
                            </tr>
                        </thead>
                        <tbody>

                            [{InvoiceItems}]
                            
                        </tbody>
                    </table>
                </div>




                <div class=""row mb-4"">

                    <div class=""col-md-12 justify-content-end text-end"" id=""divInvoiceTotals"">

                        <div class=""d-flex flex-column gap-2"">

                            <div class=""d-flex align-items-center gap-2 justify-content-end text-end"">
                                <label class=""form-label mb-0"">Discount:</label>
                                <input type=""text"" id=""discountRate"" class=""form-control form-control-sm border-0 text-end""
                                       name=""DiscountPercentage"" value=""[{DiscountPercentage}]"" />
                            </div>


                            <div class=""d-flex align-items-center gap-2 justify-content-end text-end"">
                                <label class=""form-label mb-0"">Tax:</label>
                                <input type=""text"" id=""taxRate"" class=""form-control form-control-sm border-0 text-end""
                                       name=""TaxPercentage"" value=""[{TaxPercentage}]"">
                            </div>

                            <p>
                                <strong>Subtotal:</strong>
                                <span id=""subtotal"" class=""invoice-total-details"">[{SubTotal}]</span>
                            </p>

                            <p>
                                <strong>Discount:</strong>
                                <span id=""discountAmount"" class=""invoice-total-details"">[{DiscountAmount}]</span>
                            </p>
                            <p>
                                <strong>Tax:</strong>
                                <span id=""taxAmount"" class=""invoice-total-details"">[{TaxAmount}]</span>
                            </p>


                            <p class=""fs-5"">
                                <strong>Total:</strong>
                                <span id=""grandTotal"" class=""invoice-total-details"">[{FinalAmount}]</span>
                            </p>

                        </div>

                    </div>

                    <div class=""col"">
                        <hr>
                        <p contenteditable=""true"">
                            <i name=""PaymentNotes"">[{PaymentNotes}]</i>
                        </p>
                    </div>

                </div>


            </div>


        </div>
    </div>


</body>";
    }
}