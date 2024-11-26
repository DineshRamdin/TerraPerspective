

function togglePasswordVisibility(inputId) {
    console.log("Data : ",inputId);
    const input = document.getElementById(inputId);
    console.log("input.type : ", input.type);
    // Check the current type and toggle
    if (input.type === 'password') {
        input.type = 'text';
    } else {
        input.type = 'password';
    }
}
function showModal() {
    $('#ChangePassword').modal('show'); // Bootstrap's modal function
}

function validatePasswordStrength(password) {
    const minLength = 8;
    const regex = /^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[!@#$%^&*])[A-Za-z\d!@#$%^&*]{8,}$/;

    if (password.length < minLength) {
        toastr.error("Password must be at least 8 characters long.");
        return false;
    }

    if (!regex.test(password)) {
        toastr.error("Password must contain:\n- At least 1 uppercase letter\n- At least 1 lowercase letter\n- At least 1 number\n- At least 1 special character (!@#$%^&*)");
        return false;
    }

    return true;
}

function checkPassword() {
    const password = document.getElementById("NewPassword").value;
    return validatePasswordStrength(password);
}

function validatePassword() {
    const newPassword = document.getElementById("NewPassword").value;
    const confirmPassword = document.getElementById("ConfirmPassword").value;
    if (newPassword == '' && confirmPassword == '') {
        toastr.error("Please Fill In Mandatory Fields.");
    }
    else if (newPassword === confirmPassword) {
        return true;
    } else {
        toastr.error("The password you entered do not match. Please re-enter your password");
       
    }
}


function ChangeUserPassword() {
    var dataObject = {};
    dataObject.NewPassword = $('#NewPassword').val();
    dataObject.ConfirmPassword = $('#ConfirmPassword').val();

    if (validatePassword() && checkPassword()) {
        $.ajax({
            url: ChangeUserPasswordUrl,
            type: "POST",
            data: dataObject,  // Send data if needed
            beforeSend: function () {

            },
            success: function (response) {
                if (response.data) {
                    toastr.success(response.errorMessage);
                    $('#ChangePassword').modal('hide');
                }
                else {
                    toastr.error(response.errorMessage);
                }
                // Handle success (e.g., update UI with response data)
            },
            error: function (error) {
                console.log("Error:", error);
                toastr.error("Error:" + error);
            }
        }).done(function () {

        });
    }
}