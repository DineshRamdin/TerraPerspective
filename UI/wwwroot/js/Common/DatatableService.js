

function initializeDataGrid(tableID, onclickPartial, url, columnList, showDelete, showAdd, showEdit, showResetUserPassword = false, showPreview = false, onclickPreview = null,
    showExcel = false, onclickDelete = null) {

    var dataToShow = null; //init data variable
    var access = null; // init access variable
    var addBtn = '';

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
                            emptyTable: 'No data available',
                            searchPlaceholder: 'Search',
                            info: "Showing _TOTAL_ entries",
                            infoEmpty: "Showing 0 entries",
                            infoFiltered: "(filtered from _MAX_ entries)",
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
                                    var view = '<a href="#" onclick="' + onclickPartial + '(\'' + full.id + '\')" title="View"><i class="fa fa-eye text-secondary ms-1"></i></a> |';
                                    //edit button
                                    var edit = '<a href="#" onclick="' + onclickPartial + '(\'' + full.id + '\')" title="Edit"><i class="fa fa-edit text-secondary ms-1"></i></a> ';
                                    //delete button
                                    /*var del = ' | <a href="#" onclick="delete(\'' + full.id + '\')" title="Delete"><i class="fa fa-delete text-secondary "></i></a>';*/
                                    var del = '| <a href="#" onclick="' + onclickDelete + '(\'' + full.id + '\')" title="Delete"><i class="fa fa-trash text-secondary ms-1"></i></a> ';
                                    //reset user 
                                    var reset = '| <a href="#" onclick="ResetUserPassword(\'' + full.id + '\')"  title="Reset Password To Default"><i class="fas fa-undo text-secondary ms-1"></i></a> ';
                                    //list of buttons

                                    var preview = '| <a href="#" onclick="' + onclickPreview + '(\'' + full.id + '\')" title="Preview"><i class="fa fa-eye text-secondary ms-1"></i></a> ';

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
                                        btn += preview
                                    }

                                    data = '<div class="text-nowrap">' + btn + '</div>';
                                    return data;
                                }
                            }
                        ],
                        columns: columnList, //column list
                        buttons: [
                            {
                                text: '<i class="fa fa-plus me-2"></i> Add New',
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
                                text: '<i class="fa fa-file-excel me-2"></i>  Export Excel',
                                className: 'btn-sm btn-c-secondary d-none excelbtn',
                                exportOptions: {
                                    columns: ':visible:not(.sorting_disabled)'

                                }
                            },
                            //PDF Export Button
                            {
                                extend: 'pdf',
                                text: '<i class="fa fa-file-pdf me-2"></i>  Export PDF',
                                className: 'btn-sm btn-c-secondary d-none printbtn',
                                exportOptions: {
                                    columns: ':visible:not(.sorting_disabled)'

                                }
                            },
                            //CSV Export Button
                            {
                                extend: 'csv',
                                text: '<i class="fa fa-file-excel me-2"></i>  Export CSV',
                                className: 'btn-sm btn-c-secondary d-none printbtn',
                                exportOptions: {
                                    columns: ':visible:not(.sorting_disabled)'

                                }
                            },

                            //'<button class="btn btn-block btn-primary mb-3" onclick="' + onclickPartial + '()"><i class="fa fa-plus me-2"></i>Add New</button>',

                        ],

                        initComplete: function () {
                            // Append a dropdown filter to the DataTable filter area
                            var dropdownHtml = '<select id="dropdownFilter" class="form-control btn-sm btn-info clsDownload btn btn-secondary buttons-collection dropdown-toggle" style="width: auto;">' +
                                '<option value=""> Export </option>' +
                                '<option value="1">PDF</option>' +
                                '<option value="2">Excel</option>' +
                                '<option value="3">CSV</option>' +
                                '</select>';

                            // Append the dropdown to the filter area
                            $('div.dataTables_filter').append(dropdownHtml);

                            //if (tableID === 'AccessLogTable') {
                            //    $('#dropdownFilter option[value="2"]').hide();  // Hide Excel option
                            //}

                            // Custom filter logic when dropdown changes
                            $('#dropdownFilter').on('change', function () {
                                var value = $(this).val();
                                var table = $('#' + tableID).DataTable();

                                if (value === '1') { // PDF Export
                                    $('.buttons-pdf').click();
                                } else if (value === '2') { // PDF Export
                                    $('.buttons-excel').click();
                                }
                                else if (value === '3') { // CSV Export
                                    $('.buttons-csv').click();
                                }

                                //$('#dropdownFilter').val('');
                            });

                            $('div.dataTables_filter').css({
                                'gap': '10px' // Adds space between elements (dropdown and buttons)
                            });

                            // Optionally add some margin to the 'Add New' button to provide space between the dropdown and the button
                            $('.addbtn').css('margin-left', '10px'); // Adds margin between 'Add New' button and the dropdown
                            // $('.excelbtn').css('margin-left', '10px'); // Adds margin between 'Add New' button and the dropdown
                        },
                        drawCallback: function () {

                        },

                    })//.buttons().container().appendTo('#example1_wrapper .col-md-6:eq(0)');

                    $('div.dt-buttons').prepend($('.clsDownload'));
                    $('div.dt-buttons').css('float', 'inline-end');
                    $('.addbtn').css('border-radius', '.25rem');
                    $('.custom-class').removeClass('btn-secondary');
                    $('.dt-scroll-headInner table thead').addClass('custom-table');
                    $('.dataTables_filter').css('float', 'inline-start');
                    $('.dataTables_paginate').css('margin-top', '-30px');

                    if (response.data == "") {
                        $('#' + tableID + '_previous').css('display', 'none');
                        $('#' + tableID + '_next').css('display', 'none');
                        $('.dataTables_paginate').css('margin-top', '0px');
                    }

                    if (showAdd) {
                        $('.addbtn').removeClass('d-none');
                    }
                    else {
                        $('.addbtn').addClass('d-none');
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

