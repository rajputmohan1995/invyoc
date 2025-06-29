function adjustHeight() {
    document.body.style.height = 'auto';
    let contentHeight = document.documentElement.scrollHeight;
    document.body.style.height = contentHeight + 'px';
}

window.addEventListener('load', adjustHeight);
window.addEventListener('resize', adjustHeight);

$("#spanCurrentYear").text(new Date().getFullYear());