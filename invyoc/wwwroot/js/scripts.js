
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