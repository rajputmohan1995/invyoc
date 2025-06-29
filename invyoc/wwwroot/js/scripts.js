function adjustHeight() {
    document.body.style.height = 'auto';
    let contentHeight = document.documentElement.scrollHeight;
    document.body.style.height = contentHeight + 'px';
}

window.addEventListener('load', adjustHeight);
window.addEventListener('resize', adjustHeight);

$("#spanCurrentYear").text(new Date().getFullYear());

function isNumberKey(evt, id) {
    try {
        var charCode = (evt.which) ? evt.which : event.keyCode;

        if (charCode == 46) {
            var txt = document.getElementById(id).value;
            if (!(txt.indexOf(".") > -1)) {

                return true;
            }
        }
        if (charCode > 31 && (charCode < 48 || charCode > 57))
            return false;

        return true;
    } catch (w) {
        alert(w);
    }
}

function toINRCurrency(val) {
    return Number(val).toLocaleString('en-IN', {
        minimumFractionDigits: 2,
        maximumFractionDigits: 2
    });
}