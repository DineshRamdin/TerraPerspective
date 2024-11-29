

function togglePasswordVisibility(inputId) {
    const input = document.getElementById(inputId);
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
                    setTimeout(
                        function () {
                            window.location.href = LogOutURLForCP;
                        }, 500);
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

function loadMenu() {
    //const roleId = localStorage.getItem("RoleId");
    //const userToken = localStorage.getItem("userToken");

    //const requestData = {
    //    Id: roleId,
    //    UserId: userToken
    //};

    // AJAX call to fetch menu data
    $.ajax({
        type: "POST",
        url: GetMenuUrl, // Adjust to your controller and action
        data: null,
        success: function (response) {
            renderDynamicMenu(response.data);
        },
        error: function (error) {
            console.error("Error loading menu:", error);
        }
    });
}

function renderDynamicMenu(data) {
    const menuContainer = $(".sidebar nav ul");
    menuContainer.empty(); // Clear the existing menu

    data.forEach(grandParent => {
        // Build grandparent menu item
        let grandParentHtml = `
                    <li class="nav-item">
                        <a href="${ grandParent.url ? '/' + grandParent.url : '#'}" class="nav-link">
                            <i class="nav-icon ${grandParent.icon}"></i>
                            <p>
                                ${grandParent.name}
                                ${grandParent.subMenu.length ? '<i class="fas fa-angle-right right"></i>' : ''}
                            </p>
                        </a>
                `;

        // Check for submenus
        if (grandParent.subMenu.length > 0) {
            let parentHtml = '<ul class="nav nav-treeview">';

            grandParent.subMenu.forEach(parent => {
                parentHtml += `
                            <li class="nav-item">
                                <a href="${parent.url ? '/' + parent.url : '#'}" class="nav-link">
                                    <i class="${parent.icon || 'far fa-circle nav-icon'}"></i>
                                    <p>${parent.name}</p>
                                </a>
                        `;

                // Check for child menus
                if (parent.child.length > 0) {
                    parentHtml += '<ul class="nav nav-treeview">';

                    parent.child.forEach(child => {
                        parentHtml += `
                                    <li class="nav-item">
                                        <a href="${child.url ? '/' + child.url : '#'}" class="nav-link">
                                            <i class="${child.icon || 'far fa-dot-circle nav-icon'}"></i>
                                            <p>${child.name}</p>
                                        </a>
                                    </li>
                                `;
                    });

                    parentHtml += '</ul>'; // Close child menu
                }

                parentHtml += '</li>'; // Close parent menu
            });

            parentHtml += '</ul>'; // Close parent list
            grandParentHtml += parentHtml;
        }

        grandParentHtml += '</li>'; // Close grandparent menu
        menuContainer.append(grandParentHtml);
    });

    // Initialize toggle behavior
    initMenuToggle();
}

function initMenuToggle() {
    $(".nav-item > a").click(function (e) {
        const submenu = $(this).next(".nav-treeview");
        if (submenu.length) {
            e.preventDefault();
            submenu.slideToggle();
            $(this).find(".right").toggleClass("fa-angle-right fa-angle-down");
        }
    });
}


