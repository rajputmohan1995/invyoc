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
        htmlContent = htmlContent.Replace("[{GSTNo}]", invoiceVM.Company.GSTNo);


        htmlContent = htmlContent.Replace("[{BillToName}]", invoiceVM.BillTo.Name);
        htmlContent = htmlContent.Replace("[{BillToAddress}]", invoiceVM.BillTo.Address);
        htmlContent = htmlContent.Replace("[{BillToContact}]", invoiceVM.BillTo.ContactNum);


        var shipToContent = "";
        var billToContentWidth = "70";

        if (!string.IsNullOrWhiteSpace(invoiceVM.ShipTo.Name) ||
            !string.IsNullOrWhiteSpace(invoiceVM.ShipTo.Address) ||
            !string.IsNullOrWhiteSpace(invoiceVM.ShipTo.ContactNum))
        {
            shipToContent = @$"<td width='35%' class='pr-20' >
                <strong>Ship To</strong>
                <div style='margin-top:2px;'>
                    {invoiceVM.ShipTo.Name}<br />
                    <span style='word-wrap: break-word'>{invoiceVM.ShipTo.Address}</span>
                    <br />
                    <span style='word-wrap: break-word'>{invoiceVM.ShipTo.ContactNum}</span>
                </div>
            </td>";
            billToContentWidth = "35";
        }

        htmlContent = htmlContent.Replace("[{ShipToContent}]", shipToContent);
        htmlContent = htmlContent.Replace("[{BillToContentWidth}]", billToContentWidth);

        //htmlContent = htmlContent.Replace("[{ShipToName}]", invoiceVM.ShipTo.Name);
        //htmlContent = htmlContent.Replace("[{ShipToAddress}]", invoiceVM.ShipTo.Address);
        //htmlContent = htmlContent.Replace("[{ShipToContact}]", invoiceVM.ShipTo.ContactNum);

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
            invoiceItemTemplate = invoiceItemTemplate.Replace("[{Items[i].Rate}]", item.Rate.ToCurrency());
            invoiceItemTemplate = invoiceItemTemplate.Replace("[{Items[i].Total}]", itemTotal.ToCurrency());

            invoiceItemIndex++;
            invoiceItemTotal += itemTotal;
            invoiceItems += invoiceItemTemplate;
        }

        var discountAmount = ((invoiceItemTotal * invoiceVM.DiscountPercentage) / 100).ToFormat();
        var taxAmount = (((invoiceItemTotal - discountAmount) * invoiceVM.TaxPercentage) / 100).ToFormat();
        var finalAmount = (invoiceItemTotal - discountAmount + taxAmount).ToFormat();

        invoiceItems += @"<tr>
            <td class='border-0 p-0 m-0' colspan='5'></td>
        </tr>";

        invoiceItems += @$"<tr>
            <td class='border-0' colspan='3'></td>
            <td class='text-right'>Subtotal</td>
            <td>{invoiceVM.Currency + invoiceItemTotal.ToCurrency()}</td>
        </tr>";

        invoiceItems += @$"<tr>
            <td class='border-0' colspan='3'></td>
            <td class='text-right'>Discount ({invoiceVM.DiscountPercentage}%)</td>
            <td>{invoiceVM.Currency + discountAmount.ToCurrency()}</td>
        </tr>";

        invoiceItems += @$"<tr>
            <td class='border-0' colspan='3'></td>
            <td class='text-right'>Tax ({invoiceVM.TaxPercentage}%)</td>
            <td>{invoiceVM.Currency + taxAmount.ToCurrency()}</td>
        </tr>";

        invoiceItems += @$"<tr>
            <td class='border-0' colspan='3'></td>
            <td class='text-right'><strong>Total</strong></td>
            <td><strong>{invoiceVM.Currency + finalAmount.ToCurrency()}</strong></td>
        </tr>";

        htmlContent = htmlContent.Replace("[{InvoiceItems}]", invoiceItems);

        //htmlContent = htmlContent.Replace("[{SubTotal}]", invoiceVM.Currency + invoiceItemTotal.ToINRCurrency());

        //htmlContent = htmlContent.Replace("[{DiscountPercentage}]", invoiceVM.DiscountPercentage.ToString() + "%");
        //htmlContent = htmlContent.Replace("[{DiscountAmount}]", invoiceVM.Currency + discountAmount.ToINRCurrency());

        //htmlContent = htmlContent.Replace("[{TaxAmount}]", invoiceVM.Currency + taxAmount.ToINRCurrency());
        //htmlContent = htmlContent.Replace("[{TaxPercentage}]", invoiceVM.TaxPercentage.ToString() + "%");

        //htmlContent = htmlContent.Replace("[{FinalAmount}]", invoiceVM.Currency + finalAmount.ToINRCurrency());

        return htmlContent;
    }
}