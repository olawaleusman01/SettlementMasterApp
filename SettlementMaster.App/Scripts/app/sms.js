$(document).ready(function () {
    // alert("success");
    //startProcessing();
    //setTimeout(function () {
    //    endProcessing();
    //}, 10000);

    var col;
    //var table2 = $('.datatable').DataTable(
    //);
    // var cnt = 0;
    var url = BaseUrl() + 'sms/';
    BindCombo();
    var table2 = $('.datatable').DataTable({
        "pageLength": 100,
        ajax:null, // url + "SmsLogList",
        columns: [
            {
                data: null,
                className: "center_column",
               // defaultContent: '<input type="checkbox" id="chkAll" name="chkAll" />',
                render: function (data, type, row) {
                    // Combine the first and last names into a single table field
                    var html = '<input type="checkbox" id="chkSingle" name="chkSingle" ' + 'value="' + data.ItbId + '" />';
                    return html;
                }
            },
           // { data: "ServiceId" },
            { data: "AccountNo" },
            { data: "AccountName" },
           // { data: "Request Type" },
            { data: "BatchId" },
            {
                data: null,
                className: "center_column",
                //defaultContent: '<a class="btn btn-primary btn-xs editor_edit" data-key="' + data.ServiceId + '"><i class="fa fa-edit"></i></a>' +
                //    '  <a class="btn btn-warning btn-xs editor_remove" data-key="' + data.ServiceId + '"><i class="fa fa-trash-o"></i></a>',
                render: function (data, type, row) {
                    // Combine the first and last names into a single table field
                    var html = '<a class="btn btn-primary btn-xs editor_post" data-key="' + data.ItbId + '"><i class="fa fa-check"></i> Send</a>' +
                        '<a style="margin-left:3px" class="btn btn-danger btn-xs editor_delete" data-key="' + data.ItbId + '"><i class="fa  fa-trash-o"></i> Delete</a>';

                    return html;
                }
            }
        ]
    });

    var table3 = $('.datatable2').DataTable({
        ajax: url + "SmsTemplateList",
        columns: [
            //{
            //    data: null,
            //    className: "center_column",
            //    // defaultContent: '<input type="checkbox" id="chkAll" name="chkAll" />',
            //    render: function (data, type, row) {
            //        // Combine the first and last names into a single table field
            //        var html = '<input type="checkbox" id="chkSingle" name="chkSingle" ' + 'value="' + data.ItbId + '" />';
            //        return html;
            //    }
            //},
           // { data: "ServiceId" },
            { data: "MessageTitle" },
            { data: "Message" },
           { data: "Status" },
            {
                data: null,
                className: "center_column",
                //defaultContent: '<a class="btn btn-primary btn-xs editor_edit" data-key="' + data.ServiceId + '"><i class="fa fa-edit"></i></a>' +
                //    '  <a class="btn btn-warning btn-xs editor_remove" data-key="' + data.ServiceId + '"><i class="fa fa-trash-o"></i></a>',
                render: function (data, type, row) {
                    // Combine the first and last names into a single table field
                    var html = '<a class="btn btn-primary btn-xs editor_edit" data-key="' + data.ItbId + '"><i class="fa fa-edit"></i> </a>' ;
                      

                    return html;
                }
            }
        ]
    });



    BindGrid();

    var validator = $("#formSmsTemplate").validate({
        rules: {
            MessageTitle: "required",
            Message: "required",
            //ChargeAmount: "required",
            // ChargeGlAccount: "required",
        },
        //messages: {

        //    // equalTo: "Password must be Equal",
        //    Name: {
        //        required: "Please Enter Last Name"
        //    },


        //},
        submitHandler: function () {
            //  e.preventDefault();
            // alert('success');
            var btn = $('#btnSaveTemplate').val();
            var urlTemp;
            var postTemp;
            var event;
            if (btn == "1") {

                urlTemp = url + 'Create';
                postTemp = 'post';
                $('#ItbId').val(0);
                event = 'new';
            }
            else {

                urlTemp = url + 'Create';
                //postTemp = 'put';
                postTemp = 'post';
                event = 'modify';
            };
            //  alert(urlTemp);
            var $reqLogin = {
                url: urlTemp,

                data: $('#formSmsTemplate').serialize(),
                type: postTemp,
                contentType: "application/x-www-form-urlencoded"
            };
           // alert($('#formSmsTemplate').serialize());
            loaderSpin(true);
            disableButton(true);
            var ajax = new Ajax();
            ajax.send($reqLogin).done(function (response) {
                loaderSpin(false);
                disableButton(false);
                if (response.RespCode === 0) {
                    new Util().clearform('#formSmsTemplate');

                    if (event == 'new') {
                        addGridItemTemplate(response.data);
                        $('#myModalSms').modal('hide');
                        $('#ItbId').val(0);
                        // alert('Record Created Successfully');
                        displayNoty('alert-success', 'Record Created Successfully');
                    }
                    else {
                        var btn = $('#btnSaveTemplate').html('<i class="fa fa-save"></i> Save');
                        updateGridItemTemplate(response.data);
                        $('#myModalSms').modal('hide');
                        $('#ItbId').val(0);
                        // alert('Record Updated successfully');
                        displayNoty('alert-success', 'Record Updated Successfully');

                    }

                }
                else {
                    // alert(response.RespMessage)
                    displayModalNoty('alert-warning', response.RespMessage, true);

                }


            }).fail(function (xhr, status, err) {
                loaderSpin(false);
                disableButton(false);
                displayModalNoty('alert-warning', 'No network connectivity. Please check your network connections.', true);
                //$('#display-error').show();
                //$('#errorMessage').text("No network connectivity. Please check your network connections.");
                // errorMessage
                // showPopDialog("No network connectivity. Please check your network connections.", handlerError);
            });


        }
    });

    $('#btnSave').click(function (e) {
        e.preventDefault();
    
        var msgtype = $("input[name$='MessageType']:checked").val();
        var recOpt = $("input[name$='RecipentOpt']:checked").val();
        var acctNo = $("#AccountNo").val();
        var rqstMsg = $("#RequestMsg").val(); 
        var msg = $("#Message").val();
        //alert(send_sms);
        //alert(status);
        //alert(msgtype);
        //alert(recOpt);
        //alert(acctNo);
        //alert(acctNo);
        if (msgtype == '' || recOpt == '' || acctNo == '') {
            displayModalNoty('alert-warning', 'All Fields are Compulsory');
            return;
        }
        else {
            if (msgtype == 'SM' && rqstMsg == '') {

                displayModalNoty('alert-warning', 'All Fields are Compulsory');
                return;
            }
            else if (msgtype == 'CM' && msg == '') {
                displayModalNoty('alert-warning', 'All Fields are Compulsory');
                return;
            }
        }
       
        //var card_selected = new Array();

        //$("input[name='cardstat_selected']:checked").each(function (i, e) {

        //    card_selected[i] = $(this).val();

        //});
        //  var postData = { card_selected: card_selected };
        //  alert(card_selected);
        // alert(postData.card_selected);
        //alert(JSON.stringify({ q: q, reqType: reqType, status: status, card_selected: card_selected, send_sms: send_sms }));
        //  return;
        //if (confirm('Are you sure you want to process request?')) {
     
        var data = {
            ItbId:0,
            AccountNo:acctNo,
            Message:msg,
            MessageType:msgtype,
            RecipentOption: recOpt,
            RequestMsg: rqstMsg
        }
            var $reqLogin = {
                url: url + 'PostSMS',

                data: JSON.stringify(data), // $('#formCustomer').serialize(),
                type: 'POST',
                contentType: "application/json", // "application/x-www-form-urlencoded"
            };
            // alert($('#formCustomer').serialize());
            loaderSpin(true);
            // disableButton(true);
            var ajax = new Ajax();
            ajax.send($reqLogin).done(function (response) {
                loaderSpin(false);
                // disableButton(false);
                if (response.RespCode === 0) {
                    ClearStatusForm();
                    $('#myModal').modal('hide');
                    $('#ItbId').val(0);
                    displayDialogNoty('Notification', response.RespMessage);
                   // addGridItem(response.data);
                   // alert(JSON.stringify(response.data))
                    BindGrid2(response.data);
                  
                    // alert('Record Updated successfully');
                   // displayNoty('alert-success', 'Record Updated Successfully');
                }
                else {
                    // alert(response.RespMessage)
                    // displayNoty('alert-warning', response.RespMessage, true);
                    //alert(response.RespMessage);
                    displayModalNoty('alert-warning', response.RespMessage);
                }
            }).fail(function (xhr, status, err) {
                loaderSpin(false);
                // disableButton(false);
                //displayNoty('alert-warning', 'No network connectivity. Please check your network connections.', true);
                alert('No network connectivity. Please check your network connections.');

            });
       // }
    });

    function ClearStatusForm(){

    }

    $("#example1").on("click", "a.editor_delete", DeleteSmsSingle);
    $("#example1").on("click", "a.editor_post", PostSmsSingle);
    $("#example2").on("click", "a.editor_edit", editDetailServerTemplate);
    //$("#example1").on("click", ".current", setAsCurrent);
    $('#chkAll').on('change', function () {
        if ($('#chkAll').is(':checked')) {
            $(".datatable :checkbox[name *='chkSingle']").prop("checked", "checked");
            // $(".datatable_request :checkbox[name *='cardstat_selected']").val(true);
        }
        else {
            $(".datatable :checkbox[name *='chkSingle']").removeAttr("checked");
            // $(".datatable_request :checkbox[name *='cardstat_selected']").val(false);
        }
    })
    $('#btnPostSmsAll').click(function (e) {
        e.preventDefault();
        try
        {
            var form = $('#formSms');
            var token = $('input[name="__RequestVerificationToken"]', form).val();
            var cnt = $('input[name="chkSingle"]:checkbox:checked').size();
            //  alert(cnt);
            if (cnt <= 0)
            {
                displayDialogNoty('Notification', 'No record is selected.');
                return;
            }
            var card_selected = new Array();

            $("input[name='chkSingle']:checked").each(function (i, e) {

                card_selected[i] = $(this).val();

            });
            var data = {
                selected: card_selected,
                __RequestVerificationToken: token,
            }

           // alert('me---' + JSON.stringify(data));
            loaderSpin2(true);
            $.ajax({
                url: url + 'PostSmsMultiple',
                type: 'POST',
                data: data,
                success: function (response) {
                    //$('#btnSaveRef').prop('disabled', false);
                    loaderSpin2(false);
                    // disableButton(false);
                    if (response.RespCode === 0) {

                        displayDialogNoty('Notification', 'Message Sent Successfully');

                        BindGrid2(response.data);

                    }
                    else {

                        displayDialogNoty('Notification', response.RespMessage);

                    }
                },
                failure: function (result) {
                    loaderSpin2(false);
                    //$('#btnSaveRef').prop('disabled', false);
                }
            });
        }
        catch (err) {
           // alert(err)
            loaderSpin2(false);
        }
    });
    function PostSmsSingle()
    {
            var editLink = $(this).attr('data-key');
       //alert(editLink)
            var form = $('#formSms');
            var token = $('input[name="__RequestVerificationToken"]', form).val();
        //$('input[name="selected_subject[]"]:selected').each(function (i, e) {
        //    alert(i);
        //});

            //var classItbId = $('#Class_ItbId', form).val();
            //var subjectItbId = $('#Subject_ItbId').val();
        //var classTypeItbId = $('#ClassType_ItbId').val();
            var data = {
                ItbId: editLink,
                __RequestVerificationToken: token,
            }
         
            // alert(JSON.stringify(data));
            loaderSpin2(true);
            $.ajax({
                url: url + 'PostSmsSingle',
                type: 'POST',
                data: data,
                success: function (response) {
                    //$('#btnSaveRef').prop('disabled', false);
                    loaderSpin2(false);
                    // disableButton(false);
                    if (response.RespCode === 0) {
                       
                        displayDialogNoty('Notification', 'Message Sent Successfully');

                        BindGrid2(response.data);

                    }
                    else {

                        displayDialogNoty('Notification', response.RespMessage);

                    }
                },
                failure: function (result) {
                    loaderSpin2(false);
                    //$('#btnSaveRef').prop('disabled', false);
                }
            });
       
    }
    $('#btnDeleteSmsAll').click(function (e) {
        e.preventDefault();
        try {
            var form = $('#formSms');
            var token = $('input[name="__RequestVerificationToken"]', form).val();
            var cnt = $('input[name="chkSingle"]:checkbox:checked').size();
            //  alert(cnt);
            if (cnt <= 0) {
                displayDialogNoty('Notification', 'No record is selected.');
                return;
            }
            var card_selected = new Array();

            $("input[name='chkSingle']:checked").each(function (i, e) {

                card_selected[i] = $(this).val();

            });
            var data = {
                selected: card_selected,
                __RequestVerificationToken: token,
            }

           // alert('me---' + JSON.stringify(data));
            loaderSpin2(true);
            $.ajax({
                url: url + 'DeleteSmsMultiple',
                type: 'POST',
                data: data,
                success: function (response) {
                    //$('#btnSaveRef').prop('disabled', false);
                    loaderSpin2(false);
                    // disableButton(false);
                    if (response.RespCode === 0) {

                        displayDialogNoty('Notification', 'Message Sent Successfully');

                        BindGrid2(response.data);

                    }
                    else {

                        displayDialogNoty('Notification', response.RespMessage);

                    }
                },
                failure: function (result) {
                    loaderSpin2(false);
                    //$('#btnSaveRef').prop('disabled', false);
                }
            });
        }
        catch (err) {
            alert(err)
            loaderSpin2(false);
        }
    });
    function DeleteSmsSingle() {
        var editLink = $(this).attr('data-key');
        //alert(editLink)
        var form = $('#formSms');
        var token = $('input[name="__RequestVerificationToken"]', form).val();
        //$('input[name="selected_subject[]"]:selected').each(function (i, e) {
        //    alert(i);
        //});

        //var classItbId = $('#Class_ItbId', form).val();
        //var subjectItbId = $('#Subject_ItbId').val();
        //var classTypeItbId = $('#ClassType_ItbId').val();
        var data = {
            ItbId: editLink,
            __RequestVerificationToken: token,
        }

        // alert(JSON.stringify(data));
        loaderSpin2(true);
        $.ajax({
            url: url + 'DeleteSmsSingle',
            type: 'POST',
            data: data,
            success: function (response) {
                //$('#btnSaveRef').prop('disabled', false);
                loaderSpin2(false);
                // disableButton(false);
                if (response.RespCode === 0) {

                    displayDialogNoty('Notification', 'Record Deleted Successfully');

                    BindGrid2(response.data);

                }
                else {

                    displayDialogNoty('Notification', response.RespMessage);

                }
            },
            failure: function (result) {
                loaderSpin2(false);
                //$('#btnSaveRef').prop('disabled', false);
            }
        });

    }
    function deleteDetailServer()
    {
        if(!confirm('Are you sure you want to delete record?'))
        {
            return;
        }
        alert('deleted');
    }
    function editDetailServer() {
        //  loaderSpin(true);
        //  disableButton(true);
        var editLink = $(this).attr('data-key');
        col = $(this).parent();
        // alert(editLink);
        $('#ServiceId').val(editLink);

        //  alert( $('#ServiceId').val());
        var $reqLogin = {
            url: url + 'ViewServiceType/' + editLink,

            data: null,
            type: "Get",
            contentType: "application/json"
        };
        // alert($reqLogin.url);
        //$('#ajax-loading').show();
        var ajax = new Ajax();
        ajax.send($reqLogin).done(function (response) {
            //  alert(response.model);
            if (response.RespCode === 0) {

                $('#Name').val(response.model.Name);
                //alert(response.model.ServiceId);
                $('#ServiceId').val(response.model.ServiceId);
                $('#ServiceName').val(response.model.ServiceName);
                $('#ChargeAmount').val(response.model.ChargeAmount);
                $('#ChargeGlAccount').val(response.model.ChargeGlAccount);

                //  $('#UserName').attr('disabled','disabled');
                $('#CreatedBy').text(response.model.CreatedBy);
                $('#CreatedDate').text(response.model.DateString);
                //  $('#Status').text(response.model.Status);
                $('#pnlAudit').css('display', 'block');
                $('a.editor_reset').hide();
                // $('').val(response.ExamName);
                $('#btnSave').html('<i class="fa fa-edit"></i> Update');
                $('#btnSave').val(2);

                $('#myModal').modal({ backdrop: 'static', keyboard: false });
            }
            else {
                alert(response.RespMessage + 'edit error');
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
        d.ServiceTypeId = model.ServiceTypeId;
        d.ServiceName = model.ServiceName
        d.ChargeAmount = model.ChargeAmount;
        d.ChargeGlAccount = model.ChargeGlAccount;
        d.Status = model.Status

        table2
     .row(rowIdx)
     .data(d)
     .draw();

    }

    function editDetailServerTemplate() {
        //  loaderSpin(true);
        //  disableButton(true);
        var editLink = $(this).attr('data-key');
        col = $(this).parent();
        // alert(editLink);
        $('#formSmsTemplate #ItbId').val(editLink);

        //  alert( $('#ServiceId').val());
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
            //  alert(response.model);
            if (response.RespCode === 0) {

                $('#Name').val(response.model.Name);
                //alert(response.model.ServiceId);
                $('#ItbId').val(response.model.ItbId);
                $('#MessageTitle').val(response.model.MessageTitle);
                $('#formSmsTemplate #Message').val(response.model.Message);

                //  $('#UserName').attr('disabled','disabled');
                $('#CreatedBy').text(response.model.CreatedBy);
                $('#CreatedDate').text(response.model.DateString);
                //  $('#Status').text(response.model.Status);
                $('#pnlAudit').css('display', 'block');
                $('#btnRefreshTemplate').hide();
                // $('').val(response.ExamName);
                $('#btnSaveTemplate').html('<i class="fa fa-edit"></i> Update');
                $('#btnSaveTemplate').val(2);

                $('#myModalSms').modal({ backdrop: 'static', keyboard: false });
            }
            else {
                alert(response.RespMessage + 'edit error');
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

    function addGridItemTemplate(model) {
        table3.row.add(model).draw();
    }
    function updateGridItemTemplate(model) {
        var rowIdx = table3
    .cell(col)
    .index().row;

        var d = table3.row(rowIdx).data();
        d.MessageTitle = model.MessageTitle;
        d.Message = model.Message
        d.Status = model.Status

        table3
     .row(rowIdx)
     .data(d)
     .draw();

    }
    $('a.editor_create').click(function () {
        // formClear('#FormTerm');
        new Util().clearform('#formSms');

       // validator.resetForm();
        $('#pnlAudit').css('display', 'none');
        $('#ItbId').val(0);
        $('#btnSave').val(1);
        $('#btnSave').html('<i class="fa fa-envelope"></i> Send');
        $("input[name$='MessageType'][value='SM']").prop('checked', true);
        $("input[name$='RecipentOpt'][value='A']").prop('checked', true);
        $('#lblRecipent').html('Account No.');
        $('#divStatic').show();
        $('#divCustom').hide();
        // $('#UserName').removeAttr('disabled', 'disabled');
        $('a.editor_reset').show();
        $('#myModal').modal({ backdrop: 'static', keyboard: false });
    });
    $('a.editor_reset').click(function () {
        // formClear('#FormTerm');
        new Util().clearform('#formSms');

       // validator.resetForm();
        $('#pnlAudit').css('display', 'none');
        $('#ServiceId').val(0);

    });
    $("input[name$='MessageType']").on('click', function () {
        ///alert('success');
        var msgType = $("input[name$='MessageType']:checked").val();
        if (msgType == 'SM') {
            $('#divStatic').show();
            $('#divCustom').hide();
        }
        else {
            $('#divCustom').show();
            $('#divStatic').hide();
        }
    });
    $("input[name$='RecipentOpt']").on('click', function () {
        ///alert('success');
        var msgType = $("input[name$='RecipentOpt']:checked").val();
        if (msgType == 'A') {
            
            $('#lblRecipent').html('Account No.');
          //  $('#divCustom').hide();
        }
        else {
            $('#lblRecipent').html('Batch ID.');
        }
    });

    $('#btnAddTemplate').click(function () {
        // formClear('#FormTerm');
        new Util().clearform('#formSmsTemplate');

        // validator.resetForm();
        $('#pnlAudit').css('display', 'none');
        $('#formSmsTemplate #ItbId').val(0);
        $('#btnSaveTemplate').val(1);
        $('#btnSaveTemplate').html('<i class="fa fa-save"></i> Save');
        // $('#UserName').removeAttr('disabled', 'disabled');
        $('#btnRefreshTemplate').show();
        $('#myModalSms').modal({ backdrop: 'static', keyboard: false });
    });
    $('#btnRefreshTemplate').click(function () {
        // formClear('#FormTerm');
        new Util().clearform('#formSmsTemplate');

        // validator.resetForm();
        $('#pnlAudit').css('display', 'none');
        $('#formSmsTemplate #ItbId').val(0);

    });

    function BindCombo() {
        try {
            //  Bind Level
            var $reqLogin = {
                url: BaseUrl() + 'sms/SmsTemplateListFilter',
                data: null,
                type: "Get",
                contentType: "application/json"
            };
            // alert($reqLogin.url);
            var ajax = new Ajax();
            ajax.send($reqLogin).done(function (response) {
                //alert(response.data.length);
                var exist = false;
                if (response.data.length > 0) {
                    $("#RequestMsg").empty();
                    $("#RequestMsg").append("<option value=''> --Select Option-- </option>");
         
                    for (var i = 0; i < response.data.length; i++) {
                        $("#RequestMsg").append("<option value='" + response.data[i].ItbId + "'>" +
                       response.data[i].MessageTitle + "</option>");
                    }
                }
                // return response.data;
            }).fail(function (xhr, status, err) {
                return null;

            });
        }
        catch (err) {

        }
    }

    function BindGrid()
    {
        try {
            //  Bind Level
            var $reqLogin = {
                url: BaseUrl() + 'sms/SmsLogList',
                data: null,
                type: "Get",
                contentType: "application/json"
            };
            // alert($reqLogin.url);
            var ajax = new Ajax();
            ajax.send($reqLogin).done(function (response) {
                table2.clear().draw();
                var data = response.data;
                for (var i = 0; i < data.length; i++) {
                    //alert(data[i]);
                    table2.row.add(data[i]).draw();
                }
            }).fail(function (xhr, status, err) {
                return null;

            });
        }
        catch (err) {

        }
      
    }
    function BindGrid2(data)
    {
        table2.clear().draw();
       // var data = response.data;
        for (var i = 0; i < data.length; i++) {
            //alert(data[i]);
            table2.row.add(data[i]).draw();
        }
    }
});