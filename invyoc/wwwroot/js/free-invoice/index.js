function updateTotals() {
    let subtotal = 0;
    let currency = $('#currency').val();
    $('#invoiceTable tbody tr').each(function () {
        const qty = parseFloat($(this).find('.item-qty').val()) || 0;
        const price = parseFloat($(this).find('.item-price').val()) || 0;
        const total = qty * price;
        $(this).find('.item-total').text(currency + total.toFixed(2));
        subtotal += total;
    });
    const discountRate = parseFloat($('#discountRate').val()) || 0;
    const taxRate = parseFloat($('#taxRate').val()) || 0;
    const discount = subtotal * (discountRate / 100);
    const tax = (subtotal - discount) * (taxRate / 100);
    const grandTotal = subtotal - discount + tax;
    $('#subtotal').text(currency + subtotal.toFixed(2));
    $('#discountAmount').text(currency + discount.toFixed(2));
    $('#taxAmount').text(currency + tax.toFixed(2));
    $('#grandTotal').text(currency + grandTotal.toFixed(2));
}

function updateSerialNumber() {
    var index = 1;
    $('#invoiceTable tbody tr').each(function () {
        $(this).find("td:first").text(index);
        index++;
    });
}

$(document).on('input', '.item-qty, .item-price, #taxRate, #discountRate, #currency', updateTotals);

$('#addRow').click(function () {
    var currentRowValue = $('#invoiceTable tbody tr').length;
    var newRowValue = currentRowValue + 1;

    const row = `<tr>
        <td><span name="Items[${newRowValue - 1}].LineNumber">${newRowValue}</span></td>
        <td><input type="text" class="form-control form-control-sm item-desc" value="New Item"></td>
        <td><input type="number" class="form-control form-control-sm item-qty" value="1"></td>
        <td><input type="number" class="form-control form-control-sm item-price" value="100"></td>
        <td class="item-total">$100.00</td>
        <td><button class="btn btn-danger btn-sm remove-row">×</button></td>
      </tr>`;
    $('#invoiceTable tbody').append(row);
    updateTotals();
});

$(document).on('click', '.remove-row', function () {
    $(this).closest('tr').remove();
    updateSerialNumber();
    updateTotals();
});



$('#download').click(function () {
    const element = document.getElementById('invoice');
    html2pdf().from(element).save('invoice.pdf');
});

$('#logoUpload').change(function (e) {
    const reader = new FileReader();
    reader.onload = function (event) {
        $('#logoContainer').html(`<img src="${event.target.result}" class="logo-preview" />`);
    };
    reader.readAsDataURL(e.target.files[0]);
});

$(document).ready(updateTotals);
