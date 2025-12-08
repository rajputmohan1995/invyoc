
$(function () {
    $("#spanCurrentYear").text(new Date().getFullYear());
    updateTotals();

    $.validator.setDefaults({
        onkeyup: function (element) { $(element).valid(); },
        onfocusout: function (element) { $(element).valid(); }
    });
});


$(document).on('click', '#addRow', function () {
    let allLineItems = $('#invoiceTable tbody tr.line-item');
    var lastLineItemCount = allLineItems.length - 1;

    let lastRowCGST = 0, lastRowSGST = 0, lastRowCess = 0;

    if (lastLineItemCount >= 0) {
        lastRowCGST = $(allLineItems[lastLineItemCount]).find(".item-cgst").val();
        lastRowSGST = $(allLineItems[lastLineItemCount]).find(".item-sgst").val();
        lastRowCess = $(allLineItems[lastLineItemCount]).find(".item-cess").val();
    }

    if (!lastRowCGST) lastRowCGST = "0";
    if (!lastRowSGST) lastRowSGST = "0";
    if (!lastRowCess) lastRowCess = "0";

    addNewLineItem(lastLineItemCount + 1, "", "", "1", "0", lastRowCGST, lastRowSGST, lastRowCess);
});

$(document).on('click', '.remove-row', function () {

    if ($('#invoiceTable tbody tr.line-item').length == 1)
        return;

    $(this).closest('tr').remove();

    reindexInvoiceRows();
    updateTotals();

    setTimeout(function () {
        saveDraft();
    }, 200);
});

$(document).on('input', '.item-qty, .item-price, .item-sgst, .item-cgst, .item-cess, #taxRate, #discountRate, #currency', updateTotals);

$(document).on('change', '#chkSameAsBillTo', function () {

    let isChecked = $("#chkSameAsBillTo").prop('checked');


    if (isChecked) {
        let billToName = $("#BillToName").val().trim();
        let billToGSTNo = $("#BillToGSTNo").val().trim();
        let billToAddress = $("#BillToAddress").val().trim();
        let billToAddressCity = $("#BillToCity").val().trim();
        let billToAddressState = $("#BillToState").val().trim();
        let billToAddressCountry = $("#BillToCountry").val().trim();
        let billToAddressPincode = $("#BillToPinCode").val().trim();
        let billToAddressContactNum = $("#BillToContact").val().trim();

        $("#ShipToName").val(billToName);
        $("#ShipToGSTIN").val(billToGSTNo);
        $("#ShipToAddress").val(billToAddress);
        $("#ShipToCity").val(billToAddressCity);
        $("#ShipToState").val(billToAddressState);
        $("#ShipToCountry").val(billToAddressCountry);
        $("#ShipToPincode").val(billToAddressPincode);
        $("#ShipToContact").val(billToAddressContactNum);

        $("#ShipToName").attr('disabled', 'disabled');
        $("#ShipToGSTIN").attr('disabled', 'disabled');
        $("#ShipToAddress").attr('disabled', 'disabled');
        $("#ShipToCity").attr('disabled', 'disabled');
        $("#ShipToState").attr('disabled', 'disabled');
        $("#ShipToCountry").attr('disabled', 'disabled');
        $("#ShipToPincode").attr('disabled', 'disabled');
        $("#ShipToContact").attr('disabled', 'disabled');
    }
    else {
        $("#ShipToName").removeAttr('disabled', 'disabled');
        $("#ShipToGSTIN").removeAttr('disabled', 'disabled');
        $("#ShipToAddress").removeAttr('disabled', 'disabled');
        $("#ShipToCity").removeAttr('disabled', 'disabled');
        $("#ShipToState").removeAttr('disabled', 'disabled');
        $("#ShipToCountry").removeAttr('disabled', 'disabled');
        $("#ShipToPincode").removeAttr('disabled', 'disabled');
        $("#ShipToContact").removeAttr('disabled', 'disabled');
    }

    let invoiceDraft = JSON.parse(localStorage.getItem("invoiceDraft"));

    if (invoiceDraft) {
        invoiceDraft.ShipToName = $("#ShipToName").val();
        invoiceDraft.ShipToGSTIN = $("#ShipToGSTIN").val();
        invoiceDraft.ShipToAddress = $("#ShipToAddress").val();
        invoiceDraft.ShipToCity = $("#ShipToCity").val();
        invoiceDraft.ShipToState = $("#ShipToState").val();
        invoiceDraft.ShipToCountry = $("#ShipToCountry").val();
        invoiceDraft.ShipToPincode = $("#ShipToPincode").val();
        invoiceDraft.ShipToContactNum = $("#ShipToContact").val();
    }

    localStorage.setItem(STORAGE_KEY, JSON.stringify(invoiceDraft));
});

$(document).on('click', 'button.remove-logo', function (event) {
    event.stopPropagation();
    event.preventDefault();

    $("#companyLogoBase64").val('');
    $("#logoDropZone p").removeClass("d-none");
    $("#logoDropZone").removeClass("border-0");

    $("#logoPreview").hide();
    $("button.remove-logo").hide();

    localStorage.setItem(LOGO_STORAGE_KEY, '');
});


function updateTotals() {
    let subtotal = 0;
    let finalTotal = 0;
    let currency = $('#currency').val();

    let sgstArr = [];
    let cgstArr = [];
    let cessArr = [];

    $('#invoiceTable tbody tr.line-item').each(function () {
        const qty = parseFloat($(this).find('.item-qty').val()) || 0;
        const price = parseFloat($(this).find('.item-price').val()) || 0;
        const total = qty * price;
        subtotal += total;

        const sgst = parseFloat($(this).find('.item-sgst').val()) || 0;
        const cgst = parseFloat($(this).find('.item-cgst').val()) || 0;
        const cess = parseFloat($(this).find('.item-cess').val()) || 0;

        const sgstAmt = parseFloat((total * sgst) / 100);
        const cgstAmt = parseFloat((total * cgst) / 100);
        const cessAmt = parseFloat((total * cess) / 100);

        finalTotal += total + sgstAmt + cgstAmt + cessAmt;

        $(this).find('.lbl-sgst').text(round2Decimals(sgstAmt));
        $(this).find('.lbl-cgst').text(round2Decimals(cgstAmt));
        $(this).find('.lbl-cess').text(round2Decimals(cessAmt));
        $(this).find('.item-total .content').text(round2Decimals(total));

        let sgstName = `SGST (${sgst}%)`
        if (sgst > 0) {
            sgstArr.push({ name: sgstName, total: sgstAmt, value: sgst });
        }

        let cgstName = `CGST (${cgst}%)`
        if (cgst > 0) {
            cgstArr.push({ name: cgstName, total: cgstAmt, value: cgst });
        }

        let cessName = `Cess (${cess}%)`
        if (cess > 0) {
            cessArr.push({ name: cessName, total: cessAmt, value: cess });
        }
    });

    $('#lblSubTotalValue').text(round2Decimals(subtotal));
    $('#lblFinalTotalValue').text(currency + toINRCurrency(finalTotal));

    $(".total-tax").remove();

    updateTaxTotals(sgstArr, cgstArr, cessArr);
}

function updateTaxTotals(sgstArr, cgstArr, cessArr) {

    sgstArr = sortAndCombinArray(sgstArr);
    cgstArr = sortAndCombinArray(cgstArr);
    cessArr = sortAndCombinArray(cessArr);

    const maxLength = Math.max(sgstArr.length, cgstArr.length, cessArr.length);

    for (let i = 0; i < maxLength; i++) {
        if (sgstArr[i] && sgstArr[i].value > 0) {
            $("#trFinalTotalDetails").before(`<tr class="total-tax">
                                            <td><span>${sgstArr[i].name}</span></td>
                                            <td><span>${round2Decimals(sgstArr[i].total)}</span></td>
                                        </tr>`);
        }

        if (cgstArr[i] && cgstArr[i].value > 0) {
            $("#trFinalTotalDetails").before(`<tr class="total-tax">
                                            <td><span>${cgstArr[i].name}</span></td>
                                            <td><span>${round2Decimals(cgstArr[i].total)}</span></td>
                                        </tr>`);
        }
        if (cessArr[i] && cessArr[i].value > 0) {
            $("#trFinalTotalDetails").before(`<tr class="total-tax">
                                            <td></strong>${cessArr[i].name}</strong></td>
                                            <td>${round2Decimals(cessArr[i].total)}</td>
                                        </tr>`);
        }
    }
}


function reindexInvoiceRows() {
    const rows = document.querySelectorAll("table#invoiceTable tbody tr");

    rows.forEach((row, index) => {

        const descInput = row.querySelector(".item-desc");
        if (descInput) {
            descInput.name = `Items[${index}].Description`;
            descInput.id = `Items_${index}__HSN_SAC`;
        }

        const hsnInput = row.querySelector(".item-hsn");
        if (hsnInput) {
            hsnInput.name = `Items[${index}].HSN_SAC`;
            hsnInput.id = `Items_${index}__Description`;
        }

        const qtyInput = row.querySelector(".item-qty");
        if (qtyInput) {
            qtyInput.name = `Items[${index}].Quantity`;
            qtyInput.id = `Items_${index}__Quantity`;
        }

        const qtyInvariant = qtyInput?.nextElementSibling;
        if (qtyInvariant && qtyInvariant.name === "__Invariant") {
            qtyInvariant.value = `Items[${index}].Quantity`;
        }

        const priceInput = row.querySelector(".item-price");
        if (priceInput) {
            priceInput.name = `Items[${index}].Rate`;
            priceInput.id = `Items_${index}__Rate`;
        }

        const priceInvariant = priceInput?.nextElementSibling;
        if (priceInvariant && priceInvariant.name === "__Invariant") {
            priceInvariant.value = `Items[${index}].Rate`;
        }

        const sgstInput = row.querySelector(".item-sgst");
        if (sgstInput) {
            sgstInput.name = `Items[${index}].SGST`;
            sgstInput.id = `Items_${index}__SGST`;
        }

        const sgstInvariant = sgstInput?.nextElementSibling;
        if (sgstInvariant && sgstInvariant.name === "__Invariant") {
            sgstInvariant.value = `Items[${index}].SGST`;
        }

        const cgstInput = row.querySelector(".item-cgst");
        if (cgstInput) {
            cgstInput.name = `Items[${index}].CGST`;
            cgstInput.id = `Items_${index}__CGST`;
        }

        const cgstInvariant = cgstInput?.nextElementSibling;
        if (cgstInvariant && cgstInvariant.name === "__Invariant") {
            cgstInvariant.value = `Items[${index}].CGST`;
        }

        const cessInput = row.querySelector(".item-cess");
        if (cessInput) {
            cessInput.name = `Items[${index}].Cess`;
            cessInput.id = `Items_${index}__Cess`;
        }

        const cessInvariant = cessInput?.nextElementSibling;
        if (cessInvariant && cessInvariant.name === "__Invariant") {
            cgstInvariant.value = `Items[${index}].Cess`;
        }
    });
}

function addNewLineItem(lineNum, desc = "", hsn = "", qty = "1", rate = "100", cgst = "0", sgst = "0", cess = "0") {

    let lineItem = {
        LineNumber: lineNum,
        Description: desc,
        HSN_SAC: hsn,
        Quantity: qty,
        Rate: rate,
        SGST: sgst,
        CGST: cgst,
        Cess: cess
    }

    $("button#addRow").attr("disabled", "disabled");

    $.ajax({
        url: '/Invoice/NewLineItem',
        type: 'POST',
        data: lineItem,
        success: function (lineItemHtmlContent) {
            $('#invoiceTable tbody tr#lastRow').before(lineItemHtmlContent);

            setTimeout(function () {
                updateTotals();
                $("button#addRow").removeAttr("disabled")
            }, 200);
        }
    });

}

$("#printInvoice").on('click', function () {
    invoiceFormSubmit("true")
});

$("#downloadInvoice").on('click', function () {
    invoiceFormSubmit("false")
});

function invoiceFormSubmit(isPreviewValue) {
    $("#hdnIsPreview").val(isPreviewValue);
    $("form#frmInvoice").submit();
}