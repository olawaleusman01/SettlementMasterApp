$(document).ready(function () {

    var col;

    // var url = BaseUrl() + '/ItemType/';
    var url = BaseUrl() + 'StockRequest/';
    var url2 = BaseUrl() + 'Settings/';
    BindCombo();
    BindGrid();
    // BindCategory();
    // BindInventoryStatistic();
    //var table2 = $('.datatable').DataTable({
    //    ajax: url + "ItemList",
    //    columns: [
    //     {
    //         data: null,
    //         className: "center_column",
    //         //defaultContent: '<a class="btn btn-primary btn-xs editor_edit" data-key="' + data.ItbId + '"><i class="fa fa-edit"></i></a>' +
    //         //    '  <a class="btn btn-warning btn-xs editor_remove" data-key="' + data.ItbId + '"><i class="fa fa-trash-o"></i></a>',
    //         render: function (data, type, row) {
    //             // Combine the first and last names into a single table field
    //             var html = '<a class="btn btn-primary btn-xs editor_edit" data-key="' + data.ItbId + '"><i class="fa fa-edit"></i></a>';
    //             return html;
    //         }
    //     },
    //        { data: "ItemName" },
    //        { data: "Quantity" },
    //        { data: "ItbId" },
    //        { data: "UnitCost" },

    //        { data: "CreateDate" },
    //        {
    //            data: null,
    //            className: "center_column",
    //            //defaultContent: '<a class="btn btn-primary btn-xs editor_edit" data-key="' + data.ItbId + '"><i class="fa fa-edit"></i></a>' +
    //            //    '  <a class="btn btn-warning btn-xs editor_remove" data-key="' + data.ItbId + '"><i class="fa fa-trash-o"></i></a>',
    //            render: function (data, type, row) {
    //                // Combine the first and last names into a single table field
    //                var html = '<a class="btn btn-primary btn-xs editor_edit" data-key="' + data.ItbId + '"><i class="fa fa-edit"></i></a>';
    //                return html;
    //            }
    //        }
    //    ]
    //});

    var validator = $("#DocumentAddFormy").validate({
        rules: {
            Name: "required",
        },
        messages: {

            // equalTo: "Password must be Equal",
            Name: {
                required: "Please Enter Last Name"
            },


        },
        submitHandler: function () {
            //  e.preventDefault();
            // alert('success');
            var pDate = $('#PurchaseDate').val();
            var custId = $('#CustomerId').val();
            // var pDate = $('#PurchaseDate').val();
            if (pDate == '' && custId == '') {
                displayDialogNoty2('Notification', 'Please Select Customer and Purchase Date.<br/> <i>Select Sundry Customer if its a walk in Customer.</i>');
                return;
            }
            else if (custId == '') {
                displayDialogNoty2('Notification', 'Please Select Customer.<br/> <i>Select Sundry Customer if its a walk in Customer.</i>');
                return;
            }
            else if (pDate == '') {
                displayDialogNoty2('Notification', 'Please Select Purchase Date.', $('#PurchaseDate'));
                // $('#PurchaseDate').focus();
                return;
            }
            var itemCnt = $('.document.add .extensible tr.item:visible, .document.edit .extensible tr.item:visible').size();
            if (itemCnt <= 0) {
                displayDialogNoty('Notification', 'There are no Inventory items added to this Invoice. Add at least an item before saving invoice.');
                return;
            }
            var amtPaid = $('#AmountPaid').val();
            if (amtPaid <= 0) {
                if (!confirm('Amount Paid is not Specified.<br/> Are you Sure you want to Proceed.?')) {
                    $('#AmountPaid').focus();
                    return;
                }
            }
            var btn = $('#btnSave').val();
            var urlTemp;
            var postTemp;
            var event;


            urlTemp = url + '/CreateSales';
            postTemp = 'post';


            //  alert(urlTemp);
            var $reqLogin = {
                url: urlTemp,

                data: $('#DocumentAddForm').serialize(),
                type: postTemp,
                contentType: "application/x-www-form-urlencoded"
            };
            alert($('#DocumentAddForm').serialize());
            //return;
            loaderSpin2(true);
            disableButton(true);
            var ajax = new Ajax();
            ajax.send($reqLogin).done(function (response) {
                loaderSpin2(false);
                disableButton(false);
                if (response.RespCode === 0) {
                    // new Util().clearform('#formPriviledge');


                    // alert('Record Created Successfully');
                    displayDialogNoty('Notification', 'New Invoice Created Successfully');
                    //GetPriv($('#RoleId').val());

                }
                else {
                    // alert(response.RespMessage)
                    displayDialogNoty('alert-warning', response.RespMessage);

                }


            }).fail(function (xhr, status, err) {
                loaderSpin2(false);
                disableButton(false);
                displayModalNoty('alert-warning', 'No network connectivity. Please check your network connections.', true);
                //$('#display-error').show();
                //$('#errorMessage').text("No network connectivity. Please check your network connections.");
                // errorMessage
                // showPopDialog("No network connectivity. Please check your network connections.", handlerError);
            });


        }
    });

    $('.btnSaveInvoice').on('click', function (e) {
        // alert('lol');
        e.preventDefault();
        var lst = new Array();
        $('.document.add .extensible tr.item:visible, .document.edit .extensible tr.item:visible').each(function () {
            // itemCount++;

            var itemId = $(this).find('input.itemid').val();
            var unitPrice = (!isNaN($(this).find('input.price').val())) ? parseFloat($(this).find('input.price').val()) : 0;
            var unitQuantity = (!isNaN($(this).find('td.quantity input.quantity').val())) ? parseFloat($(this).find('td.quantity input.quantity').val()) : 0;
            lst.push({ ItemId: itemId, Price: unitPrice, Quantity: unitQuantity })
        });

        // console.log(JSON.stringify(lst));
        var form = $('#DocumentAddForm');
        var token = $('input[name="__RequestVerificationToken"]', form).val();

        var supplierId = $('#SupplierId').val();
        var pDate = $('#PurchaseDate').val();
        var briefSummary = $('#BriefSummary').val();
        var poNumber = $('#PoNumber').val();
        var publicNote = $('#PublicNote').val();
        // var amtPaid = $('#AmountPaid').val();
        var location = $('#LocationQ').val();
        //var discount = $('#Discount').val();
        // var discountDesc = $('#DiscountDesc').val();
        // var userId = $('#SaleUserId').val();

        if (pDate == '' && supplierId == '') {
            displayDialogNoty2('Notification', 'Please Select Supplier and Purchase Date.<br/> <i>Select Sundry Customer if its a walk in Customer.</i>');
            return;
        }
        else if (supplierId == '') {
            $('.select2').focus();
            displayDialogNoty2('Notification', 'Please Select Supplier or create a new supplier.');
            return;
        }
        else if (pDate == '') {
            displayDialogNoty2('Notification', 'Please Select Purchase Date.', $('#PurchaseDate'));
            // $('#PurchaseDate').focus();
            return;
        }

        else if (location == '') {
            displayDialogNoty2('Notification', 'Please Select a Location.', $('#LocationQ'));
            // $('#PurchaseDate').focus();
            return;
        }
        var itemCnt = $('.document.add .extensible tr.item:visible, .document.edit .extensible tr.item:visible').size();
        if (itemCnt <= 0) {
            displayDialogNoty('Notification', 'There are no Inventory items added to this Invoice. Add at least an item before saving invoice.');
            return;
        }
        //var amtPaid = $('#AmountPaid').val();
        //if (amtPaid <= 0) {
        //    if (!confirm('Amount Paid is not Specified.<br/> Are you Sure you want to Proceed.?')) {
        //        $('#AmountPaid').focus();
        //        return;
        //    }
        //}
        var order = {
            //AmountPaid: amtPaid,
            PoNumber: poNumber,
            LocationItbId: location,
            BriefSummary: briefSummary,
            SupplierId: supplierId,
            PublicNote: publicNote,
            // Locatio: discount,
            //DiscountDesc: discountDesc,
            PurchaseDate: pDate,
            //SaleUserId: userId,
        };
        var data = {
            __RequestVerificationToken: token,
            order: order,
            orderLineItem: lst,

        };
        //  console.log(JSON.stringify(data))
        //$.ajax({
        //    type: "POST",
        //    url: url + 'CreateSales',
        //    contentType: "application/json",
        //    data: JSON.stringify({ ItemList: lst, BatchId: $('#lblSingBatchId').text() }),// JSON.stringify(data),
        //    success: function (response) {

        //    },
        //    error: function (err) {
        //        //alert('Error Posting Record');
        //        console.log(err);
        //        loaderSpin2(false);
        //    }
        //});
        loaderSpin2(true);
        $.ajax({
            url: url + '/CreatePurchase',
            type: 'POST',
            data: data,
            success: function (response) {
                loaderSpin2(false);
                disableButton(false);
                if (response.RespCode === 0) {
                    showPrintDialog('Notification', 'New Invoice Created<br/> Print Sales Invoice');
                    //GetPriv($('#RoleId').val());
                    BindGrid();
                }
                else {
                    // alert(response.RespMessage)
                    displayDialogNoty('Notification', response.RespMessage);

                }

            },
            failure: function (result) {
                //$("#divPayroll").empty();
                loaderSpin2(false);
                //$('#btnSaveRef').prop('disabled', false);
            }
        });
    });
    function showPrintDialog(title, msg) {
        var html = msg;
        var $noty = $('#dialog-message');
        var $notyMsg = $('.notyDialogMsg');

        $noty.attr('title', title);
        $noty.html(html);
        $("#dialog-message").dialog({
            resizable: false,
            height: "auto",
            width: 400,
            modal: true,
            buttons: {
                "Print Invoice": function () {
                    $(this).dialog("close");
                    clearSalesForm();
                    // $('#pnlDetail').fadeOut();
                    //$('#pnlGrid').fadeIn();
                },
                Cancel: function () {

                    $(this).dialog("close");
                    clearSalesForm();

                }
            }
        });
    }
    function clearSalesForm() {
        $('.document.add .extensible tr.item:visible, .document.edit .extensible tr.item:visible').empty();
        new Util().clearform('#DocumentAddForm');
        
        $('.grand-total').html('0').digits();
        $('.select2').val('').trigger('change');

    }
    $("#example1").on("click", "a.editor_edit", editDetailServer);

    //$("#example1").on("click", ".current", setAsCurrent);

    function editDetailServer() {
        loaderSpin2(true);
        //  disableButton(true);
        var editLink = $(this).attr('data-key');
        col = $(this).parent();
        // alert(editLink);
        $('#ItbId').val(editLink);

        // alert( $('#ItbId').val());
        var $reqLogin = {
            url: url + 'ViewDetail/' + editLink,

            data: null,
            type: "Get",
            contentType: "application/json"
        };
        // alert($reqLogin.url);
        //$('#ajax-loading').show();
        var ajax = new Ajax();
        ajax.send($reqLogin).done(function (response) {
            loaderSpin2(false);
            if (response.RespCode === 0) {

                $('#LocationNameFull').val(response.model.LocationNameFull);
                $('#LocationNameShort').val(response.model.LocationNameShort);
                $('#Address').val(response.model.Address);
                $('#City').val(response.model.City);
                $('#CountryCode').val(response.model.CountryCode);
                $('#StateCode').val(response.model.StateCode);
                $('#ContactName').val(response.model.ContactName);
                $('#ContactEmail').val(response.model.ContactEmail);
                $('#MobileNo1').val(response.model.MobileNo1);
                //$('#ContactEmail').val(response.model.ContactEmail);
                // $('#Qu').val(response.model.Name);
                //$('#Amount').val(response.model.Amount);
                //$('#RateType').val(response.model.RateType);
                //if (response.model.ApplyToAllStaff) {
                //    $('#ApplyToAllStaff').prop('checked', true);
                //    $('#ApplyToAllStaff').prop('value', 'true');
                //}
                //else
                //{
                //    $('#ApplyToAllStaff').prop('checked', false);
                //    $('#ApplyToAllStaff').prop('value', 'false');
                //}
                //$('#Deduction_Start').val(response.model.Deduction_Start);
                //$('#Deduction_End').val(response.model.Deduction_End);
                //alert(response.model.ItbId);
                $('#ItbId').val(response.model.ItbId);

                //  $('#UserName').attr('disabled','disabled');
                $('#CreatedBy').text(response.model.CreatedBy);
                $('#CreatedDate').text(response.model.DateString);
                $('#Status').val(response.model.Status);
                $('#pnlAudit').css('display', 'block');
                $('a.editor_reset').hide();
                // $('').val(response.ExamName);
                $('#btnSave').html('<i class="fa fa-edit"></i> Update');
                $('#btnSave').val(2);
                $('#divStatus').show();

                $('#myModal').modal({ backdrop: 'static', keyboard: false });
            }
            else {
                displayDialogNoty('Notification', response.RespMessage);
            }
        }).fail(function (xhr, status, err) {
            //Disable Button
            // loginbtn.hide();
            //hide login loader
            $('#ajax-loading').hide();
            $('#display-error').show();
            $('#errorMessage').text("No network connectivity. Please check your network connections.");
            // errorMessage
            // showPopDialog("No network connectivity. Please check your network connections.", handlerError);
        });

    }

    function addGridItem(model) {
        table2.row.add(model).draw();
    }
    function updateGridItem(model) {
        var rowIdx = table2
    .cell(col)
    .index().row;

        var d = table2.row(rowIdx).data();
        d.UnitDescription = model.UnitDescription;
        d.UnitShortCode = model.UnitShortCode;

        //d.AmountString = model.AmountString;
        //d.ApplyToAllStaff = model.ApplyToAllStaff;
        //d.RateType = model.RateType;
        d.Status = model.Status

        table2
     .row(rowIdx)
     .data(d)
     .draw();

    }

    $('a.editor_create').click(function () {
        // formClear('#FormTerm');
        // new Util().clearform('#DocumentAddForm');

        // validator.resetForm();
        // $('#divStatus').hide();
        // $('#pnlAudit').css('display', 'none');
        // $('#ItbId').val(0);
        // $('#btnSave').val(1);
        // $('#btnSave').html('<i class="fa fa-save"></i> Save');
        // $('#UserName').removeAttr('disabled', 'disabled');
        /// $('a.editor_reset').show();
        // $('#myModal').modal({ backdrop: 'static', keyboard: false });
        $('#pnlDetail').fadeIn();
        $('#pnlGrid').fadeOut();
        $('.new_btn').fadeIn();
        $('.grid_btn').fadeOut();
        $('tbody.extensible').empty();
        calculator();
    });
    $('a.editor_return').click(function () {
        $('#pnlDetail').fadeOut();
        $('#pnlGrid').fadeIn();
        $('.new_btn').fadeOut();
        $('.grid_btn').fadeIn();
    });
    $('a.editor_create_item').click(function (e) {
        e.preventDefault();
        // formClear('#FormTerm');
        // new Util().clearform('#formItem');

        // validator.resetForm();
        // $('#divStatus').hide();
        // $('#pnlAudit').css('display', 'none');
        // $('#ItbId').val(0);
        // $('#btnSave').val(1);
        // $('#btnSave').html('<i class="fa fa-save"></i> Save');
        // $('#UserName').removeAttr('disabled', 'disabled');
        // $('a.editor_reset').show();
        $('#myModalItem').modal({ backdrop: 'static', keyboard: false });
    });
    $('a.editor_reset').click(function () {
        // formClear('#FormTerm');
        new Util().clearform('#formItem');

        validator.resetForm();
        $('#pnlAudit').css('display', 'none');
        $('#ItbId').val(0);

    });

    function BindCombo() {

        //  Bind Country
        //var $reqLogin = {
        //    url: url2 + 'GetItemType',
        //    data: null,
        //    type: "Get",
        //    contentType: "application/json"
        //};

        //var ajax = new Ajax();
        //ajax.send($reqLogin).done(function (response) {

        //    var exist = false;
        //    if (response.data.length > 0) {
        //        $("#ItemType_ItbId").empty();

        //        $("#ItemType_ItbId").append("<option value=''> --Select Item Category-- </option>");
        //        //  $("#Kin_Country").append("<option value=''> --Select a Country-- </option>");
        //        // $("#formRef #Country").append("<option value=''> --Select a Country-- </option>");

        //        for (var i = 0; i < response.data.length; i++) {
        //            $("#ItemType_ItbId").append("<option value='" + response.data[i].ItbId + "'>" +
        //            response.data[i].Name + "</option>");

        //        }
        //        //$("#CountryCode").val('NG');

        //    }
        //    // return response.data;
        //}).fail(function (xhr, status, err) {
        //    return null;

        //});

        //  Bind Level
        var $reqLogin = {
            url: url2 + 'GetItem',
            data: null,
            type: "Get",
            contentType: "application/json"
        };

        var ajax = new Ajax();
        ajax.send($reqLogin).done(function (response) {

            var exist = false;
            if (response.data.length > 0) {
                $("#q").empty();


                $("#q").append("<option value=''> --Select Item-- </option>");

                for (var i = 0; i < response.data.length; i++) {
                    $("#q").append("<option data-price='" + response.data[i].SellingPrice + "' value='" + response.data[i].ItbId + "'>" +
                    response.data[i].ItemName + "</option>");
                }
            }
            // return response.data;
        }).fail(function (xhr, status, err) {
            return null;

        });



        var $reqLogin = {
            url: url2 + 'GetLocation',
            data: null,
            type: "Get",
            contentType: "application/json"
        };

        var ajax = new Ajax();
        ajax.send($reqLogin).done(function (response) {

            //var exist = false;
            if (response.data.length > 0) {
                $("#LocationQ").empty();
                $("#LocationFilter_ItbId").empty();

                $("#LocationQ").append("<option value=''> Display All Location </option>");
                $("#LocationFilter_ItbId").append("<option value=''> --ALL-- </option>");
                for (var i = 0; i < response.data.length; i++) {
                    $("#LocationQ").append("<option value='" + response.data[i].ItbId + "'>" +
                    response.data[i].LocationNameFull + "</option>");
                    $("#LocationFilter_ItbId").append("<option value='" + response.data[i].ItbId + "'>" +
                   response.data[i].LocationNameFull + "</option>");
                }
            }
            // return response.data;
        }).fail(function (xhr, status, err) {
            return null;

        });


        var $reqLogin = {
            url: url2 + 'GetOrderTempStatus',
            data: null,
            type: "Get",
            contentType: "application/json"
        };

        var ajax = new Ajax();
        ajax.send($reqLogin).done(function (response) {
            $("#OrderStatus_ItbId").empty();

            $("#OrderStatus_ItbId").append("<option value=''> Display All </option>");
            //var exist = false;
            if (response.data.length > 0) {


                for (var i = 0; i < response.data.length; i++) {
                    $("#OrderStatus_ItbId").append("<option value='" + response.data[i].Code + "'>" +
                    response.data[i].Description + "</option>");
                }
            }
            // return response.data;
        }).fail(function (xhr, status, err) {
            return null;

        });
        //var $reqLogin = {
        //    url: url2 + 'SalesPerson',
        //    data: null,
        //    type: "Get",
        //    contentType: "application/json"
        //};

        //var ajax = new Ajax();
        //ajax.send($reqLogin).done(function (response) {

        //    var exist = false;
        //    if (response.data.length > 0) {
        //        $("#SaleUserId").empty();


        //        $("#SaleUserId").append("<option value=''> --Select Option-- </option>");

        //        for (var i = 0; i < response.data.length; i++) {
        //            $("#SaleUserId").append("<option value='" + response.data[i].UserName + "'>" +
        //            response.data[i].FullName + "</option>");
        //        }
        //        //alert(response.CurrentUser);
        //        $("#SaleUserId").val(response.CurrentUser);
        //    }
        //    // return response.data;
        //}).fail(function (xhr, status, err) {
        //    return null;

        //});

        var $reqLogin = {
            url: url2 + 'SupplierList',
            data: null,
            type: "Get",
            contentType: "application/json"
        };

        var ajax = new Ajax();
        ajax.send($reqLogin).done(function (response) {

            $("#SupplierId").empty();
            $("#SupplierId").append("<option value=''> --Select Supplier-- </option>");
            if (response.data.length > 0) {
                for (var i = 0; i < response.data.length; i++) {
                    $("#SupplierId").append("<option data-price='" + response.data[i].ItbId + "' value='" + response.data[i].ItbId + "'>" +
                    response.data[i].SupplierName + "</option>");
                }
                // $("#CustomerId").val(response.CurrentUser);
            }
            // return response.data;
        }).fail(function (xhr, status, err) {
            return null;

        });
    }
    function BindGrid(location, category) {
        // loaderSpin2(true);
        // alert('here');
        $("#listItem tbody").empty();
        var $reqLogin = {
            url: url + 'SRAuthList?id=', // + location + '&cat=' + category,

            data: null,
            type: "Get",
            contentType: "application/json"
        };

        var ajax = new Ajax();
        ajax.send($reqLogin).done(function (response) {
            loaderSpin2(false);
            var data = response.data;
            // alert('here 2');
            var rw = '';

            if (response.RespCode === 0) {
                if (data !== undefined) {
                    if (data.length > 0) {
                        for (var i = 0; i < data.length; i++) {
                            // console.log(JSON.stringify(data[i]));
                            rw += '<tr>'
                                    + '<td>' + data[i].OrderStatusDesc + '</td>'
                                    + '<td class="jettisonable list-center">' + data[i].OrderNumber + '</td>'
                                    + '<td class="jettisonable list-center">' + data[i].OrderDateString + '</td>'

                                    //+ '<td>' + data[i].Source + '</td>'
                                    //+ '<td>' + data[i].Destination + '</td>'
                                    + '<td class="jettisonable list-right">₦' + data[i].OrderValueString + '</td>'
                                    + '<td>' + data[i].UserFullName + '</td>'
                                    + '<td>' +
                                        '<ul class="nav nav-pills">' +
                                            '<li class="dropdown options">' +
                                                '<a class="dropdown-toggle" data-toggle="dropdown" href="#"><span>Options <span class="caret"></span></span></a>' +
                                                '<ul class="dropdown-menu" role="menu">' +
                                                    '<li><a href="' + url + "/SRAuthDetail/" + data[i].OrderNumber + '">View</a></li>' +
                                                   // (data[i].PaymentStatus != 1 && data[i].PaymentStatus != 4 ? '<li><a href="' + BaseUrl() + "Sales/Edit/" + data[i].OrderNumber + '">Edit</a></li>' : '') +
                                                    '<li><a href"#">Delete</a></li>' +
                                                    '<li href="#"><a>Archive</a></li>' +
                                                '</ul>' +
                                            '</li>' +
                                        '</ul>' +
                                    '</td>' +
                                    + '</tr>';
                            // alert(rw);
                        };
                        $("#listItem tbody").html(rw);
                        //$("#listItem").fadeOut();
                        //$("#listItem").fadeIn();
                    }
                    else {
                        rw = '<tr><td colspan="7">There is no record to display.</td></tr>'
                        $("#listItem tbody").html(rw);
                    }

                }
                else {
                    rw = '<tr><td colspan="7">There is no record to display.</td></tr>'
                    $("#listItem tbody").html(rw);
                }
            }
            else {
                displayDialogNoty('Notification', response.RespMessage);
            }
        }).fail(function (xhr, status, err) {

        });

    }
    function BindCategory() {
        try {
            // loaderSpin2(true);
            //alert('here');
            $("#listCategory").empty();
            var $reqLogin = {
                url: url2 + 'GetItemType',

                data: null,
                type: "Get",
                contentType: "application/json"
            };

            var ajax = new Ajax();
            ajax.send($reqLogin).done(function (response) {
                loaderSpin2(false);
                var data = response.data;
                // alert('here 2');
                var rw = '';

                if (response.RespCode === 0) {
                    if (data !== undefined) {
                        if (data.length > 0) {
                            for (var i = 0; i < data.length; i++) {
                                // alert(data[i].Description);
                                rw += '<li>' +
                                      '<span class="' + data[i].cat_class + '">' + data[i].Quantity + '</span>' +
                                      '<span class="name"><span class="drag">&nbsp;</span>';
                                if (data[i].Quantity > 0) {
                                    rw += '<span><a href="#" class="filter_cat" data-key="' + data[i].ItbId + '" title="' + (data[i].Description || '') + '">' + data[i].Name + '</a></span>';

                                }
                                else {
                                    rw += '<span>' + data[i].Name + '</span>';
                                }

                                rw += '</li>';

                                // alert(rw);
                            };
                            $("#listCategory").html(rw);
                            //$("#listItem").fadeOut();
                            //$("#listItem").fadeIn();
                            $('#listCategory li .filter_cat').on('click', function (e) {
                                e.preventDefault();
                                var sel = $(this).data('key');
                                // alert(sel);
                                //filter by Category
                            });
                        }
                        else {
                            rw = '<tr><td colspan="6">There is no record to display.</td></tr>'
                            $("#listCategory tbody").html(rw);
                        }

                    }
                    else {
                        rw = '<tr><td colspan="6">There is no record to display.</td></tr>'
                        $("#listCategory").html(rw);
                    }
                }
                else {
                    displayDialogNoty('Notification', response.RespMessage);
                }
            }).fail(function (xhr, status, err) {

            });
        }
        catch (err) {

        }
    }
    function BindInventoryStatistic() {
        try {
            // loaderSpin2(true);
            //alert('here');
            $("#listStatistic").empty();
            var $reqLogin = {
                url: url2 + 'GetItemStockCount',

                data: null,
                type: "Get",
                contentType: "application/json"
            };

            var ajax = new Ajax();
            ajax.send($reqLogin).done(function (response) {
                loaderSpin2(false);
                var data = response.data;
                // alert('here 2');
                var rw = '';

                if (response.RespCode === 0) {
                    if (data !== undefined) {
                        //if (data.length > 0) {
                        //for (var i = 0; i < data.length; i++) {
                        // alert(data[i].Description);
                        rw += '  <li>Total Quantity Items: <span style="float:right">' + data.ItemCount + '</span></li>';
                        rw += '  <li>Total Quantity Items: <span style="float:right">' + data.StockCount + '</span></li>';

                        // alert(rw);
                        // };
                        $("#listStatistic").html(rw);
                        //$("#listItem").fadeOut();
                        //$("#listItem").fadeIn();

                        //}
                        //else {
                        //    rw = '<tr><td colspan="6">There is no record to display.</td></tr>'
                        //    $("#listStatistic tbody").html(rw);
                        //}

                    }
                    //else {
                    //    rw = '<tr><td colspan="6">There is no record to display.</td></tr>'
                    //    $("#listCategory").html(rw);
                    //}
                }
                else {
                    displayDialogNoty('Notification', response.RespMessage);
                }
            }).fail(function (xhr, status, err) {

            });
        }
        catch (err) {

        }
    }
    var cat_itbid = null;
    var loc_itbid = null;
    $('#LocationFilter_ItbId').on('change', function () {
        loc_itbid = $(this).val();

        // alert(sel);
        //filter by location
        BindGrid(loc_itbid, cat_itbid);
    });

    $(document).on("click", '.qtyplus, .qtyminus', function (e) {
        // Stop acting like a button
        e.preventDefault();
        // Get the field name
        var fieldName = $(this).attr('field');

        var operators = {
            '+': function (a, b) { return a + b },
            '-': function (a, b) { return a - b }
        };
        var op = ''
        if ($(this).hasClass("qtyplus")) {
            op = '+';
        } else {
            op = '-';
        }

        // Get its current value
        var currentVal = parseInt($('input[id=' + fieldName + ']').val());
        // If is not undefined
        var formNameCheck = $('input[id=' + fieldName + ']').closest("form").attr('id');

        if (!isNaN(currentVal)) {
            // Increment
            if (op == '-' && (currentVal === 0 || currentVal < 0 || currentVal === 1)) {
                $('input[id=' + fieldName + ']').val(1);
            }
            else {
                $('input[id=' + fieldName + ']').val(operators[op](currentVal, 1));
            }
            // $('input[id=' + fieldName + ']').val(operators[op](currentVal, 1));

            if ((formNameCheck == 'DocumentEditForm') || (formNameCheck == 'DocumentAddForm')) {
                calculator();
            }
        } else {
            // Otherwise put a 0 there
            $('input[id=' + fieldName + ']').val(0);
            if ((formNameCheck == 'DocumentEditForm') || (formNameCheck == 'DocumentAddForm')) {
                calculator();
            }
        }
    });
    // The javascript method toFixed(2) is unpredictable betweem all browsers
    // This function ensures numbers are rounded up for accuracy.
    function toFixed(number, precision) {
        var multiplier = Math.pow(10, precision);
        return Math.round(number * multiplier) / multiplier;
    }

    function precise_round(value, decPlaces) {
        var val = value * Math.pow(10, decPlaces);
        var fraction = (Math.round((val - parseInt(val)) * 10) / 10);
        if (fraction == -0.5) fraction = -0.6;
        val = Math.round(parseInt(val) + fraction) / Math.pow(10, decPlaces);
        return val;
    }
    $(document).on("blur", '.recalculate', function (e) {
        calculator();
    });
    calculator = function () {
        // alert('here');
        var totalTax = 0;
        var totalTax2 = 0;
        var totalPrice = 0;
        var totalCosts = 0;
        var totalExpenses = 0;
        var totalDiscountPercent = 0;
        var totalDiscountAmount = 0;
        var itemCount = 0;
        var shippingFee = 0;
        var shippingCost = 0;

        $('.document.add .extensible tr:visible, .document.edit .extensible tr:visible').each(function () {
            itemCount++;
            var taxRate = (!isNaN($(this).find('input.tax').val())) ? parseFloat($(this).find('input.tax').val()) : 0;
            var taxRate2 = (!isNaN($(this).find('input.tax2').val())) ? parseFloat($(this).find('input.tax2').val()) : 0;
            var unitPrice = (!isNaN($(this).find('input.price').val())) ? parseFloat($(this).find('input.price').val()) : 0;
            var unitQuantity = (!isNaN($(this).find('td.quantity input.quantity').val())) ? parseFloat($(this).find('td.quantity input.quantity').val()) : 0;
            var unitCost = (!isNaN($(this).find('.cost input').val())) ? parseFloat($(this).find('.cost input').val()) : 0;
            var unitExpenses = $(this).find('.expenses input').length > 0 ? parseFloat($(this).find('.expenses input').val()) : 0;
            var unitDiscount = (!isNaN($(this).find('input.discount').val())) ? parseFloat($(this).find('input.discount').val()) : 0;

            var linePrice = unitPrice * unitQuantity;
            var lineTax = (linePrice * taxRate) / 100;
            var lineTax2 = (linePrice * taxRate2) / 100;
            var lineCosts = unitCost * unitQuantity;
            var lineExpenses = unitExpenses * unitQuantity;

            totalTax += lineTax;
            totalTax2 += lineTax2;
            totalPrice += linePrice;
            totalExpenses += lineExpenses;
            totalDiscountPercent += unitDiscount;
            totalCosts += lineCosts;
        });

        totalTax = precise_round(totalTax, 2);
        totalTax2 = precise_round(totalTax2, 2);


        $('.document.add .extensible tr:visible, .document.edit .extensible tr:visible').each(function () {
            if (!isNaN($(this).find('input.discount').val())) {
                dicounted_cost = (parseFloat($(this).find('input.discount').val()));
                dicounted_cost = precise_round(((dicounted_cost / 100) * totalPrice), 2);
                $(this).find('input.discount_cost').val(dicounted_cost);
                $(this).find('input.discount_price').val(dicounted_cost * -1);
            }
        });

        if (!isNaN($('#sidebar #DocumentShippingFee').val())) {
            var shippingFee = parseFloat($('#sidebar #DocumentShippingFee').val());
            var shippingCost = parseFloat($('#sidebar #DocumentShippingCost').val());
            if ((shippingFee > 0) || (shippingCost > 0)) {
                totalCosts += shippingCost;
                $('div.shipping').show();
                $('span.shipping').effect('highlight', {}, 4000);
            } else {
                $('div.shipping').hide();
            }
        }

        if (totalDiscountPercent > 0) {
            totalTax = 0;
            totalTax2 = 0;
            $('.document.add .extensible tr:visible, .document.edit .extensible tr:visible').each(function () {
                var taxRate = (!isNaN($(this).find('input.tax').val())) ? parseFloat($(this).find('input.tax').val()) : 0;
                var taxRate2 = (!isNaN($(this).find('input.tax2').val())) ? parseFloat($(this).find('input.tax2').val()) : 0;
                var unitPrice = (!isNaN($(this).find('input.price').val())) ? parseFloat($(this).find('input.price').val()) : 0;
                var unitQuantity = (!isNaN($(this).find('td.quantity input.quantity').val())) ? parseFloat($(this).find('td.quantity input.quantity').val()) : 0;
                var linePrice = unitPrice * unitQuantity;

                totalTax += precise_round(((linePrice - ((linePrice * totalDiscountPercent) / 100)) * taxRate) / 100, 2);
                totalTax2 += precise_round(((linePrice - ((linePrice * totalDiscountPercent) / 100)) * taxRate2) / 100, 2);
            });

            totalDiscountAmount = precise_round(((totalDiscountPercent / 100) * totalPrice), 2);
            $('div.discounts').show();
            $('span.discounts').effect('highlight', {}, 4000);
        } else {
            $('div.discounts').hide();
        }

        var net = ((totalPrice - totalDiscountAmount + shippingFee) - (totalCosts + totalExpenses)).toFixed(2);

        if (itemCount > 0 && $('.widget.profit-analysis .shutter').is(':visible')) {
            $('.widget.profit-analysis .shutter').hide('slide', {
                direction: 'up'
            }, 500);

        } else if (itemCount == 0 && $('.widget.profit-analysis .shutter').is(':hidden')) {
            $('.widget.profit-analysis .shutter').show('slide', {
                direction: 'up'
            }, 500);
        }

        var currencySymbol = '₦';
        //switch (jsVars.defaultCurrency) {
        //    case 'AMD': currencySymbol = 'AMD'; break;
        //    case 'BRL': currencySymbol = 'R&#36;'; break;
        //    case 'HKD': currencySymbol = 'HK&#36;'; break;
        //    case 'EGP': currencySymbol = 'LE'; break;
        //    case 'AED': currencySymbol = ''; break;
        //    case 'EUR': currencySymbol = '&#8364;'; break;
        //    case 'KWD': currencySymbol = 'K.D.'; break;
        //    case 'PHP': currencySymbol = '&#8369;'; break;
        //    case 'PKR': currencySymbol = 'Rs'; break;
        //    case 'GBP': currencySymbol = '&#163;'; break;
        //    case 'ZAR': currencySymbol = 'R'; break;
        //    case 'IDR': currencySymbol = 'Rp'; break;
        //    case 'JPY': currencySymbol = '¥'; break;
        //    case 'TRY': currencySymbol = 'TL'; break;
        //    case 'NPR': currencySymbol = 'Rs'; break;
        //    case 'QAR': currencySymbol = 'QR'; break;
        //    case 'OMR': currencySymbol = 'OMR'; break;
        //    case 'TZS': currencySymbol = 'TZS'; break;
        //    case 'AFL': currencySymbol = 'AFL'; break;
        //    case 'THB': currencySymbol = '฿'; break;
        //    case 'NGN': currencySymbol = '₦'; break;
        //    case 'BHD': currencySymbol = 'BD'; break;
        //    case 'CNY': currencySymbol = '¥'; break;
        //    case 'BBD': currencySymbol = 'Bds$'; break;
        //    case 'MUR': currencySymbol = 'Rs'; break;
        //    case 'NOK': currencySymbol = 'kr'; break;
        //    case 'CFA': currencySymbol = 'CFA'; break;
        //    case 'ANG': currencySymbol = 'ƒ'; break;
        //    case 'INR': currencySymbol = 'INR'; break;
        //    case 'LKR': currencySymbol = 'Rs'; break;
        //    case 'AZN': currencySymbol = 'ман'; break;
        //    case 'GHS': currencySymbol = 'GH¢'; break;
        //    case 'SEK': currencySymbol = 'kr'; break;
        //    case 'CHF': currencySymbol = 'CHF'; break;
        //    case 'DOP': currencySymbol = 'RD$'; break;
        //    case 'BND': currencySymbol = 'B$'; break;
        //    case 'RON': currencySymbol = 'lei'; break;
        //    case 'PEN': currencySymbol = 'S/'; break;
        //    case 'JMD': currencySymbol = 'J$'; break;
        //    case 'DKK': currencySymbol = 'kr'; break;
        //    case 'RSD': currencySymbol = 'РСД'; break;
        //    case 'ILS': currencySymbol = '₪'; break;
        //    case 'PYG': currencySymbol = '₲'; break;
        //}

        $('.totals span.pretotal').html(currencySymbol + (totalPrice).toFixed(2));
        $('.totals span.subtotal').html(currencySymbol + (totalPrice - totalDiscountAmount).toFixed(2));
        $('.totals span.nettotal').html(currencySymbol + (totalPrice - totalDiscountAmount + shippingFee).toFixed(2));
        $('.totals span.discounts').html('-' + currencySymbol + (totalDiscountAmount).toFixed(2));
        $('.totals span.tax').html(currencySymbol + (totalTax).toFixed(2));
        $('.totals span.tax2').html(currencySymbol + (totalTax2).toFixed(2));
        $('.totals span.costs').html('(' + currencySymbol + (totalCosts).toFixed(2) + ')');
        $('.totals span.shipping').html(currencySymbol + (shippingFee).toFixed(2));
        $('.totals span.expenses').html(currencySymbol + (totalExpenses).toFixed(2));
        $('.totals span.grand-total').html(currencySymbol + (totalPrice - totalDiscountAmount + totalTax + totalTax2 + shippingFee).toFixed(2));
        $('.totals span.net').html(currencySymbol + net);
        $('.totals span.margin').html((totalPrice && totalCosts > 0 ? ((((totalPrice - totalDiscountAmount + shippingFee) / (totalCosts + totalExpenses)) * 100) - 100).toFixed(2) : 0) + '%');

        $('.totals span.tax').digits();
        $('.totals span.tax2').digits();
        $('.totals span.pretotal').digits();
        $('.totals span.subtotal').digits();
        $('.totals span.discounts').digits();
        $('.totals span.shipping').digits();
        $('.totals span.grand-total').digits();
        $('.totals span.costs').digits();
        $('.totals span.net').digits();
        $('.totals span.expenses').digits();
    }

    $(document).on("click", '#selectAll', function (e) {
        var table = $(e.target).closest('table');
        $('td input:checkbox', table).prop('checked', this.checked);
    });

    $(document).on("click", ".document-items input[type='text']", function (e) {
        $(this).select();
    });

    $(document).on("click", '#itemUp, #itemDown, #itemTop, #itemBottom', function (e) {
        alert('lol');
        var row = $(this).parents("tr:first");
        if ($(this).is("#itemUp")) {
            row.insertBefore(row.prev());
        } else if ($(this).is("#itemDown")) {
            row.insertAfter(row.next());
        } else if ($(this).is("#itemTop")) {
            row.insertBefore($("table tr:first"));
        } else {
            row.insertAfter($("table tr:last"));
        }
        return false;
    });
    $(document).on("click", '.document a.delete', function (e) {
        // alert('lol');
        if ($(this).attr('href').indexOf('/') == 0 || $(this).attr('href').indexOf('http') == 0) {
            var that = this;
            $.get($(this).attr('href'), function () {
                $(that).parents('tr').remove();
                calculator();
            });
        } else {
            $(this).parents('tr').remove();
            calculator();
        }
        return false;
    });

    $('.print').on('click', function (e) {
        $('#divToPrint').load($(this).attr('href'), function () {
            //  $('#divToPrint').jqprint();
        })
        e.preventDefault();
    });

    $(document).on("click", '#btnAddItem', function (e) {
        e.preventDefault();
        var productId = $('#q').val();
        var price = $('#q').find(':selected').data('price');
        var prodName = $('#q').find(':selected').text();
        var quantity = (!isNaN($('#quantity').val())) ? parseInt($('#quantity').val()) : 1;;
        // alert($('#quantity').val());
        if (productId == '') {
            displayModalNoty('alert-warning', 'Please Select an Item.');
            // alert('Please Select an Item.');
            return;
        }
        var model = {
            ItemId: productId,
            Price: price,
            Quantity: quantity
        };
        //itemQty = $(this).siblings('.number'); //.text(newNum);
        //AddItemToCart(model);
        var exist = false;
        var itemIdex = 0;
        $('.document.add .extensible tr:visible, .document.edit .extensible tr:visible').each(function () {
            // alert($(this).data('id'));
            itemIdex++;
            var key = $(this).data('id');

            if (key == productId) {
                var unitQuantity = (!isNaN($(this).find('td.quantity input.quantity').val())) ? parseFloat($(this).find('td.quantity input.quantity').val()) : 0;
                $(this).find('td.quantity input.quantity').val(unitQuantity + quantity);
                calculator();
                exist = true;
                $('.select2Item').val('').trigger('change');
                $('#quantity').val('1');
                // $('#').trigger();
                return;

            }
        });
        if (exist)
            return;
        itemIdex++;
        var rw = '<tr class="item" data-id="' + productId + '" data-barcode="">' +
                                   '<td>' +
                                       '<div style="float: left; width: 60%; min-width: 160px;">' +
                                           '<b>' +
                                           prodName + ' </b>' +
                                           '<br><span class="light-grey" style="font-size: 11px;">ID: ' + productId + '<br>' +
                                           '<input type="hidden" class="itemid" name="ItemList[' + itemIdex + '].ItemId" value="' + productId + '" id="ItemId_' + itemIdex + '">' +
                                           '</span>' +
                                       '</div>' +
                                   '</td>' +
                                   '<td style="text-align: center; border-right: 1px solid #DDD; border-left: 1px solid #DDD;" class="quantity">' +
                                      '<div class="input text">' +
                                       '<input type="button" value="-" class="qtyminus qty-adjust" field="Quantity_' + itemIdex + '">' +
                                       '<input name="ItemList[' + itemIdex + '].Quantity" type="text" value="' + quantity + '" class="quantity recalculate" id="Quantity_' + itemIdex + '">' +
                                       '<input type="button" value="+" class="qtyplus qty-adjust" field="Quantity_' + itemIdex + '">' +
                                      '</div>' +
                                       //'<p class="light-grey" style="font-size: 11px;">Available: -100</p>' +
                                   '</td>' +
                                   '<td style="text-align: center; border-right: 1px solid #DDD; line-height: 34px;">pcs</td>' +
                                   '<td style="text-align: right;">' +
                                       '<div class="input-group input-group-sm">' +
                                           '<span class="input-group-addon" style="width: 25px; background-color: #fff;">' +
                                           '<div class="money-symbol">₦</div>' +
                                           '</span>' +
                                           '<input name="ItemList[' + itemIdex + '].Price" type="text" value="' + price + '" title="" class="money price recalculate form-control text-right" id="Price_' + itemIdex + '">' +
                                       '</div>' +
                                   '</td>' +
                                   '<td class="document-item-options">' +
                                       '<a href="javascript:void(0);" class="delete"></a>' +
                                       //'<a href="#" class="up_down" id="itemUp">&nbsp;</a>' +
                                       //'<a href="#" class="up_down" id="itemDown">&nbsp;</a>' +
                                   '</td>' +
                               '</tr>';
        $('tbody.extensible').append(rw);
        $('.select2Item').val('').trigger('change');
        $('#quantity').val('1');
        //$('#myModalItem').modal('hide');
        calculator();

    });
    var validator = $("#formOrderStatus").validate({
        rules: {
            //Reason: {
            //    required: true,
            //    number: true
            //},
            //TransactionDate: "required",
        },
        messages: {
            ////AmountPaid: {
            ////    required: "Please Enter Last Name"
            ////},


        },
        submitHandler: function () {
            var form = $('#formOrderStatus');
            var token = $('input[name="__RequestVerificationToken"]', form).val();

            var reason =$.trim($('#Reason').val());
            //var recDate = $('#ReceivedDate').val();
            //alert(sentDate);
            //alert(recDate);
            //var refId = $('#RefId').val();
            var ordNo = $('#hidOrderNo').val();

            if (reason == '') {
                displayModalNoty('alert-warning', 'Please Enter Rejection Reason.!');
                return;
            }
            // alert('lol');
            var data = {
                __RequestVerificationToken: token,
                Reason: reason,
                //ReceivedDate: recDate,
                //TranxDate: pDate,
                OrderNo: ordNo

            };
            //return;
            // console.log(JSON.stringify(data))
            loaderSpin(true);
            $.ajax({
                url: url + '/ApplyRequestRejection',
                type: 'POST',
                data: data,
                success: function (response) {
                    loaderSpin(false);
                    disableButton(false);
                    if (response.RespCode === 0) {
                        new Util().clearform('#formOrderStatus');
                        displayDialogNoty('Stock Request Authorization Notification', response.RespMessage)
                       // window.location = BaseUrl() + 'Purchase/POAuth';
                        updatePOStatus(response.data, response.actLog);
                       // $('#hidSentDate').val(response.SentDate);
                       // $('#hidReceivedDate').val(response.ReceivedDate);
                        $('#myModalStatus').modal('hide');
                    }
                    else {
                        // alert(response.RespMessage)
                        displayModalNoty('Stock Request Authorization Notification', response.RespMessage);

                    }

                },
                failure: function (result) {
                    //$("#divPayroll").empty();
                    //loaderSpin2(false);
                    //$('#btnSaveRef').prop('disabled', false);
                }
            });

        }
    });
    var curSymbol = '₦';
    function updatePOStatus(data, activityLog) {
        //alert('am here');

        //console.log(JSON.stringify(order));

        $('.paystatus').html('Status: ' + (data.ApprovedStatus ? "Approved" : "Rejected"));
         
            $('.reject,.approve').hide();
            var nameHtml = '<i class="fa fa-user"></i>' +
                            ' <span class="light-grey">Authorized By:</span> ' + data.AuthName;
          var dateHtml = '<i class="fa fa-user"></i>' +
                            ' <span class="light-grey">Authorized Date:</span> ' + data.AuthDate;
          $('.div-authname').html(nameHtml);
          $('.div-authdate').html(dateHtml);
        if (activityLog) {
            var rw = ''

            for (var i = 0; i < activityLog.length; i++) {
                rw += '  <li class="activity-item remove">' +
                            '<div class="activity-text">' +
                              '<span class="created">' + activityLog[i].DateString + '</span>' +
                                 '<span class="source"> - ' + activityLog[i].SalesFullName + ' - </span>' +
                                  '<img src="' + BaseUrl() + "Img/icon-system_activity.gif" + '" class="icon" style="margin:0;">' +
                                  '<span class="">' +
                                  '<div style="display: inline-block;">' + activityLog[i].Description + '</div></span>' +
                                                        '</div>' +
                                                    '</li>';

            }
            $('#activityLog').empty();
            $('#activityLog').append(rw);
        }

    }
    function AddItemToCart(model) {
        //alert(model.ItemId);
        //alert(model.Price);
        // url = '/Order/AddItem';
        //  console.log(JSON.stringify(model));

        var $reqLogin = {
            url: url + 'AddItem',
            data: JSON.stringify(model),    //$('#formRegister').serialize(),
            type: 'post',
            contentType: "application/json"// "application/x-www-form-urlencoded"
        };
        // alert($reqLogin.url);
        // return;
        $('#ajax-loading').show();
        $('#btnSubmit').attr('disabled', 'disabled');
        var ajax = new Ajax();
        ajax.send($reqLogin).done(function (response) {
            // $('#ajax-loading').hide();
            // $('#btnSubmit').removeAttr('disabled', 'disabled');
            // alert('success');
            if (response.responseCode === 0) {
                //$('#').val();
                //$('#btnSubmit').removeAttr('disabled', 'disabled');
                //$('#total').text(response.model.totalCart);
                //// alert(response.model.Quantity);
                //itemQty.text(response.model.Quantity);
                //$('#totalCart').text(response.model.totalCart);
                //$('#totalCartQty').text(response.model.totalCartQty);
                //// alert('Record Created Successfully');

                //$('#btnSubmit').removeAttr('disabled', 'disabled');
                //$('#pnlAudit').css('display', 'none');

            }
            else {
                console.log(response.ResponseMessage)
            }
            //  $('#SubjectID').val('0');
            // updateLocalBank(response);

        }).fail(function (xhr, status, err) {
            //Disable Button
            // loginbtn.hide();
            //hide login loader
            $('#ajax-loading').hide();
            $('#display-error').show();
            $('#errorMessage').text("No network connectivity. Please check your network connections.");
            // errorMessage
            // showPopDialog("No network connectivity. Please check your network connections.", handlerError);
            //  return null;
        });

    }

    $(document).on("click", 'a.ajax2', function (e) {
        var that = this;
        //$.ajax({
        //    url: url + 'AddDiscount', // $(this).attr('href'),
        //    success: function (responseText) {
        //        //if ($(that).parent().hasClass('add-to-document')) {
        //        //    $(that).closest('.dialog').find('#SearchKeywords').trigger('keyup');
        //        //}
        //        if (responseText) {
        //            $('tbody.extensible').append(responseText);
        //            $('#SearchKeywords').val('');
        //            $('#SearchKeywords').focus();
        //            calculator();
        //        }
        //    }
        //});
        var rw = '<tr>' +
                                     '<td style="border-right: 1px solid #DDD;">' +
                                      '<input name="DiscountDesc" type="text" value="Discount" placeholder="Enter a discount name..." class="form-control" style="font-size: 13px;" id="DiscountDesc">' +
                                     '</td>' +
                                     '<td class="quantity" style="text-align: center; border-right: 1px solid #DDD; border-left: 1px solid #DDD;">' +
                                         '<input name="Quantity" type="text" style="display: none;" value="1" class="quantity" maxlength="14" id="ItemQuantity">' +
                                         '1' +
                                     '</td>' +
                                     '<td style="text-align: center; border-right: 1px solid #DDD; line-height: 34px;">discount' +
                                       '<input type="hidden" name="UnitId" value="5" id="UnitId">' +
                                     '</td>' +
                                     '<td style="text-align: right;">' +
                                         '<div class="input-group input-group-sm" style="float: right;">' +
                                             '<input name="Discount" type="text" value="0" class="money discount recalculate form-control text-right" style="width: 52px;" id="Discount">' +
                                             '<span class="input-group-addon" style="width: 25px; background-color: #fff;">%</span>' +
                                         '</div>' +
                                     '</td>' +
                                     '<td class="document-item-options"><a href="javascript:void(0);" class="delete"></a></td>' +
                                 '</tr>';
        $('tbody.extensible').append(rw);
        return false;
    });

    $('.select2Item').select2({
        theme: "classic"
    });
    function displayDialogNoty2(title, msg, $focus) {
        //  alert('loader modal noty');
        //var html = '<button type="button" class="close" data-dismiss="alert" aria-label="Close">' +
        //                        '<span aria-hidden="true">×</span>' +
        //                    '</button>';
        var html = msg;
        var $noty = $('#dialog-message');
        var $notyMsg = $('.notyDialogMsg');

        $noty.attr('title', title);
        $noty.html(html);
        $("#dialog-message").dialog({
            modal: true,
            buttons: {
                Ok: function () {
                    $(this).dialog("close");
                    $($focus).focus();
                }
            }
        });
    }

    $('a.editOrderStatus').on('click', function (e) {
        e.preventDefault();
          alert('payment click');
        // $('.spanamountdue').html('₦' + $('#hidAmountDue').val()).digits();
        $('#SentDate').val($('#hidSentDate').val());
        $('#ReceivedDate').val($('#hidReceivedDate').val());
        $('#myModalStatus').modal({ backdrop: 'static', keyboard: false });

    });
    $('a.approve').on('click', function (e) {
        e.preventDefault();
        // $('#myModalStatus').modal({ backdrop: 'static', keyboard: false });
        if(confirm('Are you sure you want to Approve this Purchase Order.? Approving this order will make it available to be sent to Supplier.'))
        {
            var form = $('#formOrderStatus');
            var token = $('input[name="__RequestVerificationToken"]', form).val();

           // var reason = $('#Reason').val();
            //var recDate = $('#ReceivedDate').val();
            //alert(sentDate);
            //alert(recDate);
            //var refId = $('#RefId').val();
            var ordNo = $('#hidOrderNo').val();

            //if (reason == '') {
            //    displayModalNoty('alert-warning', 'Please Specify Rejection Reason.!');
            //    return;
            //}
            // alert('lol');
            var data = {
                __RequestVerificationToken: token,
                //Reason: reason,
                //ReceivedDate: recDate,
                //TranxDate: pDate,
                OrderNo: ordNo

            };
            // console.log(JSON.stringify(data))
            loaderSpin2(true);
            $.ajax({
                url: url + '/ApplyRequestApproval',
                type: 'POST',
                data: data,
                success: function (response) {
                    loaderSpin2(false);
                    disableButton(false);
                    if (response.RespCode === 0) {
                       // new Util().clearform('#formOrderStatus');
                        displayDialogNoty('PO Authorization Notification', response.RespMessage)
                        // window.location = BaseUrl() + 'Purchase/POAuth';
                        updatePOStatus(response.data, response.actLog);
                        // $('#hidSentDate').val(response.SentDate);
                        // $('#hidReceivedDate').val(response.ReceivedDate);
                       // $('#myModalStatus').modal('hide');
                    }
                    else {
                        // alert(response.RespMessage)
                        displayModalNoty('PO Authorization Notification', response.RespMessage);

                    }

                },
                failure: function (result) {
                    //$("#divPayroll").empty();
                    loaderSpin2(false);
                    //$('#btnSaveRef').prop('disabled', false);
                }
            });
        }

    });
    $('a.reject').on('click', function (e) {
        e.preventDefault();
        $('#myModalStatus').modal({ backdrop: 'static', keyboard: false });

    });
    $('a.add-note').on('click', function (e) {
        e.preventDefault();
        alert('payment click');
        //$('.spanamountdue').html('₦' + $('#hidAmountDue').val()).digits();
        //$('#AmountPaid').val($('#hidAmountDue').val());
        //$('#myModalStatus').modal({ backdrop: 'static', keyboard: false });

    })
    $('a.edit-dispatched').on('click', function (e) {
        e.preventDefault();
        alert('payment click');
        //$('.spanamountdue').html('₦' + $('#hidAmountDue').val()).digits();
        //$('#AmountPaid').val($('#hidAmountDue').val());
        //$('#myModalStatus').modal({ backdrop: 'static', keyboard: false });

    })


});
/* http://stackoverflow.com/questions/1990512/add-comma-to-numbers-every-three-digits-using-jquery */
$.fn.digits = function () {
    return this.each(function () {
        $(this).text($(this).text().replace(/(\d)(?=(\d\d\d)+(?!\d))/g, "$1,"));
    })
}