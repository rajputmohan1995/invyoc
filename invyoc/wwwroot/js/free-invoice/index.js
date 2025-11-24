
$(function () {
    $("#spanCurrentYear").text(new Date().getFullYear());
    updateTotals();
});


$(document).on('click', '#addRow', function () {
    addNewLineItem();
});

$(document).on('click', '.remove-row', function () {

    if ($('#invoiceTable tbody tr.line-item').length == 1)
        return;

    $(this).closest('tr').remove();
    reindexInvoiceRows();
    updateTotals();
});

$(document).on('input', '.item-qty, .item-price, .item-sgst, .item-cgst, .item-cess, #taxRate, #discountRate, #currency', updateTotals);

$(document).on('change', '#chkSameAsBillTo', function () {

    let isChecked = $("#chkSameAsBillTo").prop('checked');

    if (isChecked) {
        let billToAddress = $("#BillTo_ClientAddress_Address").val().trim();
        let billToAddressCity = $("#BillTo_ClientAddress_City").val().trim();
        let billToAddressState = $("#BillTo_ClientAddress_State").val().trim();
        let billToAddressCountry = $("#BillTo_ClientAddress_Country").val().trim();
        let billToAddressContactNum = $("#BillTo_ClientAddress_ContactNum").val().trim();

        $("#ShipTo_Address").val(billToAddress);
        $("#ShipTo_City").val(billToAddressCity);
        $("#ShipTo_State").val(billToAddressState);
        $("#ShipTo_Country").val(billToAddressCountry);
        $("#ShipTo_ContactNum").val(billToAddressContactNum);

        $("#ShipTo_Address").attr('disabled', 'disabled');
        $("#ShipTo_City").attr('disabled', 'disabled');
        $("#ShipTo_State").attr('disabled', 'disabled');
        $("#ShipTo_Country").attr('disabled', 'disabled');
        $("#ShipTo_ContactNum").attr('disabled', 'disabled');
    }
    else {
        $("#ShipTo_Address").removeAttr('disabled');
        $("#ShipTo_City").removeAttr('disabled');
        $("#ShipTo_State").removeAttr('disabled');
        $("#ShipTo_Country").removeAttr('disabled');
        $("#ShipTo_ContactNum").removeAttr('disabled');
    }
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
        $(this).find('.item-total').text(round2Decimals(total));

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

function addNewLineItem(desc = "", hsn = "", rate = "100", qty = "1", sgst = "0", cgst = "0", cess = "0") {

    var currentRowValue = $('#invoiceTable tbody tr').length - 1;
    var newRowValue = currentRowValue + 1;
    var newRowIndex = newRowValue - 1;


    const row = `<tr class="line-item">
        <td class="align-top">
            <input type="text" class="item-desc mb-1 w-100"
                   placeholder="Enter item name/description"
                   name="Items[${newRowIndex}].Description"
                   id="Items_${newRowIndex}__Description"
                   value="${desc}" />
            <input type="text" class="item-hsn mb-1"
                   name="Items[${newRowIndex}].HSN_SAC"
                   placeholder="HSN/SAC"
                   id="Items_${newRowIndex}__HSN_SAC"
                   value="${hsn}" />
        </td>
        <td class="align-top">
            <input type="text" class="item-qty mb-1 w-100"
                placeholder="0"         
                name="Items[${newRowIndex}].Quantity"
                id="Items_${newRowIndex}__Quantity"
                onkeypress="return isNumberKey(event,this.id)"
                value="${qty}" />
        </td>
        <td class="align-top">
            <input type="text" class="item-price w-100"
                placeholder="0"                
                name="Items[${newRowIndex}].Rate"
                id="Items_${newRowIndex}__Rate"
                onkeypress="return isNumberKey(event,this.id)"
                value="${rate}" />
        </td>

        <td class="align-top">
            <input type="text" class="item-sgst w-100"
                placeholder="0"
                name="Items[${newRowIndex}].SGST"
                id="Items_${newRowIndex}__SGST"
                onkeypress="return isNumberKey(event,this.id)"
                value="${sgst}" />
            <small class="text-muted lbl-sgst" id="Label_${newRowIndex}__SGST">0.00</small>
        </td>
        <td class="align-top">
            <input type="text" class="item-cgst w-100"
                placeholder="0"
                name="Items[${newRowIndex}].CGST"
                id="Items_${newRowIndex}__CGST"
                onkeypress="return isNumberKey(event,this.id)"
                value="${cgst}" />
            <small class="text-muted lbl-cgst" id="Label_${newRowIndex}__CGST">0.00</small>
        </td>
        <td class="align-top">
            <input type="text" class="item-cess w-100"
                placeholder="0"
                name="Items[${newRowIndex}].Cess"
                id="Items_${newRowIndex}__Cess"
                onkeypress="return isNumberKey(event,this.id)"
                value="${cess}" />
            <small class="text-muted lbl-cess" id="Label_${newRowIndex}__Cess">0.00</small>
        </td>

        <td class="item-total text-end align-top">0.00</td>
        <td class="text-end align-top">
            <button class="btn btn-sm btn-outline-danger remove-row px-1 py-0" type="button">
                <i class="fa fa-times" aria-hidden="true"></i>
            </button>    
        </td>
      </tr>`;

    $('#invoiceTable tbody tr#lastRow').before(row);

    updateTotals();
}

$("#printInvoice").on('click', function () {
    $("#hdnIsPreview").val("true");
    $("form#frmInvoice").submit();

});

$("#downloadInvoice").on('click', function () {
    $("#hdnIsPreview").val("false");
    $("form#frmInvoice").submit();

});