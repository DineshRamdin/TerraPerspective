

function initializeDataGrid(tableID, onclickPartial, url, columnList, showDelete, showAdd, showEdit, showResetUserPassword = false, showPreview = false, onclickPreview = null,
    showExcel = true, onclickDelete = null, langResource = null) {

    var dataToShow = null; //init data variable
    var access = null; // init access variable
    var addBtn = '';

    var UserNamelocal = localStorage.getItem('UserName');
    var GlobalParamValuelocal = localStorage.getItem('GlobalParamValue');

    $.ajax({
        url: url,
        type: "POST",
        // data: dataObject,  // Send data if needed
        beforeSend: function () {
            if ($.fn.dataTable.isDataTable('#' + tableID)) {
                $('#' + tableID).DataTable().clear().destroy();
            }
        },
        success: function (response) {

            if (response.data) {

                if (response.data != undefined || response.data != null || response.data != "") {
                    access = response.data
                }
                dataToShow = response.data;

                if ($('#' + tableID).length > 0) { //check if table exists

                    $('#' + tableID).DataTable({ //init table                       
                        /* dom: '<"top"Bf>rt<"bottom"ip>', */
                        dom: '<"row"<"col-md-10"B><"col-md-2"f>>rtipl',
                        searching: true, //show search
                        paging: true, //no paging
                        processing: true, //processing
                        scrollY: 'auto', //fixed height
                        scrollCollapse: true,//scroll
                        order: [[0, 'desc']], //order
                        autoWidth: false,
                        lengthChange: false,
                        language: { // search anf info bars
                            search: '',
                            emptyTable: langResource.NodataavailableLabel,  //'No data available',
                            searchPlaceholder: langResource.SearchLabel, //'Search',
                            info: langResource.ShowingLabel + " _TOTAL_ " + langResource.EntriesLabel, //"Showing _TOTAL_ entries",
                            infoEmpty: langResource.ShowingLabel + " 0 " + langResource.EntriesLabel,//"Showing 0 entries",
                            infoFiltered: "(" + langResource.FilteredfromLabel + " _MAX_ " + langResource.EntriesLabel + ")",  //"(filtered from _MAX_ entries)",
                            //previous: langResource.PreviousLabel,  // Set the "Previous" button text
                            //next: langResource.NextLabel,
                        },
                        data: dataToShow, // data to display
                        columnDefs: [ // set action column
                            {
                                //set column order for action
                                targets: columnList.length,
                                //remove sorting on action
                                orderable: false,
                                //set width of action column
                                width: "10%",
                                render: function (data, type, full, meta) {

                                    //view button
                                    var view = '<a href="#" onclick="' + onclickPartial + '(\'' + full.id + '\')" title="' + langResource.ViewLabel + '"><i class="fa fa-eye text-secondary ms-1"></i></a> |';
                                    //edit button
                                    var edit = '<a href="#" onclick="' + onclickPartial + '(\'' + full.id + '\')" title="' + langResource.EditLabel + '"><i class="fa fa-edit text-secondary ms-1"></i></a> ';
                                    //delete button
                                    /*var del = ' | <a href="#" onclick="delete(\'' + full.id + '\')" title="Delete"><i class="fa fa-delete text-secondary "></i></a>';*/
                                    var del = '| <a href="#" onclick="' + onclickDelete + '(\'' + full.id + '\')" title="' + langResource.DeleteLabel + '"><i class="fa fa-trash text-secondary ms-1"></i></a> ';
                                    //reset user 
                                    var reset = '| <a href="#" onclick="ResetUserPassword(\'' + full.id + '\')"  title="' + langResource.ResetPasswordLabel + '"><i class="fas fa-undo text-secondary ms-1"></i></a> ';
                                    //list of buttons

                                    var preview = '| <a href="#" onclick="' + onclickPreview + '(\'' + full.id + '\')" title="' + langResource.PreviewLabel + '"><i class="fa fa-eye text-secondary ms-1"></i></a> ';

                                    var btn = '';

                                    if (showEdit == true) {
                                        btn += edit
                                    }

                                    if (showDelete == true) {
                                        btn += del
                                    }
                                    if (showResetUserPassword == true) {
                                        btn += reset
                                    }
                                    if (showPreview == true) {
                                        if (tableID == "ReportsTable") {
                                            // if (full.isImage == true) {
                                            if (full.viewName !== "") {
                                                btn += '| <a href="#" onclick="' + onclickPreview + '(\'' + full.viewName + '\')" title="' + langResource.PreviewLabel + '"><i class="fa fa-eye text-secondary ms-1"></i></a> ';//preview
                                            }
                                            //else if (=="PosterTable")
                                        }
                                        else if (tableID == "PosterTable") {
                                            if (full.isImage == true) {
                                                btn += preview
                                            }
                                            //else if (=="PosterTable")
                                        }
                                        else if (tableID == "UserTable") {
                                            if (full.isImage == true) {
                                                btn += preview
                                            }
                                            //else if (=="PosterTable")
                                        }
                                        else if (tableID == "CompanyTable") {
                                            if (full.companyIconFlag == true) {
                                                btn += preview
                                            }
                                            //else if (=="PosterTable")
                                        }
                                        else if (tableID == "ZoneManagementTable") {
                                            if (full.folder != null && full.folder != "") {
                                                btn += '| <a href="#" onclick="' + onclickPreview + '(\'' + full.folder + '\')" title="' + langResource.PreviewLabel + '"><i class="fa fa-folder-open text-secondary ms-1"></i></a> ';//preview
                                            }
                                            //else if (=="PosterTable")
                                        }
                                        else {
                                            btn += preview
                                        }
                                    }

                                    data = '<div class="text-nowrap">' + btn + '</div>';
                                    return data;
                                }
                            }
                        ],
                        columns: columnList, //column list
                        buttons: [
                            {
                                text: '<i class="fa fa-plus me-2"></i>  &nbsp;' + langResource.AddNewLabel, // Add New',
                                className: 'btn btn-sm btn-primary custom-class addbtn',
                                action: function (e, dt, node, config) {
                                    // Replace this with your custom action
                                    // You can dynamically invoke a function if onclickPartial is a string
                                    window[onclickPartial]();  // 

                                }
                            },
                            //Excel Export Button
                            {
                                extend: 'excel',
                                text: '<i class="fa fa-file-excel me-2"></i>' + langResource.ExportExcelLabel,//  Export Excel',
                                className: 'btn-sm btn-c-secondary d-none excelbtn',
                                exportOptions: {
                                    columns: ':visible:not(.sorting_disabled)'

                                },
                                customize: function (xlsx) {

                                    if (GlobalParamValuelocal.toLowerCase() == "yes") {
                                        var sheet = xlsx.xl.worksheets['sheet1.xml'];
                                        //var currentDate = new Date().toLocaleDateString();

                                        var date = new Date();
                                        // Define the options for the date format
                                        var options = {
                                            day: '2-digit',
                                            month: 'short',
                                            year: 'numeric'
                                        };

                                        // Format the date as '03/Dec/2024'
                                        var formattedDate = date.toLocaleDateString('en-GB', options);

                                        // Replace the default date format with slashes between parts
                                        var currentDate = formattedDate.replace(/ /g, '/');

                                        // Define the options for the time format (12-hour format with AM/PM)
                                        var timeOptions = {
                                            hour: '2-digit',
                                            minute: '2-digit',
                                            hour12: true
                                        };

                                        // Format the time as '02:20 PM'
                                        var formattedTime = date.toLocaleTimeString('en-GB', timeOptions);

                                        // Combine the date and time
                                        currentDate = currentDate + ' ' + formattedTime;

                                        var username = UserNamelocal;

                                        // Find the last row of the sheet
                                        var rowCount = $(sheet).find('sheetData row').length;

                                        // Add 4 to the rowCount to place the footer 4 rows after the last data row
                                        var footerRowPosition = rowCount + 4; // The footer row is placed 4 rows after the last row of data

                                        // Create the footer row with valid XML structure
                                        //var footerRow = `
                                        //<row r="${footerRowPosition}">
                                        //    <c t="s" r="A${footerRowPosition}"><v>UserName : ${username}</v></c>
                                        //    <c t="s" r="B${footerRowPosition}"><v>Date : ${currentDate}</v></c>
                                        //</row>`;

                                        var footerRow = `
                                        <row r="${footerRowPosition}">
                                            <c t="s" r="A${footerRowPosition}"><v>${langResource.UserNameLabel} : ${username}</v></c>
                                            <c t="s" r="B${footerRowPosition}"><v>${langResource.DateLabel} : ${currentDate}</v></c>
                                        </row>`;

                                        // Append the footer row to the sheet data, ensuring proper structure
                                        $(sheet).find('sheetData').append(footerRow);
                                    }
                                }
                            },
                            //PDF Export Button
                            {
                                extend: 'pdf',
                                text: '<i class="fa fa-file-pdf me-2"></i>' + langResource.ExportPDFLabel, //  Export PDF',
                                className: 'btn-sm btn-c-secondary d-none printbtn',
                                exportOptions: {
                                    columns: ':visible:not(.sorting_disabled)'

                                },
                                customize: function (doc) {
                                    if (GlobalParamValuelocal.toLowerCase() == "yes") {
                                        /* var currentDate = new Date().toLocaleDateString();*/
                                        var date = new Date();

                                        // Define the options for the date format
                                        var options = {
                                            day: '2-digit',
                                            month: 'short',
                                            year: 'numeric'
                                        };

                                        // Format the date as '03/Dec/2024'
                                        var formattedDate = date.toLocaleDateString('en-GB', options);

                                        // Replace the default date format with slashes between parts
                                        var currentDate = formattedDate.replace(/ /g, '/');


                                        // Define the options for the time format (12-hour format with AM/PM)
                                        var timeOptions = {
                                            hour: '2-digit',
                                            minute: '2-digit',
                                            hour12: true
                                        };

                                        // Format the time as '02:20 PM'
                                        var formattedTime = date.toLocaleTimeString('en-GB', timeOptions);

                                        // Combine the date and time
                                        currentDate = currentDate + ' ' + formattedTime;


                                        var username = UserNamelocal; // Replace with dynamic username

                                        // Define the styles for header and footer if not already defined
                                        doc.styles = doc.styles || {};  // Ensure styles object exists
                                        doc.styles.header = doc.styles.header || {}; // Ensure header style exists
                                        doc.styles.footer = doc.styles.footer || {}; // Ensure footer style exists

                                        // Adjust the font size for header and footer styles
                                        doc.styles.header.fontSize = 12;
                                        doc.styles.footer.fontSize = 10;


                                        // Add custom footer content with username and date after all data
                                        doc.content.push({  // Push to the end of the content array
                                            text: `${langResource.UserNameLabel} : ${username} | ${langResource.DateLabel} : ${currentDate}`,
                                            style: 'footer',  // Use the defined footer style
                                            alignment: 'left',
                                            margin: [0, 30, 10, 0] // Add some space before the footer text
                                        });

                                        // Optionally add the content to the footer (if needed)
                                        doc.footer = function (currentPage, pageCount) {
                                            return {
                                                columns: [
                                                    {
                                                        //text: `Page ${currentPage} of ${pageCount}`,
                                                        text: `${langResource.PageLabel} ${currentPage} ${langResource.OfLabel} ${pageCount}`,
                                                        alignment: 'center'
                                                    },
                                                    {
                                                        //text: `Generated on ${currentDate}`,
                                                        text: `${langResource.GeneratedonLabel} ${currentDate}`,
                                                        alignment: 'right'
                                                    }
                                                ],
                                                margin: [20, 0]
                                            };
                                        };

                                        // Optionally, adjust document settings like font size for the entire document
                                        doc.defaultStyle.fontSize = 10;
                                    }
                                }
                            },
                            //CSV Export Button
                            {
                                extend: 'csv',
                                text: '<i class="fa fa-file-excel me-2"></i>' + langResource.ExportCSVLabel, //  Export CSV',
                                className: 'btn-sm btn-c-secondary d-none printbtn',
                                exportOptions: {
                                    columns: ':visible:not(.sorting_disabled)'

                                },
                                customize: function (csv) {
                                    if (GlobalParamValuelocal.toLowerCase() == "yes") {
                                        // Current date and username
                                        var date = new Date();

                                        // Define the options for the date format
                                        var options = {
                                            day: '2-digit',
                                            month: 'short',
                                            year: 'numeric'
                                        };

                                        // Format the date as '03/Dec/2024'
                                        var formattedDate = date.toLocaleDateString('en-GB', options);

                                        // Replace the default date format with slashes between parts
                                        var currentDate = formattedDate.replace(/ /g, '/');

                                        // Define the options for the time format (12-hour format with AM/PM)
                                        var timeOptions = {
                                            hour: '2-digit',
                                            minute: '2-digit',
                                            hour12: true
                                        };

                                        // Format the time as '02:20 PM'
                                        var formattedTime = date.toLocaleTimeString('en-GB', timeOptions);

                                        // Combine the date and time
                                        currentDate = currentDate + ' ' + formattedTime;

                                        var username = UserNamelocal; // Replace with dynamic username from your system

                                        // Split the CSV content into rows
                                        var rows = csv.split('\n');

                                        // If rows are empty, just add at the end
                                        rows.push(`,`);
                                        rows.push(`, `);
                                        rows.push(`, `);
                                        //rows.push(`UserName : ${username}, Date : ${currentDate}`);
                                        rows.push(`${langResource.UserNameLabel} : ${username}, ${langResource.DateLabel} : ${currentDate}`);

                                        // Join the rows back into a single CSV string and return
                                        return rows.join('\n');
                                    }
                                    return csv; // Return the original CSV if the condition isn't met
                                }
                            },

                            //'<button class="btn btn-block btn-primary mb-3" onclick="' + onclickPartial + '()"><i class="fa fa-plus me-2"></i>Add New</button>',

                        ],
                        createdRow: function (row, data, dataIndex) {
                            $(row).attr('data-id', data.id);
                        },
                        initComplete: function () {

                            var clsDropdownmenu = "dropdown-menu-export";
                            if (tableID === 'AccessLogTable') {
                                clsDropdownmenu = "dropdown-menu-export-accesslog";
                            }
                            else if (tableID === 'GeomertyDataTable') {
                                clsDropdownmenu = "dropdown-menu-export-accesslog";
                            }

                            // Create custom dropdown HTML with icons and a non-blank "Export" option
                            var dropdownHtml = `
                                    <div class="custom-dropdown">
                                        <button id="dropdownButton" class="form-control btn-sm btn-info btn btn-secondary dropdown-toggle clsDownload" style="width: auto;">
                                            ${langResource.ExportLabel}
                                        </button>
                                        <div id="dropdownMenu" class="${clsDropdownmenu}" style="display: none;">
                                            <div class="dropdown-item" data-value="1"><i class="fa fa-file-pdf me-2"></i> ${langResource.PDFLabel}</div>
                                            <div class="dropdown-item" data-value="2"><i class="fa fa-file-excel me-2"></i> ${langResource.ExcelLabel}</div>
                                            <div class="dropdown-item" data-value="3"><i class="fa fa-file-csv me-2"></i> ${langResource.CSVLabel}</div>
                                        </div>
                                    </div>
                                `;

                            // Append the custom dropdown to the filter area
                            $('div.dataTables_filter').append(dropdownHtml);

                            if (tableID === 'ZoneManagementTable') {
                                var ShowallZonebtn = `<button type="button" class="btn btn-success mb-3 mr-3 clsShowAllZone" onclick="ShowAllZonebtnClick();">Show All Zone</button>`;
                                $('div.dataTables_filter').append(ShowallZonebtn);
                            }
                            // Toggle the dropdown when the button is clicked
                            $('#dropdownButton').on('click', function (e) {
                                e.stopPropagation(); // Prevents event bubbling
                                $('#dropdownMenu').toggle();
                            });

                            // Handle item selection when a dropdown item is clicked
                            $('#dropdownMenu .dropdown-item').on('click', function () {
                                var value = $(this).data('value');
                                var table = $('#' + tableID).DataTable();

                                if (value == '1') { // PDF Export
                                    $('.buttons-pdf').click();
                                } else if (value == '2') { // Excel Export
                                    $('.buttons-excel').click();
                                } else if (value == '3') { // CSV Export
                                    $('.buttons-csv').click();
                                }

                                // Hide the dropdown after selection
                                $('#dropdownMenu').hide();
                            });

                            // Hover effect for dropdown items
                            $('#dropdownMenu .dropdown-item').hover(
                                function () {
                                    $(this).css("background-color", "#f0f0f0");  // Hover color
                                },
                                function () {
                                    $(this).css("background-color", "");  // Reset background color
                                }
                            );

                            $('div.dataTables_filter').css({
                                'gap': '10px' // Adds space between elements (dropdown and buttons)
                            });


                            // Optionally add some margin to the 'Add New' button to provide space between the dropdown and the button
                            $('.addbtn').css('margin-left', '10px'); // Adds margin between 'Add New' button and the dropdown
                            // $('.excelbtn').css('margin-left', '10px'); // Adds margin between 'Add New' button and the dropdown

                        },
                        drawCallback: function () {
                            $('#' + tableID + '_previous a').html(langResource.PreviousLabel);
                            $('#' + tableID + '_next a').html(langResource.NextLabel);
                        },


                    })//.buttons().container().appendTo('#example1_wrapper .col-md-6:eq(0)');


                    $('div.dt-buttons').prepend($('.clsDownload'));
                    $('div.dt-buttons').prepend($('.clsShowAllZone'));

                    $('div.dt-buttons').css('float', 'inline-end');
                    $('.addbtn').css('border-radius', '.25rem');
                    $('.clsDownload').css('border-radius', '.25rem');
                    $('.clsShowAllZone').css('border-radius', '.25rem');

                    $('.custom-class').removeClass('btn-secondary');
                    $('.dt-scroll-headInner table thead').addClass('custom-table');
                    $('.dataTables_filter').css('float', 'inline-start');
                    $('.dataTables_paginate').css('margin-top', '-30px');

                    $('.dataTables_filter label input').css('height', '32px');

                    if (response.data == "") {
                        $('#' + tableID + '_previous').css('display', 'none');
                        $('#' + tableID + '_next').css('display', 'none');
                        $('.dataTables_paginate').css('margin-top', '0px');
                    }

                    //$('#' + tableID + '_previous a').html(langResource.PreviousLabel);
                    // $('#' + tableID + '_next a').html(langResource.NextLabel);


                    if (showAdd) {
                        $('.addbtn').removeClass('d-none');
                    }
                    else {
                        $('.addbtn').addClass('d-none');
                    }

                    if (showPreview) {
                        $('#savebtn').addClass('d-none');
                    } else {
                        $('#savebtn').removeClass('d-none');
                    }

                    if (showExcel) {
                        /*$('.excelbtn').removeClass('d-none');*/
                        $('.clsDownload').removeClass('d-none');

                    }
                    else {
                        /*$('.excelbtn').addClass('d-none');*/
                        $('.clsDownload').addClass('d-none');
                    }
                }
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

function initializeDataGridNested(tableID, columnList, childColumns, dataToShow = null, search = false, exportExcel = false, langResource = null) {
    var exportButtons = [];
    if (exportExcel) {
        exportButtons.push(this.exportExcelBtn)
    }

    if ($.fn.dataTable.isDataTable('#' + tableID)) {
        $('#' + tableID).DataTable().clear().destroy();
    }
    if ($('#' + tableID).length > 0) { //check if table exists
        var table = $('#' + tableID).DataTable({ //init table
            dom: '<"row"<"col-md-10"B><"col-md-2"f>>rtipl', //button Strip
            searching: search, //show search
            paging: false, //no paging
            processing: true, //processing
            scrollY: 'auto', //fixed height
            scrollCollapse: true,//scroll
            language: { // search anf info bars
                search: '',
                emptyTable: langResource.NodataavailableLabel,  //'No data available',
                searchPlaceholder: langResource.SearchLabel, //'Search',
                info: langResource.ShowingLabel + " _TOTAL_ " + langResource.EntriesLabel, //"Showing _TOTAL_ entries",
                infoEmpty: langResource.ShowingLabel + " 0 " + langResource.EntriesLabel,//"Showing 0 entries",
                infoFiltered: "(" + langResource.FilteredfromLabel + " _MAX_ " + langResource.EntriesLabel + ")",  //"(filtered from _MAX_ entries)",
            },

            //language: { // search anf info bars
            //    search: '',
            //    emptyTable: 'No data available',
            //    searchPlaceholder: 'Search',
            //    info: "Showing _TOTAL_ entries",
            //    infoEmpty: "Showing 0 entries",
            //    infoFiltered: "(filtered from _MAX_ entries)",
            //},
            //scrollX: true,
            data: dataToShow, // data to display
            columns: columnList, //column list
            buttons: exportButtons,
            drawCallback: function () {
                // BUTTON EVENT ON THE DATA TABLE
            }
        });

        $('#' + tableID + ' tbody').on('click', 'td.dt-control', function () {
            var tr = $(this).closest('tr');
            var row = table.row(tr);
            if (row.child.isShown()) {
                // This row is already open - close it
                row.child.hide();
                tr.removeClass('shown');
            }
            else {
                // Open this row
                row.child(format(row.data(), row.index())).show();
                addDataToChild(row.data(), row.index(), childColumns);
                tr.addClass('shown');
            }

        });


        $('div.dt-buttons').css('float', 'inline-end');
        $('.custom-class').removeClass('btn-secondary');
        $('.dt-scroll-headInner table thead').addClass('custom-table');
        $('.dataTables_filter').css('float', 'inline-start');
        $('.dataTables_paginate').css('margin-top', '-30px');
        $('.dataTables_filter label input').css('height', '32px');
    }
}

function format(d, rowIndex) {
    // `d` is the original data object for the row
    return '<table border="0" id="child_' + rowIndex + '"></table>';

}

function addDataToChild(d, rowIndex, childColumns) {

    $('#child_' + rowIndex + '').DataTable({
        "data": d.accessRightDetailDTO,
        "bLengthChange": false,
        "bInfo": false,
        "bFilter": false,
        "bPaginate": false,
        "columns": childColumns
    });
}

function initializeDataGridForViews(tableID, showExcel = true, langResource = null) {

    var UserNamelocal = localStorage.getItem('UserName');
    var GlobalParamValuelocal = localStorage.getItem('GlobalParamValue');

    if ($('#' + tableID).length > 0) { //check if table exists

        var dataRowCount = $('#' + tableID + ' tbody tr').length;

        var errorMessageColumnIndex = -1;
        // if (tableID == "FileViewTable") {
        $('#' + tableID + ' th').each(function (index) {
            var headerText = $(this).text().trim();
            if (headerText === 'ErrorMessage') {
                errorMessageColumnIndex = 0;
            }
        });
        // }


        $('#' + tableID).DataTable({ //init table                       
            /* dom: '<"top"Bf>rt<"bottom"ip>', */
            dom: '<"row"<"col-md-10"B><"col-md-2"f>>rtipl',
            searching: true, //show search
            paging: true, //no paging
            processing: true, //processing
            scrollY: 'auto', //fixed height
            scrollCollapse: true,//scroll
            order: [[0, 'desc']], //order
            autoWidth: false,
            lengthChange: false,
            language: { // search anf info bars
                search: '',
                emptyTable: langResource.NodataavailableLabel,  //'No data available',
                searchPlaceholder: langResource.SearchLabel, //'Search',
                info: langResource.ShowingLabel + " _TOTAL_ " + langResource.EntriesLabel, //"Showing _TOTAL_ entries",
                infoEmpty: langResource.ShowingLabel + " 0 " + langResource.EntriesLabel,//"Showing 0 entries",
                infoFiltered: "(" + langResource.FilteredfromLabel + " _MAX_ " + langResource.EntriesLabel + ")",  //"(filtered from _MAX_ entries)",
            },
            //language: { // search anf info bars
            //    search: '',
            //    emptyTable: 'No data available',
            //    searchPlaceholder: 'Search',
            //    info: "Showing _TOTAL_ entries",
            //    infoEmpty: "Showing 0 entries",
            //    infoFiltered: "(filtered from _MAX_ entries)",
            //},
            buttons: [
                //Excel Export Button
                {
                    extend: 'excel',
                    text: '<i class="fa fa-file-excel me-2"></i>' + langResource.ExportExcelLabel, //Export Excel',
                    className: 'btn-sm btn-c-secondary d-none excelbtn',
                    exportOptions: {
                        columns: ':visible:not(.sorting_disabled)'
                    },
                    customize: function (xlsx) {

                        if (GlobalParamValuelocal.toLowerCase() == "yes") {
                            var sheet = xlsx.xl.worksheets['sheet1.xml'];
                            //var currentDate = new Date().toLocaleDateString();
                            var date = new Date();

                            // Define the options for the date format
                            var options = {
                                day: '2-digit',
                                month: 'short',
                                year: 'numeric'
                            };

                            // Format the date as '03/Dec/2024'
                            var formattedDate = date.toLocaleDateString('en-GB', options);

                            // Replace the default date format with slashes between parts
                            var currentDate = formattedDate.replace(/ /g, '/');


                            // Define the options for the time format (12-hour format with AM/PM)
                            var timeOptions = {
                                hour: '2-digit',
                                minute: '2-digit',
                                hour12: true
                            };

                            // Format the time as '02:20 PM'
                            var formattedTime = date.toLocaleTimeString('en-GB', timeOptions);

                            // Combine the date and time
                            currentDate = currentDate + ' ' + formattedTime;

                            var username = UserNamelocal;

                            // Find the last row of the sheet
                            var rowCount = $(sheet).find('sheetData row').length;

                            // Add 4 to the rowCount to place the footer 4 rows after the last data row
                            var footerRowPosition = rowCount + 4; // The footer row is placed 4 rows after the last row of data

                            var footerRow = `
                                        <row r="${footerRowPosition}">
                                            <c t="s" r="A${footerRowPosition}"><v>${langResource.UserNameLabel} : ${username}</v></c>
                                            <c t="s" r="B${footerRowPosition}"><v>${langResource.DateLabel} : ${currentDate}</v></c>
                                        </row>`;
                            //// Create the footer row with valid XML structure
                            //var footerRow = `
                            //            <row r="${footerRowPosition}">
                            //                <c t="s" r="A${footerRowPosition}"><v>UserName : ${username}</v></c>
                            //                <c t="s" r="B${footerRowPosition}"><v>Date : ${currentDate}</v></c>
                            //            </row>`;

                            // Append the footer row to the sheet data, ensuring proper structure
                            $(sheet).find('sheetData').append(footerRow);
                        }
                    }
                },
                //PDF Export Button
                {
                    extend: 'pdf',
                    text: '<i class="fa fa-file-pdf me-2"></i>' + langResource.ExportPDFLabel,//  Export PDF',
                    className: 'btn-sm btn-c-secondary d-none printbtn',
                    exportOptions: {
                        columns: ':visible:not(.sorting_disabled)'
                    },
                    customize: function (doc) {
                        if (GlobalParamValuelocal.toLowerCase() == "yes") {
                            /* var currentDate = new Date().toLocaleDateString();*/
                            var date = new Date();

                            // Define the options for the date format
                            var options = {
                                day: '2-digit',
                                month: 'short',
                                year: 'numeric'
                            };

                            // Format the date as '03/Dec/2024'
                            var formattedDate = date.toLocaleDateString('en-GB', options);

                            // Replace the default date format with slashes between parts
                            var currentDate = formattedDate.replace(/ /g, '/');


                            // Define the options for the time format (12-hour format with AM/PM)
                            var timeOptions = {
                                hour: '2-digit',
                                minute: '2-digit',
                                hour12: true
                            };

                            // Format the time as '02:20 PM'
                            var formattedTime = date.toLocaleTimeString('en-GB', timeOptions);

                            // Combine the date and time
                            currentDate = currentDate + ' ' + formattedTime;

                            var username = UserNamelocal; // Replace with dynamic username

                            // Define the styles for header and footer if not already defined
                            doc.styles = doc.styles || {};  // Ensure styles object exists
                            doc.styles.header = doc.styles.header || {}; // Ensure header style exists
                            doc.styles.footer = doc.styles.footer || {}; // Ensure footer style exists

                            // Adjust the font size for header and footer styles
                            doc.styles.header.fontSize = 12;
                            doc.styles.footer.fontSize = 10;

                            // Add custom footer content with username and date after all data
                            doc.content.push({  // Push to the end of the content array
                                text: `${langResource.UserNameLabel} : ${username} | ${langResource.DateLabel} : ${currentDate}`,
                                style: 'footer',  // Use the defined footer style
                                alignment: 'left',
                                margin: [0, 30, 10, 0] // Add some space before the footer text
                            });

                            // Optionally add the content to the footer (if needed)
                            doc.footer = function (currentPage, pageCount) {
                                return {
                                    columns: [
                                        {
                                            //text: `Page ${currentPage} of ${pageCount}`,
                                            text: `${langResource.PageLabel} ${currentPage} ${langResource.OfLabel} ${pageCount}`,
                                            alignment: 'center'
                                        },
                                        {
                                            //text: `Generated on ${currentDate}`,
                                            text: `${langResource.GeneratedonLabel} ${currentDate}`,
                                            alignment: 'right'
                                        }
                                    ],
                                    margin: [20, 0]
                                };
                            };

                            // Optionally, adjust document settings like font size for the entire document
                            doc.defaultStyle.fontSize = 10;
                        }
                    }
                },
                //CSV Export Button
                {
                    extend: 'csv',
                    text: '<i class="fa fa-file-excel me-2"></i>' + langResource.ExportCSVLabel, // Export CSV',
                    className: 'btn-sm btn-c-secondary d-none csvbtn',
                    exportOptions: {
                        columns: ':visible:not(.sorting_disabled)'
                    },
                    customize: function (csv) {
                        if (GlobalParamValuelocal.toLowerCase() == "yes") {
                            // Current date and username
                            var date = new Date();

                            // Define the options for the date format
                            var options = {
                                day: '2-digit',
                                month: 'short',
                                year: 'numeric'
                            };

                            // Format the date as '03/Dec/2024'
                            var formattedDate = date.toLocaleDateString('en-GB', options);

                            // Replace the default date format with slashes between parts
                            var currentDate = formattedDate.replace(/ /g, '/');

                            // Define the options for the time format (12-hour format with AM/PM)
                            var timeOptions = {
                                hour: '2-digit',
                                minute: '2-digit',
                                hour12: true
                            };

                            // Format the time as '02:20 PM'
                            var formattedTime = date.toLocaleTimeString('en-GB', timeOptions);

                            // Combine the date and time
                            currentDate = currentDate + ' ' + formattedTime;

                            var username = UserNamelocal; // Replace with dynamic username from your system

                            // Split the CSV content into rows
                            var rows = csv.split('\n');

                            // If rows are empty, just add at the end
                            rows.push(`,`);
                            rows.push(`, `);
                            rows.push(`, `);
                            // rows.push(`UserName : ${username}, Date : ${currentDate}`);
                            rows.push(`${langResource.UserNameLabel} : ${username}, ${langResource.DateLabel} : ${currentDate}`);

                            // Join the rows back into a single CSV string and return
                            return rows.join('\n');
                        }
                        return csv; // Return the original CSV if the condition isn't met
                    }
                },
            ],
            createdRow: function (row, data, dataIndex) {
                $(row).attr('data-id', data.id);
            },
            initComplete: function () {

                // Create custom dropdown HTML with icons and a non-blank "Export" option
                var dropdownHtml = `
                                    <div class="custom-dropdown">
                                        <button id="dropdownButton" class="form-control btn-sm btn-info btn btn-secondary dropdown-toggle clsDownload" style="width: auto;">
                                            ${langResource.ExportLabel}
                                        </button>
                                        <div id="dropdownMenu" class="dropdown-menu-export-accesslog" style="display: none;">
                                            <div class="dropdown-item" data-value="1"><i class="fa fa-file-pdf me-2"></i> ${langResource.PDFLabel}</div>
                                            <div class="dropdown-item" data-value="2"><i class="fa fa-file-excel me-2"></i> ${langResource.ExcelLabel}</div>
                                            <div class="dropdown-item" data-value="3"><i class="fa fa-file-csv me-2"></i> ${langResource.CSVLabel}</div>
                                        </div>
                                    </div>
                                `;

                // Append the custom dropdown to the filter area
                $('div.dataTables_filter').append(dropdownHtml);

                // Toggle the dropdown when the button is clicked
                $('#dropdownButton').on('click', function (e) {
                    e.stopPropagation(); // Prevents event bubbling
                    $('#dropdownMenu').toggle();
                });

                // Handle item selection when a dropdown item is clicked
                $('#dropdownMenu .dropdown-item').on('click', function () {
                    var value = $(this).data('value');

                    if (value == '1') { // PDF Export
                        $('.buttons-pdf').click();
                    } else if (value == '2') { // Excel Export
                        $('.buttons-excel').click();
                    } else if (value == '3') { // CSV Export
                        $('.buttons-csv').click();
                    }

                    // Hide the dropdown after selection
                    $('#dropdownMenu').hide();
                });

                // Hover effect for dropdown items
                $('#dropdownMenu .dropdown-item').hover(
                    function () {
                        $(this).css("background-color", "#f0f0f0");  // Hover color
                    },
                    function () {
                        $(this).css("background-color", "");  // Reset background color
                    }
                );

                $('div.dataTables_filter').css({
                    'gap': '10px'
                });
            },
            drawCallback: function () {
                $('#' + tableID + '_previous a').html(langResource.PreviousLabel);
                $('#' + tableID + '_next a').html(langResource.NextLabel);
            },

        })

        $('div.dt-buttons').prepend($('.clsDownload'));
        $('div.dt-buttons').css('float', 'inline-end');
        $('.addbtn').css('border-radius', '.25rem');
        $('.custom-class').removeClass('btn-secondary');
        $('.dt-scroll-headInner table thead').addClass('custom-table');
        $('.dataTables_filter').css('float', 'inline-start');
        $('.dataTables_paginate').css('margin-top', '-30px');
        $('.dataTables_filter label input').css('height', '32px');

        if (showExcel) {
            $('.clsDownload').removeClass('d-none');
        }
        else {
            $('.clsDownload').addClass('d-none');
        }

        // $('#' + tableID + '_previous a').html(langResource.PreviousLabel);
        //$('#' + tableID + '_next a').html(langResource.NextLabel);

        if (errorMessageColumnIndex == 0) {
            $('.clsDownload').addClass('d-none');
            $('#' + tableID + '_previous').css('display', 'none');
            $('#' + tableID + '_next').css('display', 'none');
            $('.dataTables_paginate').css('display', 'none');
            $('.dataTables_filter').css('display', 'none');
            $('.dataTables_info').css('display', 'none');
        }

        if (dataRowCount === 0) {
            $('.clsDownload').addClass('d-none');
            $('#' + tableID + '_previous').css('display', 'none');
            $('#' + tableID + '_next').css('display', 'none');
            $('.dataTables_paginate').css('display', 'none');
            $('.dataTables_filter').css('display', 'none');
            $('.dataTables_info').css('display', 'none');
        }

    }


}
