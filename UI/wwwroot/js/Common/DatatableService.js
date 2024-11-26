

function initializeDataGrid(tableID, onclickPartial, url, columnList, showDelete, showAdd, showEdit, showResetUserPassword = false, showPreview = false, onclickPreview = null, showExcel = false) {

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
                if (response.data != undefined || response.data != null) {
                    access = response.data
                }
                dataToShow = response.data;

                if ($('#' + tableID).length > 0) { //check if table exists
                    // $('#' + tableID).DataTable().destroy();
                    $('#' + tableID).DataTable({ //init table
                        dom: '<"row"<"col-md-10"B><"col-md-2"f>>rtipl', // Search on the left, buttons on the right
                        /*dom: '<"top"B><"top"f>rt<"bottom"ip>', */
                        searching: true, //show search
                        paging: false, //no paging
                        processing: true, //processing
                        scrollY: 'auto', //fixed height
                        scrollCollapse: true,//scroll
                        order: [[0, 'desc']], //order
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
                                    var del = ' | <a href="#" onclick="delete(\'' + full.id + '\')" title="Delete"><i class="fa fa-xmark text-danger"></i></a>';
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

                                    //if (access.length > 0) {
                                    //    if (access.find(x => x.accessType == AccessOperationType.Read) != null) {
                                    //        if (access.find(x => x.accessType == AccessOperationType.Read).hasAccess) {
                                    //            btn += view
                                    //        }
                                    //    }
                                    //    if (access.find(x => x.accessType == AccessOperationType.Update) != null) {
                                    //        if (access.find(x => x.accessType == AccessOperationType.Update).hasAccess) {
                                    //            btn += edit
                                    //        }
                                    //    }
                                    //    if (access.find(x => x.accessType == AccessOperationType.Delete) != null) {
                                    //        if (access.find(x => x.accessType == AccessOperationType.Delete).hasAccess) {
                                    //            btn += del
                                    //        }
                                    //    }

                                    //} else {
                                    //    btn += view + edit;
                                    //    if (showDelete) {
                                    //        btn += del
                                    //    }
                                    //    //TO REMOVE!! NO HARDCODING!! Please pass custom btn as param!!
                                    //    if (className == "User") {
                                    //        btn =
                                    //            '<div class="text-nowrap">' + view + edit + reset + del + '</div>';
                                    //    }
                                    //    if (className == "GlobalParam") {
                                    //        btn =
                                    //            '<div class="text-nowrap">' + view + edit + '</div>';
                                    //    }
                                    //}
                                    data = '<div class="text-nowrap">' + btn + '</div>';
                                    return data;
                                }
                            }
                        ],
                        columns: columnList, //column list
                        buttons: [
                            {
                                text: '<i class="fa fa-plus me-2"></i>Add New',
                                className: 'btn btn-block btn-primary custom-class mb-3 addbtn',
                                action: function (e, dt, node, config) {
                                    // Replace this with your custom action
                                    // You can dynamically invoke a function if onclickPartial is a string
                                    window[onclickPartial]();  // 

                                }
                            },
                            //Excel Export Button
                            {
                                extend: 'excel',
                                text: '<i class="fa fa-file-excel me-2"></i>Export Excel',
                                className: 'form-control btn-c-secondary d-none excelbtn',
                                exportOptions: {
                                    columns: ':visible:not(.sorting_disabled)'

                                }
                            },


                            //'<button class="btn btn-block btn-primary mb-3" onclick="' + onclickPartial + '()"><i class="fa fa-plus me-2"></i>Add New</button>',

                        ],

                        //buttons: [ //set buttons above table
                        //    //Excel Export Button
                        //    //{
                        //    //    extend: 'excel',
                        //    //    text: '<i class="fa fa-file-excel me-2"></i>Export Excel',
                        //    //    className: 'form-control btn-c-secondary d-none',
                        //    //    exportOptions: {
                        //    //        columns: ':visible:not(.sorting_disabled)'

                        //    //    }
                        //    //},
                        //    //Open Add popup

                        //    '<div class="row"><div class="col-12"><a href="#" ><button class="btn btn-block btn-primary mb-3" onclick="' + onclickPartial + '()"><i class="fa fa-plus me-2"></i>Add New</button></a></div></div>',
                        //],
                        drawCallback: function () {

                            // BUTTON EVENT ON THE DATA TABLE
                            // $('table.dataTable thead').addClass('custom-table');
                        },

                    })//.buttons().container().appendTo('#example1_wrapper .col-md-6:eq(0)');

                    $('div.dt-buttons').css('float', 'inline-end');
                    $('.custom-class').removeClass('btn-secondary');
                    $('.dt-scroll-headInner table thead').addClass('custom-table');

                    if (showAdd) {
                        $('.addbtn').removeClass('d-none');
                    }
                    else {
                        $('.addbtn').addClass('d-none');
                    }

                    if (showExcel) {
                        $('.excelbtn').removeClass('d-none');
                    }
                    else {
                        $('.excelbtn').addClass('d-none');
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

