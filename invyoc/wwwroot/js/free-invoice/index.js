function updateTotals() {
    let subtotal = 0;
    let currency = $('#currency').val();
    $('#invoiceTable tbody tr').each(function () {
        const qty = parseFloat($(this).find('.item-qty').val()) || 0;
        const price = parseFloat($(this).find('.item-price').val()) || 0;
        const total = qty * price;
        $(this).find('.item-total').text(toINRCurrency(total));
        subtotal += total;
    });
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

$(document).on('input', '.item-qty, .item-price, #taxRate, #discountRate, #currency', updateTotals);

$('#addRow').click(function () {
    var currentRowValue = $('#invoiceTable tbody tr').length;
    var newRowValue = currentRowValue + 1;
    var newRowIndex = newRowValue - 1;

    const row = `<tr>
        <td><span name="Items[${newRowIndex}].LineNumber">${newRowValue}</span></td>
        <td><input type="text" class="form-control form-control-sm rounded-0 item-desc"
                placeholder="Enter Item Description here"
                value="Item ${newRowIndex + 1}" 
                name="Items[${newRowIndex}].Description"
                id="Items_${newRowIndex}__Description" /></td>
        <td><input type="number" class="form-control form-control-sm rounded-0 item-qty"
                placeholder="Quantity"         
                value="1"
                name="Items[${newRowIndex}].Quantity" 
                id="Items_${newRowIndex}__Quantity" /></td>
        <td><input type="text" class="form-control form-control-sm rounded-0 item-price"
                placeholder="Rate"                
                value="100"
                name="Items[${newRowIndex}].Rate" 
                id="Items_${newRowIndex}__Rate" 
                onkeypress="return isNumberKey(event,this.id)" /></td>
        <td class="item-total">100.00</td>
        <td><button class="btn btn-danger btn-sm remove-row">×</button></td>
      </tr>`;
    $('#invoiceTable tbody').append(row);
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
        row.querySelector("td:first-child span").textContent = index + 1;

        const descInput = row.querySelector(".item-desc");
        const qtyInput = row.querySelector(".item-qty");
        const priceInput = row.querySelector(".item-price");
        const qtyInvariant = qtyInput?.nextElementSibling;
        const priceInvariant = priceInput?.nextElementSibling;

        if (descInput) {
            descInput.name = `Items[${index}].Description`;
            descInput.id = `Items_${index}__Description`;
        }

        if (qtyInput) {
            qtyInput.name = `Items[${index}].Quantity`;
            qtyInput.id = `Items_${index}__Quantity`;
        }

        if (qtyInvariant && qtyInvariant.name === "__Invariant") {
            qtyInvariant.value = `Items[${index}].Quantity`;
        }

        if (priceInput) {
            priceInput.name = `Items[${index}].Rate`;
            priceInput.id = `Items_${index}__Rate`;
        }

        if (priceInvariant && priceInvariant.name === "__Invariant") {
            priceInvariant.value = `Items[${index}].Rate`;
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