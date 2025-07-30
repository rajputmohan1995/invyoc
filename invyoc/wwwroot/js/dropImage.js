const dropZone = document.getElementById('logoDropZone');
const fileInput = document.getElementById('logoInput');
const previewImg = document.getElementById('logoPreview');

// Trigger input on click
dropZone.addEventListener('click', () => fileInput.click());

// Handle input change
fileInput.addEventListener('change', handleFile);

// Drag-over styles
dropZone.addEventListener('dragover', e => {
    e.preventDefault();
    dropZone.classList.add('hover');
});
dropZone.addEventListener('dragleave', () => dropZone.classList.remove('hover'));

// Handle drop
dropZone.addEventListener('drop', e => {
    e.preventDefault();
    dropZone.classList.remove('hover');
    const file = e.dataTransfer.files[0];
    fileInput.files = e.dataTransfer.files;
    handleFile();
});

function handleFile() {

    if (fileInput && fileInput.files && fileInput.files.length > 0) {

        const file = fileInput.files[0];
        if (file && file.type.startsWith('image/')) {
            const reader = new FileReader();
            reader.onload = e => {

                compressBase64Image(e.target.result).then((smallBase64) => {
                    previewImg.src = smallBase64;
                    previewImg.style.display = 'block';

                    $("#companyLogoBase64").val(smallBase64);
                    $("#logoDropZone p").addClass("d-none");
                    $("#logoDropZone").addClass("border-0");
                });
            };
            reader.readAsDataURL(file);
        }
    }
}

function compressBase64Image(base64, maxWidth = 300) {
    return new Promise((resolve) => {
        const img = new Image();
        img.src = base64;
        img.onload = () => {
            const canvas = document.createElement("canvas");
            const scale = maxWidth / img.width;
            canvas.width = maxWidth;
            canvas.height = img.height * scale;

            const ctx = canvas.getContext("2d");
            ctx.drawImage(img, 0, 0, canvas.width, canvas.height);

            const compressedBase64 = canvas.toDataURL("image/png", 1);
            resolve(compressedBase64);
        };
    });
}