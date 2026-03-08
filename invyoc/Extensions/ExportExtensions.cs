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
        htmlContent = htmlContent.Replace("[{CompanyPincode}]", invoiceVM.Company?.CompanyAddress?.Pincode);
        htmlContent = htmlContent.Replace("[{CompanyState}]", invoiceVM.Company?.CompanyAddress?.State);
        htmlContent = htmlContent.Replace("[{CompanyCountry}]", invoiceVM.Company?.CompanyAddress?.Country);
        htmlContent = htmlContent.Replace("[{CompanyPhone}]", invoiceVM.Company?.CompanyAddress?.ContactNum);
        htmlContent = htmlContent.Replace("[{CompanyEmail}]", invoiceVM.Company?.Email);
        htmlContent = htmlContent.Replace("[{GSTNo}]", invoiceVM.Company?.GSTNo);

        var companyLogoContent = "";
        if (!string.IsNullOrWhiteSpace(invoiceVM.Company?.LogoBase64))
        {
            companyLogoContent = $"<img src='{invoiceVM.Company.LogoBase64}' alt='Logo not found!' style='max-width:130px;max-height:130px;' />";
        }
        htmlContent = htmlContent.Replace("[{CompanyLogoContent}]", companyLogoContent);


        htmlContent = htmlContent.Replace("[{BillToName}]", invoiceVM.BillTo.Name);
        htmlContent = htmlContent.Replace("[{BillToGSTNo}]", invoiceVM.BillTo.GSTNo);
        htmlContent = htmlContent.Replace("[{BillToAddress}]", invoiceVM.BillTo?.ClientAddress?.Address);
        htmlContent = htmlContent.Replace("[{BillToCity}]", invoiceVM.BillTo?.ClientAddress?.City);
        htmlContent = htmlContent.Replace("[{BillToPincode}]", invoiceVM.BillTo?.ClientAddress?.Pincode);
        htmlContent = htmlContent.Replace("[{BillToState}]", invoiceVM.BillTo?.ClientAddress?.State);
        htmlContent = htmlContent.Replace("[{BillToCountry}]", invoiceVM.BillTo?.ClientAddress?.Country);
        htmlContent = htmlContent.Replace("[{BillToContact}]", "<i>Contact</i>: " + invoiceVM.BillTo?.ClientAddress?.ContactNum);


        var shipToContent = "";
        var billToContentWidth = "70";


        if (invoiceVM.ShipTo != null)
        {
            if (!string.IsNullOrWhiteSpace(invoiceVM.ShipTo?.Name) ||
                !string.IsNullOrWhiteSpace(invoiceVM.ShipTo?.GSTNo))
            {
                shipToContent = @$"<td width='35%' class='pr-20' >
                <strong style='text-decoration:underline; font-size:15px;'>Ship To</strong>
                <div style='margin-top:2px;'>
                    <span style='word-wrap: break-word'>{invoiceVM.ShipTo.Name}</span>
                    <br />                 
                    <span style='word-wrap: break-word'>{invoiceVM.ShipTo.GSTNo}</span>";
            }


            if (!string.IsNullOrWhiteSpace(invoiceVM.ShipTo?.ClientAddress?.Address) ||
               !string.IsNullOrWhiteSpace(invoiceVM.ShipTo?.ClientAddress?.City) ||
               !string.IsNullOrWhiteSpace(invoiceVM.ShipTo?.ClientAddress?.Pincode) ||
               !string.IsNullOrWhiteSpace(invoiceVM.ShipTo?.ClientAddress?.State) ||
               !string.IsNullOrWhiteSpace(invoiceVM.ShipTo?.ClientAddress?.Country) ||
               !string.IsNullOrWhiteSpace(invoiceVM.ShipTo?.ClientAddress?.ContactNum))
            {

                shipToContent +=
                    @$"<br />                 
                    <span style='word-wrap: break-word'>{invoiceVM.ShipTo.ClientAddress.Address}</span>
                    <br />
                    <span style=""word-wrap: break-word"">{invoiceVM.ShipTo.ClientAddress.City}</span>
                    <span style=""word-wrap: break-word"">{invoiceVM.ShipTo.ClientAddress.Pincode}</span>
                    <br />
                    <span style=""word-wrap: break-word"">{invoiceVM.ShipTo.ClientAddress.State}</span>
                    <span style=""word-wrap: break-word"">{invoiceVM.ShipTo.ClientAddress.Country}</span>
                    <br />
                    <span style='word-wrap: break-word'><i>Contact</i>: {invoiceVM.ShipTo.ClientAddress.ContactNum}</span>";

            }

            shipToContent += "</div></td>";
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

        invoiceVM.Items = [.. invoiceVM.Items
            .OrderBy(o => o.SGST)
            .ThenBy(o => o.CGST)
            .ThenBy(o => o.Cess)];


        foreach (var item in invoiceVM.Items)
        {
            var itemTotal = (item.Rate * item.Quantity).ToFormat();
            var invoiceItemTemplate = @"<tr>
                                            <td>[{Items[i].LineNumber}]</td>
                                            <td class='text-left'>[{Items[i].Description}]</td>
                                            <td class='text-left'>[{Items[i].HSN_SAC}]</td>
                                            <td>[{Items[i].Quantity}]</td>
                                            <td>[{Items[i].Rate}]</td>
                                            <td>[{Items[i].SGST}]</td>
                                            <td>[{Items[i].CGST}]</td>
                                            <td>[{Items[i].Cess}]</td>
                                            <td>[{Items[i].Total}]</td>
                                        </tr>";

            invoiceItemTemplate = invoiceItemTemplate.Replace("[{Items[i].LineNumber}]", invoiceItemIndex.ToString());
            invoiceItemTemplate = invoiceItemTemplate.Replace("[{Items[i].Description}]", item.Description);
            invoiceItemTemplate = invoiceItemTemplate.Replace("[{Items[i].HSN_SAC}]", item.HSN_SAC);
            invoiceItemTemplate = invoiceItemTemplate.Replace("[{Items[i].Quantity}]", item.Quantity.ToString());
            invoiceItemTemplate = invoiceItemTemplate.Replace("[{Items[i].Rate}]", item.Rate.ToCurrency());
            invoiceItemTemplate = invoiceItemTemplate.Replace("[{Items[i].Total}]", itemTotal.ToCurrency());

            invoiceItemIndex++;
            invoiceItemTotal += itemTotal;


            invoiceItemTemplate = invoiceItemTemplate.Replace("[{Items[i].SGST}]", BuildTaxHtml(itemTotal, item.SGST));
            invoiceItemTemplate = invoiceItemTemplate.Replace("[{Items[i].CGST}]", BuildTaxHtml(itemTotal, item.CGST));
            invoiceItemTemplate = invoiceItemTemplate.Replace("[{Items[i].Cess}]", BuildTaxHtml(itemTotal, item.Cess));

            invoiceItems += invoiceItemTemplate;
        }


        invoiceItems += @"<tr>
            <td class='border-0 p-0 m-0' colspan='8'></td>
        </tr>";

        invoiceItems += @$"<tr>
            <td class='border-0' colspan='5'></td>
            <td class='text-right' colspan='2'>Sub Total</td>
            <td colspan='2'>{invoiceItemTotal.ToCurrency()}</td>
        </tr>";


        // SGST grouped
        var sgstCollection = invoiceVM.Items
            .GroupBy(x => x.SGST)
            .Select(g => ($"SGST ({g.Key}%)", g.Sum(i => ((i.Rate * i.Quantity * i.SGST) / 100).ToFormat())))
            .Where(x => x.Item2 > 0)
            .ToList();

        // CGST grouped
        var cgstCollection = invoiceVM.Items
            .GroupBy(x => x.CGST)
            .Select(g => ($"CGST ({g.Key}%)", g.Sum(i => ((i.Rate * i.Quantity * i.CGST) / 100).ToFormat())))
            .Where(x => x.Item2 > 0)
            .ToList();

        // Cess grouped
        var cessCollection = invoiceVM.Items
            .GroupBy(x => x.Cess)
            .Select(g => ($"Cess ({g.Key}%)", g.Sum(i => ((i.Rate * i.Quantity * i.Cess) / 100).ToFormat())))
            .Where(x => x.Item2 > 0)
            .ToList();

        var maxTaxLength = Math.Max(sgstCollection.Count, Math.Max(cgstCollection.Count, cessCollection.Count));

        for (int i = 0; i < maxTaxLength; i++)
        {
            invoiceItems += BuildTaxGroupRow(
                sgstCollection.Count > i ? sgstCollection[i] : new("Empty", 0),
                cgstCollection.Count > i ? cgstCollection[i] : new("Empty", 0),
                cessCollection.Count > i ? cessCollection[i] : new("Empty", 0)
            );
        }

        var finalAmount = (invoiceItemTotal
           + sgstCollection.Sum(x => x.Item2)
           + cgstCollection.Sum(x => x.Item2)
           + cessCollection.Sum(x => x.Item2))
           .ToFormat();

        invoiceItems += @$"<tr>
            <td class='border-0' colspan='5'></td>
            <td class='text-right' colspan='2'><strong>Total</strong></td>
            <td colspan='2'>
                <strong style='font-size:16px;'>{invoiceVM.Currency + finalAmount.ToCurrency()}</strong>
            </td>
        </tr>";

        htmlContent = htmlContent.Replace("[{InvoiceItems}]", invoiceItems);

        return htmlContent;
    }

    private static string BuildTaxHtml(decimal itemTotal, decimal taxPercent)
    {
        var taxValue = ((itemTotal * taxPercent) / 100).ToCurrency();
        var percentLabel = $"({taxPercent.ToFormat()}%)";

        return $"{taxValue}<br /><span class='text-muted'>{percentLabel}</span>";
    }

    private static string BuildTaxGroupRow(
        (string Label, decimal Amount) sgst,
        (string Label, decimal Amount) cgst,
        (string Label, decimal Amount) cess)
    {
        var html = string.Empty;

        html += BuildTaxRow(sgst.Label, sgst.Amount);
        html += BuildTaxRow(cgst.Label, cgst.Amount);
        html += BuildTaxRow(cess.Label, cess.Amount);

        return html;
    }

    private static string BuildTaxRow(string label, decimal value)
    {
        if (value > 0)
        {
            return @$"<tr>
                <td class='border-0' colspan='5'></td>
                <td class='text-right' colspan='2'>{label}</td>
                <td colspan='2'>{value.ToCurrency()}</td>
            </tr>";
        }

        return string.Empty;
    }
}