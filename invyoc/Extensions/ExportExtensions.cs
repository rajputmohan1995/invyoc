using invyoc.Models;

namespace invyoc.Extensions;

public static class ExportExtensions
{
    public static string GetHtmlContent(InvoiceViewModel invoiceVM, string invoiceTemplatePath)
    {
        string htmlContent = File.ReadAllText(invoiceTemplatePath);

        htmlContent = htmlContent.Replace("[{CompanyName}]", invoiceVM.Company.Name);
        htmlContent = htmlContent.Replace("[{CompanyAddress}]", invoiceVM.Company?.CompanyAddress?.Address);
        htmlContent = htmlContent.Replace("[{CompanyCity}]", invoiceVM.Company?.CompanyAddress?.City);
        htmlContent = htmlContent.Replace("[{CompanyState}]", invoiceVM.Company?.CompanyAddress?.State);
        htmlContent = htmlContent.Replace("[{CompanyCountry}]", invoiceVM.Company?.CompanyAddress?.Country);
        htmlContent = htmlContent.Replace("[{CompanyPhone}]", invoiceVM.Company?.CompanyAddress?.ContactNum);
        htmlContent = htmlContent.Replace("[{CompanyEmail}]", invoiceVM.Company?.Email);
        htmlContent = htmlContent.Replace("[{GSTNo}]", invoiceVM.Company?.GSTNo);

        var companyLogoContent = "";
        if (!string.IsNullOrWhiteSpace(invoiceVM.Company?.LogoBase64))
        {
            companyLogoContent = $"<img src='{invoiceVM.Company.LogoBase64}' alt='Logo not found!' style='max-width:150px;max-height:150px;' />";
        }
        htmlContent = htmlContent.Replace("[{CompanyLogoContent}]", companyLogoContent);


        htmlContent = htmlContent.Replace("[{BillToName}]", invoiceVM.BillTo.Name);
        htmlContent = htmlContent.Replace("[{BillToGSTNo}]", invoiceVM.BillTo.GSTNo);
        htmlContent = htmlContent.Replace("[{BillToAddress}]", invoiceVM.BillTo?.ClientAddress?.Address);
        htmlContent = htmlContent.Replace("[{BillToCity}]", invoiceVM.BillTo?.ClientAddress?.City);
        htmlContent = htmlContent.Replace("[{BillToState}]", invoiceVM.BillTo?.ClientAddress?.State);
        htmlContent = htmlContent.Replace("[{BillToCountry}]", invoiceVM.BillTo?.ClientAddress?.Country);
        htmlContent = htmlContent.Replace("[{BillToContact}]", invoiceVM.BillTo?.ClientAddress?.ContactNum);


        var shipToContent = "";
        var billToContentWidth = "70";

        if (!string.IsNullOrWhiteSpace(invoiceVM.ShipTo.Address) ||
            !string.IsNullOrWhiteSpace(invoiceVM.ShipTo.City) ||
            !string.IsNullOrWhiteSpace(invoiceVM.ShipTo.State) ||
            !string.IsNullOrWhiteSpace(invoiceVM.ShipTo.Country) ||
            !string.IsNullOrWhiteSpace(invoiceVM.ShipTo.ContactNum))
        {
            shipToContent = @$"<td width='35%' class='pr-20' >
                <strong>Ship To</strong>
                <div style='margin-top:2px;'>
                    <span style='word-wrap: break-word'>{invoiceVM.ShipTo.Address}</span>
                    <br />
                    <span style=""word-wrap: break-word"">{invoiceVM.ShipTo.City}</span>
                    <br />
                    <span style=""word-wrap: break-word"">{invoiceVM.ShipTo.State}</span>
                    <br />
                    <span style=""word-wrap: break-word"">{invoiceVM.ShipTo.Country}</span>
                    <br />
                    <span style='word-wrap: break-word'>{invoiceVM.ShipTo.ContactNum}</span>
                </div>
            </td>";

            billToContentWidth = "35";
        }

        htmlContent = htmlContent.Replace("[{ShipToContent}]", shipToContent);
        htmlContent = htmlContent.Replace("[{BillToContentWidth}]", billToContentWidth);

        htmlContent = htmlContent.Replace("[{InvoiceNumber}]", invoiceVM.InvoiceNumber);
        htmlContent = htmlContent.Replace("[{InvoiceDate}]", PrimitiveTypeExtensions.ToDateStr(invoiceVM.InvoiceDate));
        htmlContent = htmlContent.Replace("[{DueDate}]", PrimitiveTypeExtensions.ToDateStr(invoiceVM.DueDate));
        htmlContent = htmlContent.Replace("[{PONumber}]", invoiceVM.PONumber);

        htmlContent = htmlContent.Replace("[{PaymentTerms}]", invoiceVM.PaymentTerms);
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
                                            <td>[{Items[i].SGST}]</td>
                                            <td>[{Items[i].CGST}]</td>
                                            <td>[{Items[i].Cess}]</td>
                                            <td>[{Items[i].Total}]</td>
                                        </tr>";

            invoiceItemTemplate = invoiceItemTemplate.Replace("[{Items[i].LineNumber}]", invoiceItemIndex.ToString());
            invoiceItemTemplate = invoiceItemTemplate.Replace("[{Items[i].Description}]", item.Description);
            invoiceItemTemplate = invoiceItemTemplate.Replace("[{Items[i].Quantity}]", item.Quantity.ToString());
            invoiceItemTemplate = invoiceItemTemplate.Replace("[{Items[i].Rate}]", item.Rate.ToFormat().ToString());
            invoiceItemTemplate = invoiceItemTemplate.Replace("[{Items[i].SGST}]", item.SGST.ToFormat().ToString());
            invoiceItemTemplate = invoiceItemTemplate.Replace("[{Items[i].CGST}]", item.CGST.ToFormat().ToString());
            invoiceItemTemplate = invoiceItemTemplate.Replace("[{Items[i].Cess}]", item.Cess.ToFormat().ToString());
            invoiceItemTemplate = invoiceItemTemplate.Replace("[{Items[i].Total}]", itemTotal.ToFormat().ToString());

            invoiceItemIndex++;
            invoiceItemTotal += itemTotal;
            invoiceItems += invoiceItemTemplate;
        }

        //var discountAmount = ((invoiceItemTotal * invoiceVM.DiscountPercentage) / 100).ToFormat();
        //var taxAmount = (((invoiceItemTotal - discountAmount) * invoiceVM.TaxPercentage) / 100).ToFormat();
        //var finalAmount = (invoiceItemTotal - discountAmount + taxAmount).ToFormat();
        var finalAmount = invoiceItemTotal.ToFormat();

        invoiceItems += @"<tr>
            <td class='border-0 p-0 m-0' colspan='8'></td>
        </tr>";

        invoiceItems += @$"<tr>
            <td class='border-0' colspan='5'></td>
            <td class='text-right' colspan='2'>Subtotal</td>
            <td>{invoiceItemTotal.ToFormat()}</td>
        </tr>";

        //invoiceItems += @$"<tr>
        //    <td class='border-0' colspan='3'></td>
        //    <td class='text-right'>Discount ({invoiceVM.DiscountPercentage}%)</td>
        //    <td>{invoiceVM.Currency + discountAmount.ToCurrency()}</td>
        //</tr>";

        //invoiceItems += @$"<tr>
        //    <td class='border-0' colspan='3'></td>
        //    <td class='text-right'>Tax ({invoiceVM.TaxPercentage}%)</td>
        //    <td>{invoiceVM.Currency + taxAmount.ToCurrency()}</td>
        //</tr>";

        invoiceItems += @$"<tr>
            <td class='border-0' colspan='5'></td>
            <td class='text-right' colspan='2'><strong>Total</strong></td>
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