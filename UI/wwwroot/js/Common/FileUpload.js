async function FiletoBase64string(id) {
    var fileInput = $('#' + id)[0];
    var file = fileInput.files[0];
    if (file) {
        return await readFileAsBase64(file)
    }
    else {
        return ""
    }
}

async function readFileAsBase64(file) {

    return new Promise((resolve, reject) => {
        const reader = new FileReader();
        reader.onload = function (event) {
            resolve(event.target.result.split(',')[1]);
        };
        reader.onerror = function (error) {
            reject(error);
        };
        reader.readAsDataURL(file);
    });
}

function UploadFileisValidImage(id) {
    var fileInput = $('#' + id)[0];
    var file = fileInput.files[0];

    if (file) {
        const allowedTypes = ['image/jpeg', 'image/png', 'image/bmp', 'image/gif'];
        if (!allowedTypes.includes(file.type)) {
            toastr.warning("Please upload a valid file in one of the following formats: JPEG, PNG, BMP, or GIF.");
            $('#' + id).val(null); // Clear the file input
        } 
    }
}

function UploadFileisValidImageFormarJPEGAndPNG(id) {
    var fileInput = $('#' + id)[0];
    var file = fileInput.files[0];

    if (file) {
        const allowedTypes = ['image/jpeg', 'image/png'];
        if (!allowedTypes.includes(file.type)) {
            toastr.warning("Please upload a valid file in one of the following formats: JPEG,or PNG.");
            $('#' + id).val(null); // Clear the file input
        }
    }
}

function UploadFileisValidImageFormarJPEGAndPNG(id) {
    var fileInput = $('#' + id)[0];
    var file = fileInput.files[0];

    if (file) {
        const allowedTypes = ['image/jpeg', 'image/png'];
        if (!allowedTypes.includes(file.type)) {
            toastr.warning("Please upload a valid file in one of the following formats: JPEG,or PNG.");
            $('#' + id).val(null); // Clear the file input
        }
    }
}

function UploadFileisValidExcel(id) {
    var fileInput = $('#' + id)[0];
    var file = fileInput.files[0];

    if (file) {
        const allowedTypes = [
            'application/vnd.ms-excel',           // .xls
            'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet', // .xlsx
            'text/csv'                            // .csv
        ];
        if (!allowedTypes.includes(file.type)) {
            toastr.warning("Please upload a valid Excel file: XLS, XLSX, or CSV.");
            $('#' + id).val(null); // Clear the file input
        }
    }
}