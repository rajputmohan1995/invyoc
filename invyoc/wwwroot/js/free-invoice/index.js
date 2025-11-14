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

        finalTotal += subtotal + sgstAmt + cgstAmt + cessAmt;

        $(this).find('.lbl-sgst').text(round2Decimals(sgstAmt));
        $(this).find('.lbl-cgst').text(round2Decimals(cgstAmt));
        $(this).find('.lbl-cess').text(round2Decimals(cessAmt));
        $(this).find('.item-total').text(round2Decimals(total));

        let sgstName = `SGST (${sgst}%)`
        sgstArr.push({ name: sgstName, total: sgstAmt, value: sgst });

        let cgstName = `CGST (${cgst}%)`
        cgstArr.push({ name: cgstName, total: cgstAmt, value: cgst });

        let cessName = `Cess (${cess}%)`
        cessArr.push({ name: cessName, total: cessAmt, value: cess });
    });

    $('#lblSubTotalValue').text(round2Decimals(subtotal));
    $('#lblFinalTotalValue').text(toINRCurrency(finalTotal));

    $(".total-tax").remove();

    for (let i = 0; i < sgstArr.length; i++) {
        if (sgstArr[i].value > 0) {
            $("#trFinalTotalDetails").before(`<tr class="total-tax">
                                            <td><span>${sgstArr[i].name}</span></td>
                                            <td><span>${round2Decimals(sgstArr[i].total)}</span></td>
                                        </tr>`);
        }
        if (cgstArr[i].value > 0) {
            $("#trFinalTotalDetails").before(`<tr class="total-tax">
                                            <td><span>${cgstArr[i].name}</span></td>
                                            <td><span>${round2Decimals(cgstArr[i].total)}</span></td>
                                        </tr>`);
        }
        if (cessArr[i].value > 0) {
            $("#trFinalTotalDetails").before(`<tr class="total-tax">
                                            <td></strong>${cessArr[i].name}</strong></td>
                                            <td>${round2Decimals(cessArr[i].total)}</td>
                                        </tr>`);
        }
    }

    const discountRate = parseFloat($('#discountRate').val()) || 0;
    const taxRate = parseFloat($('#taxRate').val()) || 0;
    const discount = subtotal * (discountRate / 100);
    const tax = (subtotal - discount) * (taxRate / 100);
    const grandTotal = subtotal - discount + tax;

    $('#subtotal').text(currency + toINRCurrency(subtotal));
    $('#discountAmount').text(currency + toINRCurrency(discount));
    $('#taxAmount').text(currency + toINRCurrency(tax));
    $('#grandTotal').text(currency + toINRCurrency(grandTotal));
}

$(document).on('input', '.item-qty, .item-price, .item-sgst, .item-cgst, .item-cess, #taxRate, #discountRate, #currency', updateTotals);

$('#addRow').click(function () {
    var currentRowValue = $('#invoiceTable tbody tr').length - 1;
    var newRowValue = currentRowValue + 1;
    var newRowIndex = newRowValue - 1;

    //<td><span name="Items[${newRowIndex}].LineNumber">${newRowValue}</span></td>

    const row = `<tr class="line-item">
        <td class="align-top">
            <input type="text" class="item-desc mb-1 w-100"
                   placeholder="Enter item name/description"
                   name="Items[${newRowIndex}].Description"
                   id="Items_${newRowIndex}__Description" />
            <input type="text" class="item-hsn mb-1"
                   name="Items[${newRowIndex}].HSN_SAC"
                   placeholder="HSN/SAC"
                   id="Items_${newRowIndex}__HSN_SAC" />
        </td>
        <td class="align-top">
            <input type="text" class="item-qty mb-1 w-100"
                placeholder="0"         
                value="0"
                name="Items[${newRowIndex}].Quantity"
                id="Items_${newRowIndex}__Quantity"
                onkeypress="return isNumberKey(event,this.id)" />
        </td>
        <td class="align-top">
            <input type="text" class="item-price w-100"
                placeholder="0"                
                value="0"
                name="Items[${newRowIndex}].Rate"
                id="Items_${newRowIndex}__Rate"
                onkeypress="return isNumberKey(event,this.id)" />
        </td>

        <td class="align-top">
            <input type="text" class="item-sgst w-100"
                placeholder="0"
                value="0"
                name="Items[${newRowIndex}].SGST"
                id="Items_${newRowIndex}__SGST"
                onkeypress="return isNumberKey(event,this.id)" />
            <small class="text-muted lbl-sgst" id="Label_${newRowIndex}__SGST">0.00</small>
        </td>
        <td class="align-top">
            <input type="text" class="item-cgst w-100"
                placeholder="0"
                value="0"
                name="Items[${newRowIndex}].CGST"
                id="Items_${newRowIndex}__CGST"
                onkeypress="return isNumberKey(event,this.id)" />
            <small class="text-muted lbl-cgst" id="Label_${newRowIndex}__CGST">0.00</small>
        </td>
        <td class="align-top">
            <input type="text" class="item-cess w-100"
                placeholder="0"
                value="0"
                name="Items[${newRowIndex}].Cess"
                id="Items_${newRowIndex}__Cess"
                onkeypress="return isNumberKey(event,this.id)" />
            <small class="text-muted lbl-cess" id="Label_${newRowIndex}__Cess">0.00</small>
        </td>

        <td class="item-total text-end align-top">0.00</td>
        <td class="text-end align-top">
            <button class="btn btn-danger btn-sm p-0 px-1 remove-row rounded-5">  
                <span aria-hidden="true">×</span>
            </button>
        </td>
      </tr>`;

    $('#invoiceTable tbody tr#lastRow').before(row);
    updateTotals();
});

$(document).on('click', '.remove-row', function () {
    $(this).closest('tr').remove();
    reindexInvoiceRows();
    updateTotals();
});

$(document).ready(function () {
    updateTotals();
    textarea_auto_grow(document.getElementById('txtPaymentNotes'));
});

function textarea_auto_grow(element) {
    element.style.height = "5px";
    element.style.height = (element.scrollHeight) + "px";
}

function reindexInvoiceRows() {
    const rows = document.querySelectorAll("table#invoiceTable tbody tr");

    rows.forEach((row, index) => {
        //row.querySelector("td:first-child span").textContent = index + 1;

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

function setInvoiceNumWidthAsPerContent() {
    var incoiceNumElem = document.getElementById("InvoiceNumber");

    const allowedCharacters = "0123456789azertyuiopqsdfghjklmwxcvbnAZERTYUIOPQSDFGHJKLMWXCVBNzáàâãéèêíïóôõöúçñÁÀÂÃÉÈÍÏÓÔÕÖÚÇÑ "; // You can add any other character in the same way
    incoiceNumElem.value = incoiceNumElem.value.split('').filter(char => allowedCharacters.includes(char)).join('');

    incoiceNumElem.style.width = ((incoiceNumElem.value.length + 1) * 12) + 'px';
}


$("#previewInvoice").on('click', function () {

    var formData = $("#frmInvoice").serializeArray();

    $("#previewInvoice").text("Please Wait...");
    $("#previewInvoice").attr("disabled", "disabled");

    previewInvoicePdf(formData);
});

function previewInvoicePdf(invoiceData) {

    $.ajax({
        url: '/preview-invoice',
        type: 'POST',
        data: invoiceData,
        success: function (blob) {

            enablePreviewButton();
            $('#pdfFrame').html(blob);
            $('#pdfPreviewModal').modal('show');

        },
        error: function (xhr, status, error) {
            enablePreviewButton();
            alert('Failed to load Invoice preview.');
            console.error(error);
        },
    });
}

function enablePreviewButton() {
    $("#previewInvoice").html('<i class="fa-solid fa-file-pdf"></i>Preview Invoice');
    $("#previewInvoice").removeAttr("disabled");
}

$("#btnDownloadInvoice").on('click', function () {

    $("#btnDownloadInvoice").text("Please Wait...");
    $("#btnDownloadInvoice").attr("disabled", "disabled");

    $("#hdnDownloadInvoice").show();
    $("#hdnDownloadInvoice").click();

    setTimeout(function () {
        enableDownloadButton();
        $("#hdnDownloadInvoice").hide();
        $("#divInvoiceDownload").show();

        setTimeout(function () {
            $("#divInvoiceDownload").hide();
        }, 5000);
    }, 1500);
});

function enableDownloadButton() {
    $("#btnDownloadInvoice").html('<i class="fa fa-download"></i> Download Invoice');
    $("#btnDownloadInvoice").removeAttr("disabled");
}