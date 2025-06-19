using invyoc.Models;

namespace invyoc.Extensions;

public static class ExportExtensions
{
    public static string GetHtmlContent(InvoiceViewModel invoiceVM, string invoiceTemplatePath)
    {
        string htmlContent = File.ReadAllText(invoiceTemplatePath);

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

            var invoiceItemTemplate = @"<tr>
                                            <td>[{Items[i].LineNumber}]</td>
                                            <td class='text-left'>[{Items[i].Description}]</td>
                                            <td>[{Items[i].Quantity}]</td>
                                            <td>[{Items[i].Rate}]</td>
                                            <td>[{Items[i].Total}]</td>
                                        </tr>";

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
}