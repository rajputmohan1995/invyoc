
function isNumberKey(evt, id) {
    try {
        var charCode = (evt.which) ? evt.which : evt.keyCode;
        var input = document.getElementById(id);
        var value = input.value;

        // Allow only one dot (.)
        if (charCode == 46) {
            if (value.indexOf('.') === -1) {
                return true;
            } else {
                return false;
            }
        }

        // Allow digits (0-9)
        if (charCode >= 48 && charCode <= 57) {
            // If dot exists, check decimal length
            var dotIndex = value.indexOf('.');
            if (dotIndex > -1) {
                var decimals = value.substring(dotIndex + 1);
                // Prevent entering more than 2 decimal places
                if (input.selectionStart > dotIndex && decimals.length >= 2) {
                    return false;
                }
            }
            return true;
        }

        // Block other characters
        return false;

    } catch (e) {
        console.log(e);
        return false;
    }
}

function toINRCurrency(val) {
    return Number(val).toLocaleString('en-IN', {
        minimumFractionDigits: 2,
        maximumFractionDigits: 2
    });
}

function round2Decimals(num, precision = 2) {
    return parseFloat(num).toFixed(precision);
}

function sortAndCombinArray(data) {
    const result = Object.values(
        data.reduce((acc, item) => {
            if (!acc[item.value])
                acc[item.value] = { ...item }; // create new entry
            else acc[item.value].total += item.total; // aggregate total

            return acc;
        }, {})
    );

    return result;
}

const STORAGE_KEY = "invoiceDraft";



// Save invoice data to storage

function saveDraft() {

    const fields = document.querySelectorAll("[data-save]");

    let data = {}, indexNum = 1;

    fields.forEach(f => data[f.id] = f.value);


    // Handle dynamic items
    const items = [];
    document.querySelectorAll(".line-item").forEach(row => {

        const item = {
            lineNumber: indexNum,
            description: row.querySelector(".item-desc")?.value,
            hSN_SAC: row.querySelector(".item-hsn")?.value,
            qty: row.querySelector(".item-qty")?.value,
            quantity: row.querySelector(".item-rate")?.value,
            rate: row.querySelector(".item-price")?.value,
            sgst: row.querySelector(".item-sgst")?.value,
            cgst: row.querySelector(".item-cgst")?.value,
            cess: row.querySelector(".item-cess")?.value,

        };

        items.push(item);
        indexNum++;
    });

    data.items = items;

    localStorage.setItem(STORAGE_KEY, JSON.stringify(data));
}

// Auto-save on typing or change
document.addEventListener("input", function (e) {
    if (e.target.matches("[data-save]") || e.target.matches(".item-field")) {
        saveDraft();
    }
});