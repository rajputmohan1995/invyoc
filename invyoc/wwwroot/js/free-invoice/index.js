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
        <td><input type="text" class="form-control form-control-sm rounded-0 item-desc" value="New Item" name="Items[${newRowIndex}].Description" id="Items_${newRowIndex}__Description" /></td>
        <td><input type="number" class="form-control form-control-sm rounded-0 item-qty" value="1" name="Items[${newRowIndex}].Quantity" id="Items_${newRowIndex}__Quantity" /></td>
        <td><input type="text" class="form-control form-control-sm rounded-0 item-price" value="100" name="Items[${newRowIndex}].Rate" id="Items_${newRowIndex}__Rate" onkeypress="return isNumberKey(event,this.id)" /></td>
        <td class="item-total">$100.00</td>
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

$('#logoUpload').change(function (e) {
    const reader = new FileReader();
    reader.onload = function (event) {
        $('#logoContainer').html(`<img loading="lazy" src="${event.target.result}" class="logo-preview" />`);
    };
    reader.readAsDataURL(e.target.files[0]);
});

$(document).ready(function () {
    updateTotals();
    textarea_auto_grow(document.getElementById('txtPaymentNotes'));
    setInvoiceWidthAsPerContent();
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


$('#download').click(function () {
    setTimeout(function () {
        $("form").prepend('<div class="alert alert-success text-center p-1" role="alert">' +
            'Invoice Downloaded Successfully.' +
            '</div>');
    }, 1000);

    setTimeout(function () {
        $("div.alert.alert-success").remove()
    }, 6000)
});


function setInvoiceWidthAsPerContent() {
    var incoiceNumElem = document.getElementById("InvoiceNumber");

    const allowedCharacters = "0123456789azertyuiopqsdfghjklmwxcvbnAZERTYUIOPQSDFGHJKLMWXCVBNzáàâãéèêíïóôõöúçñÁÀÂÃÉÈÍÏÓÔÕÖÚÇÑ "; // You can add any other character in the same way
    incoiceNumElem.value = incoiceNumElem.value.split('').filter(char => allowedCharacters.includes(char)).join('');

    incoiceNumElem.style.width = ((incoiceNumElem.value.length + 1) * 12) + 'px';
}
