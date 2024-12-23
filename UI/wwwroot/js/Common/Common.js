

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

function loadMenu(langResource) {
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
            renderDynamicMenu(response.data, langResource);
        },
        error: function (error) {
            console.error("Error loading menu:", error);
        }
    });
}

function renderDynamicMenu(data, langResource) {

    const menuContainer = $(".sidebar nav ul");
    menuContainer.empty(); // Clear the existing menu
    data.forEach(grandParent => {

        var MenuName = "";
        if (grandParent.name == "Dashboard") {
            MenuName = langResource.DashboardMenuLabel;
        }
        else if (grandParent.name == "User") {
            MenuName = langResource.UserMenuLabel;
        }
        else if (grandParent.name == "Poster") {
            MenuName = langResource.PosterMenuLabel;
        }
        else if (grandParent.name == "Carousel") {
            MenuName = langResource.CarouselMenuLabel;
        }
        else if (grandParent.name == "Device") {
            MenuName = langResource.DeviceMenuLabel;
        }
        else if (grandParent.name == "LookUpType") {
            MenuName = langResource.LookUpTypeMenuLabel;
        }
        else if (grandParent.name == "LookUpValue") {
            MenuName = langResource.LookUpValueMenuLabel;
        }
        else if (grandParent.name == "Log") {
            MenuName = langResource.LogMenuLabel;
        }
        else if (grandParent.name == "Room") {
            MenuName = langResource.RoomMenuLabel;
        }
        else if (grandParent.name == "Resource") {
            MenuName = langResource.ResourceMenuLabel;
        }
        else if (grandParent.name == "Check In/Out") {
            MenuName = langResource.CheckInOutMenuLabel;
        }
        else if (grandParent.name == "Access Log") {
            MenuName = langResource.AccessLogMenuLabel;
        }
        else if (grandParent.name == "Test Geomerty Data") {
            MenuName = langResource.TestGeomertyDataMenuLabel;
        }
        else if (grandParent.name == "Global Param") {
            MenuName = langResource.GlobalParamMenuLabel;
        }
        else if (grandParent.name == "Testing") {
            MenuName = langResource.TestingMenuLabel;
        }
        else if (grandParent.name == "Access Rights") {
            MenuName = langResource.AccessRightsMenuLabel;
        }
        else if (grandParent.name == "Report") {
            MenuName = langResource.ReportMenuLabel;
        }
        else if (grandParent.name == "Administration") {
            MenuName = langResource.AdministrationMenuLabel;
        }
        else if (grandParent.name == "Access") {
            MenuName = langResource.AccessMenuLabel;
        }
        else if (grandParent.name == "Matrix") {
            MenuName = langResource.MatrixMenuLabel;
        }
        else if (grandParent.name == "Menu") {
            MenuName = langResource.MenuLabel;
        }
        else if (grandParent.name == "System Icon") {
            MenuName = langResource.SystemIconMenuLabel;
        }
        else if (grandParent.name == "All System Icon") {
            MenuName = langResource.AllSystemIconMenuLabel;
        }
        else if (grandParent.name == "Zone Management") {
            MenuName = langResource.ZoneManagementMenuLabel;
        }
        else if (grandParent.name == "Company") {
            MenuName = langResource.CompanyMenuLabel;
        }
        else if (grandParent.name == "Projects") {
            MenuName = langResource.ProjectsMenuLabel;
        }
        else if (grandParent.name == "Locality") {
            MenuName = langResource.LocalityMenuLabel;
        }
        else if (grandParent.name == "Country") {
            MenuName = langResource.CountryMenuLabel;
        }
        else if (grandParent.name == "MCA/VCA") {
            MenuName = langResource.MCAVCAMenuLabel;
        }
        else if (grandParent.name == "Task") {
            MenuName = langResource.TaskMenuLabel;
        }
        else if (grandParent.name == "Table Code Configuration") {
            MenuName = langResource.TableCodeConfigurationMenuLabel;
        }
        else if (grandParent.name == "Code Configuration") {
            MenuName = langResource.CodeConfigurationMenuLabel;
        }
        else if (grandParent.name == "General Information") {
            MenuName = langResource.GeneralInformationMenuLabel;
        }
        else if (grandParent.name == "Project Template") {
            MenuName = langResource.ProjectTemplateMenuLabel;
        }
        else {
            MenuName = grandParent.name;
        }

        var IDText = "li" + removeSpecialCharacters(grandParent.name) + "menu";
        // Build grandparent menu item

        let grandParentHtml = `
                    <li class="nav-item">
                        <a href="${grandParent.url ? '/' + grandParent.url : '#'}" class="nav-link" id="${IDText}">
                            <i class="nav-icon ${grandParent.icon}"></i>
                            <p>                           
                                ${MenuName}
                                ${grandParent.subMenu.length ? '<i class="fas fa-angle-right right"></i>' : ''}
                            </p>
                        </a>`;


        // Check for submenus
        if (grandParent.subMenu.length > 0) {
            let parentHtml = '<ul class="nav nav-treeview" style="display: none;">';

            grandParent.subMenu.forEach(parent => {
                //For Sub Menu 
                var SubMenuName = "";
                if (parent.name == "Dashboard") {
                    SubMenuName = langResource.DashboardMenuLabel;
                }
                else if (parent.name == "User") {
                    SubMenuName = langResource.UserMenuLabel;
                }
                else if (parent.name == "Poster") {
                    SubMenuName = langResource.PosterMenuLabel;
                }
                else if (parent.name == "Carousel") {
                    SubMenuName = langResource.CarouselMenuLabel;
                }
                else if (parent.name == "Device") {
                    SubMenuName = langResource.DeviceMenuLabel;
                }
                else if (parent.name == "LookUpType") {
                    SubMenuName = langResource.LookUpTypeMenuLabel;
                }
                else if (parent.name == "LookUpValue") {
                    SubMenuName = langResource.LookUpValueMenuLabel;
                }
                else if (parent.name == "Log") {
                    SubMenuName = langResource.LogMenuLabel;
                }
                else if (parent.name == "Room") {
                    SubMenuName = langResource.RoomMenuLabel;
                }
                else if (parent.name == "Resource") {
                    SubMenuName = langResource.ResourceMenuLabel;
                }
                else if (parent.name == "Check In/Out") {
                    SubMenuName = langResource.CheckInOutMenuLabel;
                }
                else if (parent.name == "Access Log") {
                    SubMenuName = langResource.AccessLogMenuLabel;
                }
                else if (parent.name == "Test Geomerty Data") {
                    SubMenuName = langResource.TestGeomertyDataMenuLabel;
                }
                else if (parent.name == "Global Param") {
                    SubMenuName = langResource.GlobalParamMenuLabel;
                }
                else if (parent.name == "Testing") {
                    SubMenuName = langResource.TestingMenuLabel;
                }
                else if (parent.name == "Access Rights") {
                    SubMenuName = langResource.AccessRightsMenuLabel;
                }
                else if (parent.name == "Report") {
                    SubMenuName = langResource.ReportMenuLabel;
                }
                else if (parent.name == "Administration") {
                    SubMenuName = langResource.AdministrationMenuLabel;
                }
                else if (parent.name == "Access") {
                    SubMenuName = langResource.AccessMenuLabel;
                }
                else if (parent.name == "Matrix") {
                    SubMenuName = langResource.MatrixMenuLabel;
                }
                else if (parent.name == "Menu") {
                    SubMenuName = langResource.MenuLabel;
                }
                else if (parent.name == "System Icon") {
                    SubMenuName = langResource.SystemIconMenuLabel;
                }
                else if (parent.name == "All System Icon") {
                    SubMenuName = langResource.AllSystemIconMenuLabel;
                }
                else if (parent.name == "Zone Management") {
                    SubMenuName = langResource.ZoneManagementMenuLabel;
                }
                else if (parent.name == "Company") {
                    SubMenuName = langResource.CompanyMenuLabel;
                }
                else if (parent.name == "Locality") {
                    SubMenuName = langResource.LocalityMenuLabel;
                }
                else if (parent.name == "Country") {
                    SubMenuName = langResource.CountryMenuLabel;
                }
                else if (parent.name == "MCA/VCA") {
                    SubMenuName = langResource.MCAVCAMenuLabel;
                }
                else if (parent.name == "General Information") {
                    SubMenuName = langResource.GeneralInformationMenuLabel;
                }
                else if (parent.name == "Project Template") {
                    SubMenuName = langResource.ProjectTemplateMenuLabel;
                }
                else {
                    SubMenuName = parent.name;
                }

                var IDTextSub = "li" + removeSpecialCharacters(parent.name) + "menu";

                parentHtml += `
                            <li class="nav-item ${parent.child.length ? 'childSubMenu' : ''}" >
                                <a href="${parent.url ? '/' + parent.url : '#'}" class="nav-link" id="${IDTextSub}">
                                    <i class="${parent.icon || 'far fa-circle nav-icon'}"></i>
                                    <p>
                                ${SubMenuName}
                                ${parent.child.length ? '<i class="fas fa-angle-right right" style="right: 2.4rem;"></i>' : ''}
                            </p>
                                </a>`;

                // Check for child menus
                if (parent.child.length > 0) {
                    parentHtml += '<ul class="nav nav-treeview" style="display: none;">';

                    parent.child.forEach(child => {

                        //For Child Sub Menu
                        var ChildSubMenuName = "";
                        if (child.name == "Dashboard") {
                            ChildSubMenuName = langResource.DashboardMenuLabel;
                        }
                        else if (child.name == "User") {
                            ChildSubMenuName = langResource.UserMenuLabel;
                        }
                        else if (child.name == "Poster") {
                            ChildSubMenuName = langResource.PosterMenuLabel;
                        }
                        else if (child.name == "Carousel") {
                            ChildSubMenuName = langResource.CarouselMenuLabel;
                        }
                        else if (child.name == "Device") {
                            ChildSubMenuName = langResource.DeviceMenuLabel;
                        }
                        else if (child.name == "LookUpType") {
                            ChildSubMenuName = langResource.LookUpTypeMenuLabel;
                        }
                        else if (child.name == "LookUpValue") {
                            ChildSubMenuName = langResource.LookUpValueMenuLabel;
                        }
                        else if (child.name == "Log") {
                            ChildSubMenuName = langResource.LogMenuLabel;
                        }
                        else if (child.name == "Room") {
                            ChildSubMenuName = langResource.RoomMenuLabel;
                        }
                        else if (child.name == "Resource") {
                            ChildSubMenuName = langResource.ResourceMenuLabel;
                        }
                        else if (child.name == "Check In/Out") {
                            ChildSubMenuName = langResource.CheckInOutMenuLabel;
                        }
                        else if (child.name == "Access Log") {
                            ChildSubMenuName = langResource.AccessLogMenuLabel;
                        }
                        else if (child.name == "Test Geomerty Data") {
                            ChildSubMenuName = langResource.TestGeomertyDataMenuLabel;
                        }
                        else if (child.name == "Global Param") {
                            ChildSubMenuName = langResource.GlobalParamMenuLabel;
                        }
                        else if (child.name == "Testing") {
                            ChildSubMenuName = langResource.TestingMenuLabel;
                        }
                        else if (child.name == "Access Rights") {
                            ChildSubMenuName = langResource.AccessRightsMenuLabel;
                        }
                        else if (child.name == "Report") {
                            ChildSubMenuName = langResource.ReportMenuLabel;
                        }
                        else if (child.name == "Administration") {
                            ChildSubMenuName = langResource.AdministrationMenuLabel;
                        }
                        else if (child.name == "Access") {
                            ChildSubMenuName = langResource.AccessMenuLabel;
                        }
                        else if (child.name == "Matrix") {
                            ChildSubMenuName = langResource.MatrixMenuLabel;
                        }
                        else if (child.name == "Menu") {
                            ChildSubMenuName = langResource.MenuLabel;
                        }
                        else if (child.name == "System Icon") {
                            ChildSubMenuName = langResource.SystemIconMenuLabel;
                        }
                        else if (child.name == "All System Icon") {
                            ChildSubMenuName = langResource.AllSystemIconMenuLabel;
                        }
                        else if (child.name == "Zone Management") {
                            ChildSubMenuName = langResource.ZoneManagementMenuLabel;
                        }
                        else if (child.name == "Company") {
                            ChildSubMenuName = langResource.CompanyMenuLabel;
                        }
                        else if (child.name == "Locality") {
                            ChildSubMenuName = langResource.LocalityMenuLabel;
                        }
                        else if (child.name == "Country") {
                            ChildSubMenuName = langResource.CountryMenuLabel;
                        }
                        else if (child.name == "MCA/VCA") {
                            ChildSubMenuName = langResource.MCAVCAMenuLabel;
                        }
                        else if (child.name == "General Information") {
                            ChildSubMenuName = langResource.GeneralInformationMenuLabel;
                        }
                        else if (child.name == "Project Template") {
                            ChildSubMenuName = langResource.ProjectTemplateMenuLabel;
                        }
                        else {
                            ChildSubMenuName = child.name;
                        }

                        var IDTextSubMenu = "li" + removeSpecialCharacters(child.name) + "menu";

                        parentHtml += `
                                    <li class="nav-item">
                                        <a href="${child.url ? '/' + child.url : '#'}" class="nav-link" id="${IDTextSubMenu}">
                                            <i class="${child.icon || 'far fa-dot-circle nav-icon'}"></i>
                                            <p>${ChildSubMenuName}</p>
                                        </a>
                                    </li>`;
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

function removeSpecialCharacters(str) {
    return str.replace(/[^a-zA-Z0-9]/g, ''); // Removes anything that's not a letter or number
}

function initMenuToggle() {
    $(".nav-item > a").click(function (e) {
        const submenu = $(this).next(".nav-treeview");

        //$(this).closest(".nav-link").addClass("active");
        if (submenu.length) {
            e.preventDefault();
            submenu.slideToggle();
            $(this).find(".right").toggleClass("fa-angle-right fa-angle-down");
        }
    });

    $(".nav-link").click(function () {
        // Remove active and menu-open classes from all items
        $(".nav-link").removeClass("active");
        $(".nav-item").removeClass("menu-open");

        // Add active class to the clicked item
        $(this).addClass("active");

        // If the current item has a submenu, open it
        const submenu = $(this).next(".nav-treeview");

        if (submenu.length) {

            $(this).parent().addClass("menu-open");
            submenu.slideDown();
        }

    });

    initChildMenuToggle();

}

function initChildMenuToggle() {
    $(".nav-item > a").click(function (e) {
        const child = $(this).next(".nav-treeview");
        if (child.length) {
            e.preventDefault();
            child.slideToggle();
            $(this).find(".right").toggleClass("fa-angle-right fa-angle-down");
        }
    });

}


