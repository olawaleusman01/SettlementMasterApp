$(document).ready(function () {
    // alert("success");
    //startProcessing();
    //setTimeout(function () {
    //    endProcessing();
    //}, 10000);

    var col;
    var gReqType = '';
    var gQ = '';
    //var dataP = [{"AccountNo":"012345785","FullName":"Usman Olawale","CardNo":"5412369569874563","RequestType":"FRESH","RequestDate":"20/12/2016","UserId":"usman olawale","Status":"Processing"},{"AccountNo":"012345555","FullName":"Usman Badru","CardNo":"5412369569874563","RequestType":"FRESH","RequestDate":"20/12/2015","UserId":"usman olawale","Status":"Collected"}];
    // alert(typeof (dataP));
    //var table2 = $('.datatable').DataTable(
    //);
    // var cnt = 0;
    //var dataP = [{ "AccountNo": "012345785", "FullName": "Usman Olawale", "CardNo": "5412369569874563", "RequestType": "FRESH", "RequestDate": "20/12/2016", "UserId": "usman olawale", "Status": "Processing" }, { "AccountNo": "012345555", "FullName": "Usman Badru", "CardNo": "5412369569874563", "RequestType": "FRESH", "RequestDate": "20/12/2015", "UserId": "usman olawale", "Status": "Collected" }];

    var url = BaseUrl() + 'Customer/';
    BindRequestReason();
    GetAtmStatus();
    var table2 = $('.datatable_request').DataTable({
        data: null,//  url + "BindATMRequestList",
        columns: [
             {
                 data: null,
                 className: "center_column",
                 //defaultContent: '<a class="btn btn-primary btn-xs editor_edit" data-key="' + data.ItbId + '"><i class="fa fa-edit"></i></a>' +
                 //    '  <a class="btn btn-warning btn-xs editor_remove" data-key="' + data.ItbId + '"><i class="fa fa-trash-o"></i></a>',
                 render: function (data, type, row) {
                     // Combine the first and last names into a single table field
                     // var html = '<a class="btn btn-primary btn-xs editor_edit center_column" data-key="' + data.ItbId + '"><i class="fa fa-edit"></i></a>';
                     //'  <a class="btn btn-warning btn-xs editor_remove" href="/Roles/RolePriviledge/' + data.ItbId + '"><i class="fa fa-check"></i> Assign Priviledge</a>';
                     var html = '<input type="checkbox" class="selected" name="cardstat_selected" checked value="' + data.ItbId + '"/>';

                     return html;
                 }
             },



            { data: "AccountNo" },
            { data: "FullName" },
              { data: "CardNo" },
               { data: "RequestType" },
            { data: "RequestDate" },
            { data: "UserId" },
            { data: "Status" },


        ]
    });
    // table2.clear().draw();
    // table2 = $('.datatable_request').DataTable({
    //    data: dataP,//  url + "BindATMRequestList",
    //    columns: [
    //         {
    //             data: null,
    //             className: "center_column",
    //             //defaultContent: '<a class="btn btn-primary btn-xs editor_edit" data-key="' + data.ItbId + '"><i class="fa fa-edit"></i></a>' +
    //             //    '  <a class="btn btn-warning btn-xs editor_remove" data-key="' + data.ItbId + '"><i class="fa fa-trash-o"></i></a>',
    //             render: function (data, type, row) {
    //                 // Combine the first and last names into a single table field
    //                 var html = '<a class="btn btn-primary btn-xs editor_edit center_column" data-key="' + data.ItbId + '"><i class="fa fa-edit"></i></a>';
    //                 //'  <a class="btn btn-warning btn-xs editor_remove" href="/Roles/RolePriviledge/' + data.ItbId + '"><i class="fa fa-check"></i> Assign Priviledge</a>';
    //                 //var html = '<input type="checkbox" class="selected" name="card_selected" value="' + data.ItbId + '"/>';

    //                 return html;
    //             }
    //         },



    //        { data: "AccountNo" },
    //        { data: "FullName" },
    //          { data: "CardNo" },
    //           { data: "RequestType" },
    //        { data: "RequestDate" },
    //        { data: "UserId" },
    //        { data: "Status" },


    //    ]
    //});
    // GetCustomerService('');



    var validator = $("#formCustomer").validate({
        rules: {
            Accountno: "required",
            // OldAcctno: "required",
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
            var btn = $('#btnSaveCust').val();
            var urlTemp;
            var postTemp;
            var event;
            // alert($('#ItbId').val());
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

                data: $('#formCustomer').serialize(),
                type: postTemp,
                contentType: "application/x-www-form-urlencoded"
            };
            alert($('#formCustomer').serialize());
            loaderSpin(true);
            disableButton(true);
            var ajax = new Ajax();
            ajax.send($reqLogin).done(function (response) {
                loaderSpin(false);
                disableButton(false);
                if (response.RespCode === 0) {
                    // new Util().clearform('#formCustomer');

                    if (event == 'new') {
                        //  addGridItem(response.data);
                        // $('#myModal').modal('hide');
                        $('#ItbId').val(response.data.ItbId);
                        // alert(response.data.ItbId);
                        // alert( $('#ItbId').val());
                        //displayNoty('alert-success', 'Record Created Successfully');
                        BindCustomerService(response.custdata);
                        // alert(response.custdata);
                        $('#btnSaveCust').html('<i class="fa fa-edit"></i> Update');
                        $('#btnSaveCust').val(2);
                        alert('Record Created Successfully');

                    }
                    else {
                        var btn = $('#btnSave').html('<i class="fa fa-save"></i> Save');
                        // updateGridItem(response.data);
                        // $('#myModal').modal('hide');
                        // $('#ItbId').val(0);
                        // alert('Record Updated successfully');
                        // displayNoty('alert-success', 'Record Updated Successfully');
                        BindCustomerService(response.custdata);
                        alert('Record Updated Successfully');
                    }

                }
                else {
                    // alert(response.RespMessage)
                    // displayNoty('alert-warning', response.RespMessage, true);
                    alert(response.RespMessage);

                }


            }).fail(function (xhr, status, err) {
                loaderSpin(false);
                disableButton(false);
                //displayNoty('alert-warning', 'No network connectivity. Please check your network connections.', true);
                alert('No network connectivity. Please check your network connections.');
                //$('#display-error').show();
                //$('#errorMessage').text("No network connectivity. Please check your network connections.");
                // errorMessage
                // showPopDialog("No network connectivity. Please check your network connections.", handlerError);
            });


        }
    });





    $("#example1").on("click", "a.editor_edit", editDetailServer);

    //$("#example1").on("click", ".current", setAsCurrent);

    function editDetailServer() {
        //  loaderSpin(true);
        //  disableButton(true);
        var editLink = $(this).attr('data-key');
        col = $(this).parent();
        // alert(editLink);
        $('#ItbId').val(editLink);

        //  alert( $('#ItbId').val());
        var $reqLogin = {
            url: url + 'ViewRole/' + editLink,

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
                //alert(response.model.ItbId);
                $('#ItbId').val(response.model.ItbId);

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
    $("input[name$='SingReqType']").on('click', function () {
        ///alert('success');
        var reqType = $("input[name$='SingReqType']:checked").val();
        if (reqType == 'REPRINT') {
            $('#divReason').show();
        }
        else {
            $('#divReason').hide();
        }
    });
    $("input[name$='StatReqType']").on('click', function () {
        // alert('success');
        var reqType = $("input[name$='StatReqType']:checked").val();
        if (reqType == 'S') {
            $('#lblStatAccNo').html('Account No. <span class="required"> *</span>');
        }
        else {
            $('#lblStatAccNo').html('Batch ID <span class="required"> *</span>');
        }
        ClearStatusForm(true);
       
    });
    function ClearStatusForm(stat) {
        if (!stat) {
            $("input[name$='StatReqType'][value='S']").prop('checked', true);
        }
        //$('#StatReqType').text(' ');
        $('#StatAtmStatus').val('');

        //$('#StatReqCardNo').text(' ');
       // $('#StatReqDate').text(' ');
        $('#StatQ').val('');
        $('#statRequireSms').prop('checked', false);
        $('#statRequireSms').prop('value', 0);
        $('#StatQ').focus();
        table2.clear().draw();
    }
    $('#statRequireSms').on('change', function () {
        if ($(this).is(':checked')) {
            $('#statRequireSms').prop('value', 1);
        }
        else {
            $('#statRequireSms').prop('value', 0);

        }
    });
    function addGridItem(model) {
        table2.row.add(model).draw();
    }
    function updateGridItem(model) {
        var rowIdx = table2
    .cell(col)
    .index().row;

        var d = table2.row(rowIdx).data();
        d.Name = model.Name;
        d.Status = model.Status

        table2
     .row(rowIdx)
     .data(d)
     .draw();

    }
    $('#btnUpload').on('click', function (e) {
        //var files = e.target.files;
        //if (files.length > 0) {
        $("#hidReprocess").val('0');
        loaderSpin2(true);
        if (window.FormData !== undefined) {
            var fileUpload = $('#upldfile').get(0);
            var files = fileUpload.files;
            // alert(files.length);
            if (files.length <= 0) {
                loaderSpin2(false);
                alert("Please select the file you are to upload!");
                return;
            }
            $('#btnProcess').prop('disabled', true);
            var data = new FormData();
            for (var x = 0; x < files.length; x++) {
                data.append(files[x].name + x, files[x]);
            }
            var reqType = $("input[name$='upldRequestType']:checked").val();
            // alert(reqType);
            data.append('requestType', reqType);
            $.ajax({
                type: "POST",
                url: url + '/UploadFiles',
                contentType: false,
                processData: false,
                data: data,
                success: function (response) {
                    $('#upldfile').val('');
                    //  alert(response.RespMessage);
                    // alert(response.data);
                    BindUploadList(response.data, reqType, '#' + response.BatchId);
                    $('#btnProcess').removeAttr('disabled');
                    loaderSpin2(false);
                },
                error: function (err) {

                    console.log(err);
                    loaderSpin2(false);
                }
            });

        }
        else {
            alert("This browser doesn't support HTML5 file uploads!");
            loaderSpin2(false);
        }
    });
    $('#btnProcess').on('click', function (e) {

        loaderSpin2(true);
        var data = new FormData();

        var bid = $("#lblBatchId").text();
        var reprocess = $("#hidReprocess").val();
        //alert(reprocess);
        data.append('BatchId', bid);
        if (reprocess == '1') {
            data.append('Reprocess', true);
        }
        $.ajax({
            type: "POST",
            url: url + '/Process',
            contentType: false,
            processData: false,
            data: data,
            success: function (response) {
                loaderSpin2(false);
                if (response.RespCode == 0) {

                    $("#tblReprint tbody").empty();
                    $("#tblFresh tbody").empty();
                    if (response.data) {
                        if (response.data.length > 0) {
                            //alert(response.data);
                            var req = response.data[0].RequestType;
                            BindUploadList(response.data, req, $("#lblBatchId").text())
                        }
                        // some records did not get posted
                        $("#hidReprocess").val('1');

                        var msg = '<p>' + response.SucCount + ' Records Posted Successfully' + '</p>';
                        msg += '<p>' + response.FailCount + ' Records Failed to Post' + '</p>';
                        msg += '<p>Failed Transactions will remain on queue while Reason for failed Transaction can be viewed when you hover on each row</p>';

                        displayDialogNoty('Notification', msg);
                        return;
                    }
                    else {
                        $("#lblBatchId").text('');
                    }
                    $("#hidReprocess").val('0');
                    var msg = '<p>' + response.SucCount + ' Records Posted Successfully' + '</p>';
                    msg += '<p>' + response.FailCount + ' Records Failed to Post' + '</p>';
                    // msg += '<p>Failed Transactions will remain on queue while Reason for failed Transaction can be viewed when you hover on each row</p>';

                    displayDialogNoty('Notification', msg);
                    //alert(response.RespMessage);
                }
                else {
                    //alert(response.RespMessage);
                    loaderSpin2(false);

                    if (response.data) {
                        $("#tblReprint tbody").empty();
                        $("#tblFresh tbody").empty();
                        if (response.data.length > 0) {
                            //alert(response.data);
                            var req = response.data[0].RequestType;
                            BindUploadList(response.data, req, $("#lblBatchId").text())
                        }
                        // some records did not get posted
                        // $("#hidReprocess").val('1');

                        var msg = '<p>' + response.SucCount + ' Records Posted Successfully' + '</p>';
                        msg += '<p>' + response.FailCount + ' Records Failed to Post' + '</p>';
                        msg += '<p>Failed Transactions will remain on queue while Reason for failed Transaction can be viewed when you hover on each row</p>';

                        displayDialogNoty('Notification', msg);
                        return;
                    };
                }

            },
            error: function (err) {

                console.log(err);
                loaderSpin2(false);
            }
        });

    });
    $('#btnSave').on('click', function (e) {
        e.preventDefault();
         var data = {};
            // data.AcctNo = '123456';
            data.CustomerId = $('#lblCustomerId').text();
            data.AccountNumber = $('#lblNewAcctNo').text();
            data.AccountTitle = $('#lblCustomerName').text();
            data.DOB = $('#lblDob').text();
            data.CardNo = $('#lblCardNo').text();
            data.OldAcctNo = $('#lblOldAcctNo').text();
            data.Address = $('#lblAddress').text();
            data.TownName = $('#lblCity').text();
            data.StateName = $('#lblRegion').text();
            data.CustomerType = $('#lblCustomerType').text();
            data.AccountType = $('#lblAccountType').text();
            data.BranchCode = $('#lblBranchCode').text();
            data.RequestType = $("input[name$='SingReqType']:checked").val();
            data.RequestReason = $("input[name$='SingReqReason']:checked").val();
            // alert(data.RequestReason);
            var txtAcct = $.trim($('#txtcAcctNo').val());
            var bid = $.trim($('#lblSingBatchId').text());
            data.batchId = bid;
           // alert(bid);
            var lAcct = $.trim(data.AccountNumber);
            if (txtAcct == '') {
                alert('Please Account No field is required');
                return;
            }
            if (data.RequestReason == undefined && data.RequestType == 'REPRINT') {
                alert('Please you have to specify the Reprint Reason');
                return;
            }
            if (txtAcct != lAcct && txtAcct != $.trim(data.OldAcctNo)) {
                alert('Please Revalidate Account before Saving Record.');
                return;
            }
         //   if (confirm('Are you sure you want to make this Request?')) {

                loaderSpin2(true);
                $.ajax({
                    type: "POST",
                    url: url + 'ProcessSingle',
                    contentType: "application/json",
                    //processData: false,
                    data: JSON.stringify(data),
                    success: function (response) {
                        loaderSpin2(false);
                        if (response.RespCode == 0) {

                            ClearSummaryForm();
                            ResetForm();
                            // alert(response.RespMessage);
                            var msg = '<p>Record added to Batch</p>';

                            displayDialogNoty('Notification', msg);
                            BindFormBatchRequest(response.data);
                           
                        }
                        else {
                            //alert(response.RespMessage);
                            var msg = '<p>' + response.RespMessage + '</p>';
                            displayDialogNoty('Notification', msg);
                        }

                    },
                    error: function (err) {
                        //alert('Error Posting Record');
                        console.log(err);
                        loaderSpin2(false);
                    }
                });
           // }
    });
    function BindFormBatchRequest(data)
    {
       // alert('am binded');
        //alert(data);
        //alert(data.length);
        //return;
        $("#tblSingleSummary tbody").empty();
        if (data !== undefined) {
            //alert(data.length);
            if (data.length > 0) {
                var sig = data[0];
                $('#lblSingBatchId').text(sig.BatchId);
                for (var i = 0; i < data.length; i++) {
                    // alert(data[i].CanDelete);

                    var rw = '<tr style="text-align:center" title="' + data[i].ResponseMsg + '">'
                             + '<td>' + (i + 1) + '</td>'
                             + '<td style="text-align:center"> <input class="chkSingle" id="chkSingle_' + i + '" data-key="' + data[i].Id + '" type="checkbox" checked name="chkSingle"/> </td>'
                             + '<td>' + data[i].PrimaryAcctNo + '</td>'
                             + '<td>' + data[i].FullName + '</td>'
                              + '<td>' + data[i].RequestType + '</td>'
                               + '<td>' + data[i].DateCreatedString + '</td>'

                             + '</tr>';
                    $("#tblSingleSummary tbody").append(rw);
                }
            }
            else {
                //alert('am here')
                $('#lblSingBatchId').text('');
            }
        }
        else {
           // alert('am here')
            $('#lblSingBatchId').text('');
        }
    }
    $('#btnProcessSingleBatch').on('click', function (e)
    {
        e.preventDefault();
       // alert($('#formSingleRequestProcess').serialize());
        var cnt = $('#tblSingleSummary input[id*="chkSingle"]:checkbox:checked').size();
        if (cnt == 0) {
            displayDialogNoty('Notification', 'No Record is Selected in the Grid');
            return;
        }
        var lst = new Array();
        $('#tblSingleSummary input[id*="chkSingle"]:checkbox:checked').each(function (i, e) {
           // alert(i);
           // alert($(this).attr('data-key'));
            lst.push($(this).attr('data-key'));
        });
        loaderSpin2(true);
        $.ajax({
            type: "POST",
            url: url + 'ProcessSingleBatch',
            contentType: "application/json",
            data: JSON.stringify({ value: lst, BatchId: $('#lblSingBatchId').text() }),// JSON.stringify(data),
            success: function (response) {
                loaderSpin2(false);
               // alert(response.RespMessage);
                if (response.RespCode == 0) {
                    var msg = '<p>' + response.SucCount + ' Records Posted Successfully' + '</p>';
                    msg += '<p>' + response.FailCount + ' Records Failed to Post' + '</p>';
                    msg += '<p>Failed Transactions will remain on queue while Reason for failed Transaction can be viewed when you hover on each row</p>';

                    displayDialogNoty('Notification', msg);
                    BindFormBatchRequest(response.data);
                }
                else {
                    //alert(response.RespMessage);
                    var msg = '<p>' + response.RespMessage + '</p>';
                    displayDialogNoty('Notification', msg);
                }
            },
            error: function (err) {
                //alert('Error Posting Record');
                console.log(err);
                loaderSpin2(false);
            }
        });

    });
    $('#btnRefreshSingleBatch').on('click', function (e) {
        e.preventDefault();
        $('#lblSingBatchId').text('');
        $("#tblSingleSummary tbody").empty();
    });
    $('#btnDeleteSingleBatch').on('click', function (e)
    {
        e.preventDefault();
     
      
        var cnt = $('#tblSingleSummary input[id*="chkSingle"]:checkbox:checked').size();
        if(cnt == 0)
        {
            displayDialogNoty('Notification', 'No Record is Selected in the Grid');
            return;
        }
        var lst = new Array();
        $('#tblSingleSummary input[id*="chkSingle"]:checkbox:checked').each(function (i, e)
        {
           // alert(i);
           // alert($(this).attr('data-key'));
            lst.push($(this).attr('data-key'));
        });
        loaderSpin2(true);
        $.ajax({
            type: "POST",
            url: url + 'ProcessSingleBatchDelete',
            contentType: "application/json",
            data: JSON.stringify({ value: lst, BatchId : $('#lblSingBatchId').text()}),// JSON.stringify(data),
            success: function (response) {
                loaderSpin2(false);
                if (response.RespCode == 0) {
                    var msg = '<p>' + response.RespMessage + '</p>';

                    displayDialogNoty('Notification', msg);
                    BindFormBatchRequest(response.data);
                }
                else {
                    //alert(response.RespMessage);
                    var msg = '<p>' + response.RespMessage + '</p>';
                    displayDialogNoty('Notification', msg);
                }

            },
            error: function (err) {
                //alert('Error Posting Record');
                console.log(err);
                loaderSpin2(false);
            }
        });
    });
    $('#tblSingleSummary #chkAllReq').on('change', function ()
    {
        if ( $('#tblSingleSummary #chkAllReq').is(':checked'))
        {
            $("#tblSingleSummary :checkbox[name ='chkSingle']").prop("checked", true);
        }
        else
        {
            $("#tblSingleSummary :checkbox[name ='chkSingle']").prop("checked", false);
        }
    });
    $("#btnRefresh").on('click', function () {
        ResetForm();
    });
    $('#chkAllStat').on('change', function () {
        if($('#chkAllStat').is(':checked'))
        {
            $(".datatable_request :checkbox[name *='cardstat_selected']").prop("checked", "checked");
           // $(".datatable_request :checkbox[name *='cardstat_selected']").val(true);
        }
        else {
            $(".datatable_request :checkbox[name *='cardstat_selected']").removeAttr("checked");
           // $(".datatable_request :checkbox[name *='cardstat_selected']").val(false);
        }
    })
    function ResetForm() {
        $("#txtcAcctNo").val('');

        $("input[name$='SingReqType'][value='FRESH']").prop('checked', true);
        $("input[name$='SingReqReason']").prop('checked', false);
        $("#divReason").hide();
        ClearSummaryForm();
    }
    $('a.editor_create').click(function () {
        // formClear('#FormTerm');
        // alert('here');
        new Util().clearform('#formCustomer');
        GetCustomerService('');
        $("#tblCustService tbody").empty();
        validator.resetForm();
        $('#pnlAudit').css('display', 'none');
        $('#ItbId').val(0);
        $('#btnSaveCust').val(1);
        $('#btnSaveCust').html('<i class="fa fa-save"></i> Save');
        // $('#UserName').removeAttr('disabled', 'disabled');
        $('a.editor_reset').show();
        $('#divForm').fadeIn();
        $('#divGrid').fadeOut();
        //$('#myModal').modal({ backdrop: 'static', keyboard: false });
    });
    
    $('#formSingleRequest a.editor_reset').click(function () {
        // formClear('#FormTerm');
        new Util().clearform('#formSingleRequest');
    });
    $('#formSingleRequestStatus a.editor_reset1').click(function () {
        ClearStatusForm();
    });
    
    $('a.editor_reset').click(function () {
        // formClear('#FormTerm');
        new Util().clearform('#formCustomer');

        validator.resetForm();
        $('#pnlAudit').css('display', 'none');
        $('#ItbId').val(0);

    });
    var acctNo;
    $('#btnqSearch').click(function () {
        GetCustomer($(txtq).val());
    });
    $('#btnStatSearch').click(function (e) {
        e.preventDefault();
        var q = $(StatQ).val();
        var reqType = $("input[name$='StatReqType']:checked").val();

        //  alert(reqType);
        //alert(q);

        if (q == '') {
            if (reqType == 'S') {

                displayDialogNoty('Notification', 'Account No must be specified.');
                return;
            }
            else {
                displayDialogNoty('Notification', 'Batch ID No must be specified.');
                return;
            }
        }
        GetAtmEnquiry(reqType, q);
    });

    $('#btnSaveATmStatus').click(function (e) {
        e.preventDefault();
        var q = $('#StatQ').val();
        var reqType = $("input[name$='StatReqType']:checked").val();
        var status = $("#StatAtmStatus").val();
        var send_sms = $("#statRequireSms").val();
      //  alert(send_sms);
        // alert(status);
        if (q == '') {
            if (reqType == 'S') {

                displayDialogNoty('Notification', 'Account No must be specified.');
                return;
            }
            else {
                displayDialogNoty('Notification', 'Batch ID No must be specified.');
                return;
            }
        }
        if (status == '') {
            displayDialogNoty('Notification', 'Please Select ATM Status.');
            return;
        }
        var card_selected = new Array();

        $("input[name='cardstat_selected']:checked").each(function (i, e) {

            card_selected[i] = $(this).val();

        });
        //  var postData = { card_selected: card_selected };
        //  alert(card_selected);
        // alert(postData.card_selected);
         //alert(JSON.stringify({ q: q, reqType: reqType, status: status, card_selected: card_selected, send_sms: send_sms }));
        //  return;
        if (confirm('Are you sure you want to process request?')) {
            var $reqLogin = {
                url: url + 'PostATMStatus',

                data: JSON.stringify({ q: q, reqType: reqType, status: status, card_selected: card_selected, send_sms: send_sms }), // $('#formCustomer').serialize(),
                type: 'POST',
                contentType: "application/json", // "application/x-www-form-urlencoded"
            };
            // alert($('#formCustomer').serialize());
            loaderSpin2(true);
            // disableButton(true);
            var ajax = new Ajax();
            ajax.send($reqLogin).done(function (response) {
                loaderSpin2(false);
                // disableButton(false);
                if (response.RespCode === 0) {
                    ClearStatusForm();
                    displayDialogNoty('Notification', response.RespMessage);
                }
                else {
                    // alert(response.RespMessage)
                    // displayNoty('alert-warning', response.RespMessage, true);
                    //alert(response.RespMessage);
                    displayDialogNoty('Notification', response.RespMessage);
                }
            }).fail(function (xhr, status, err) {
                loaderSpin2(false);
                // disableButton(false);
                //displayNoty('alert-warning', 'No network connectivity. Please check your network connections.', true);
                alert('No network connectivity. Please check your network connections.');

            });
        }
    });
    $('#btnCoreSearch').click(function (e) {
        e.preventDefault();
        // alert('am here');

        GetCoreBankingCustomer($(txtcAcctNo).val());
    });
    function GetAtmEnquiry(reqType, q) {
        var options = {};
        options.url = url + 'BindATMRequestList'; // 'GetAtmEnquiry';
        //  alert(options.url);
        options.type = "POST";
        options.data = JSON.stringify({ q: q, reqType: reqType });
        options.dataType = "json";
        options.contentType = "application/json";
        options.success = function (response) {
            if (response.data && response.data.length > 0) {
                BindStatusForm(response.data, reqType);
            }
            else {
                table2.clear().draw();
                displayDialogNoty('Notification', 'No Record Found');
            }
        };
        options.error = function (xhr, status, err) {
            alert(status);
            return null;
        };
        $.ajax(options);
    }
    function GetAtmStatus() {
        var options = {};
        options.url = url + 'GetAtmStatus';
        // alert(options.url);
        options.type = "POST";
        options.data = null;// JSON.stringify({ AcctNo: acctNo });
        options.dataType = "json";
        options.contentType = "application/json";
        options.success = function (response) {
            $("#StatAtmStatus").empty();
            $("#StatAtmStatus").append("<option value=''> --Select Status-- </option>");
            for (var i = 0; i < response.data.length; i++) {
                $("#StatAtmStatus").append("<option value='" + response.data[i].ItbId + "'>" +
                response.data[i].StatusDescription + "</option>");

            }

        };
        options.error = function () {
           // alert("Error retrieving states!");
            return null;
        };
        $.ajax(options);
    }
    function GetCoreBankingCustomer(acctNo) {
        ClearSummaryForm();
        // alert('am here');
        loaderSpin2(true);
        var options = {};
        options.url = url + '/GetCoreBankingCustomer';
        options.type = "POST";
        options.data = JSON.stringify({ AcctNo: acctNo });
        options.dataType = "json";
        options.contentType = "application/json";
        options.success = function (response) {
            // alert(response.data);
            loaderSpin2(false);
            var data = response.data;
            //alert(JSON.stringify(data));
            //alert(data.DOBString);
            if (data) {
                $('#lblCustomerId').text(data.CustomerId);
                $('#lblNewAcctNo').text(data.AccountNumber);
                $('#lblCustomerName').text(data.AccountTitle);
                $('#lblDob').text(data.DOBString);
                $('#lblCardNo').text(data.CardNo);
                $('#lblOldAcctNo').text(data.OldAcctNo);
                $('#lblAddress').text(data.Address);
                $('#lblCity').text(data.TownName);
                $('#lblRegion').text(data.StateName);
                $('#lblCustomerType').text((data.CustomerTypeDesc || '') + ' (' + data.CustomerType + ')');
                $('#lblAccountType').text((data.AccountType || '') );
                $('#lblBranchCode').text(data.BranchCode);
                $('#lblAvailBalance').text(data.AvailableBalance);

                $('#CustomerId').val(data.CustomerId);
                $('#Accountno').val(data.AccountNumber);
                $('#OldAcctno').val(data.OldAcctno);
                $('#AccountType').val(data.AccountType);
                $('#FullName').val(data.AccountTitle);
                $('#Address').val(data.Address);
                $('#City').val(data.TownName);
                $('#Region').val(data.StateName);
                $('#CardNo').val(data.CardNo);
                $('#BranchCode').val(data.BranchCode);

            }
            else {
                alert('Record Not Found');
            }

        };
        options.error = function () {
            loaderSpin2(false);
          //  alert("Error retrieving states!");
            return null;
        };
        $.ajax(options);
    }
    function ClearSummaryForm() {
        $('#lblCustomerId').text('');
        $('#lblNewAcctNo').text('');
        $('#lblCustomerName').text('');
        $('#lblDob').text('');
        $('#lblCardNo').text('');
        $('#lblOldAcctNo').text('');
        $('#lblAddress').text('');
        $('#lblCity').text('');
        $('#lblRegion').text('');
        $('#lblAccountType').text('');
        $('#lblCustomerType').text('');
        $('#lblBranchCode').text('');
        $('#lblAvailBalance').text('');
        
    }
    function BindRequestReason() {
        var $reqLogin = {
            url: url + 'GetRequestReason',

            data: null,
            type: 'Get',
            contentType: "application/json"
        };
     
        var ajax = new Ajax();
        ajax.send($reqLogin).done(function (response) {         
            if (response.data.length > 0) {
                $("#divReq").empty();
                var dw = '';
              
                for (var i = 0; i < response.data.length; i++) {
                    dw += '<div class="radio">' 
                          + '<label>'
                          + '<input type="radio" name="SingReqReason"'
                          + 'value="'+ response.data[i].ItbId +'">' + response.data[i].Description
                          + '</label>'
                          + '</div>';
                }
                $("#divReq").html(dw);
            }
        }).fail(function (xhr, status, err) {
           
        });

    }
    function BindCustomerService(data) {
        //alert('in bind customer service');
        $("#tblCustService tbody").empty();
        if (data != null) {
            var rw = '';
            // alert('am not null');
            for (var i = 0; i < data.length; i++) {
                //        ItbId, Accountno, b.ServiceID, ServiceName, ServiceExpiryDate,
                //AllowedDailyAmount, TotalDailyAmount
                rw += '<tr>'
                        + '<td>' + (i + 1) + '</td>'

                        + '<td>' + data[i].AccountNo + '</td>'
                        + '<td>' + data[i].ServiceName + '</td>'
                        + '<td>' + data[i].ServiceExpiryDate + '</td>'
                        + '<td>' + data[i].AllowedDailyAmount + '</td>'
                        + '<td> <a class="btn btn-xs btn-primary"><i class="fa fa-edit" data-key="' + data[i].ItbId + '"> </i> Edit</a></td>'
                        + '</tr>';
                // alert(rw);
            };
            $("#tblCustService tbody").html(rw);
        }
    }
    function GetCustomerService(acctNo) {
        var options = {};
        options.url = url + '/CustomerServices';
        options.type = "POST";
        options.data = JSON.stringify({ AcctNo: acctNo });
        options.dataType = "json";
        options.contentType = "application/json";
        options.success = function (response) {
            $("#tbService tbody").empty();
            var data = response.data;
            var pp = parseInt(data.length / 2);
            var mod = data.length % 2;
            // alert(mod);
            var lp = 0;
            var rw = '';
            for (var i = 0; i < data.length; i++) {
                // alert(data[i].CanDelete);
                //var rw = '<tr>'
                //         + '<td>' + (i + 1) + '</td>'
                //         + '<td> <input type="hidden" name="RolePrivList[' + i + '].MenuId" id="MenuId_' + i + '" value="' + data[i].MenuId + '" />'
                //          + '<input type="hidden" name="RolePrivList[' + i + '].RoleAssigId" id="RoleAssigId_' + i + '" value="' + data[i].RoleAssigId + '" />'
                //         + data[i].MenuName + '</td>'
                //         + '<td style="text-align:center"> <input class="CanView" id="CanView_' + i + '" type="checkbox" name="RolePrivList[' + i + '].CanView"' + (data[i].CanView ? 'checked' : '') + ' value="' + data[i].CanView + '" /> </td>'
                //         + '<td style="text-align:center"> <input class="CanAdd" id="CanAdd_' + i + '" type="checkbox" name="RolePrivList[' + i + '].CanAdd" ' + (data[i].CanAdd ? 'checked' : '') + ' value="' + data[i].CanAdd + '" /> </td>'
                //         + '<td style="text-align:center"> <input class="CanEdit" id="CanEdit_' + i + '" type="checkbox" name="RolePrivList[' + i + '].CanEdit" ' + (data[i].CanEdit ? 'checked' : '') + ' value="' + data[i].CanEdit + '" /> </td>'
                // + '<td style="text-align:center"> <input id="CanDelete_' + i + '" type="checkbox" name="RolePrivList[' + i + '].CanDelete" ' + (data[i].CanDelete ? 'checked' : '') + ' value="' + data[i].CanDelete + '" /> </td>'
                // + '<td style="text-align:center"> <input id="CanAuthorize_' + i + '" type="checkbox" name="RolePrivList[' + i + '].CanAuthorize" ' + (data[i].CanAuthorize ? 'checked' : '') + ' value="' + data[i].CanAuthorize + '" /> </td>'
                // + '</tr>';
                if (lp == 0) {
                    rw += '<tr>'
                };
                lp++;
                rw += '<td>'
                        + '<label class="control-label">'
                         + '<input type="checkbox" name="eServiceIdList" value="' + data[i].ServiceId + '"/>'
                         + data[i].ServiceName
                         + '</label>'
                        + '</td>';

                if (lp == 2) {
                    rw += '</tr>';
                    lp = 0;

                }

            }
            if (mod > 0) {
                rw += '</tr>';
            }
            // alert(rw);
            $("#tbService tbody").append(rw);

        };
        options.error = function () {
           // alert("Error retrieving Service type!");
            return null;
        };
        $.ajax(options);
    }
    function GetCustomer(acctNo) {
        var options = {};
        options.url = url + '/GetCustomer';
        options.type = "POST";
        options.data = JSON.stringify({ AcctNo: acctNo });
        options.dataType = "json";
        options.contentType = "application/json";
        options.success = function (response) {
            $("#example1 tbody").empty();
            var data = response.data;

            var rw = '';
            for (var i = 0; i < data.length; i++) {

                rw = '<tr>'
                      + '<td> <a class="btn btn-xs btn-primary"><i class="fa fa-edit" data-key="' + data[i].ItbId + '"> </i> Edit</a></td>'
                        + '<td>' + data[i].CustomerId + '</td>'
                         + '<td>' + data[i].FullName + '</td>'
                         + '<td>' + data[i].Accountno + '</td>'
                          + '<td>' + data[i].OldAcctno + '</td>'
                         + '<td>' + data[i].CardNo + '</td>'
                        + '<td>' + data[i].AccountType + '</td>'
                       + '<td>' + data[i].BranchCode + '</td>'
                         + '<td>' + data[i].Status + '</td>'

                        + '</tr>';
            }

            // alert(rw);
            $("#example1 tbody").append(rw);

        };
        options.error = function () {
           // alert("Error retrieving states!");
            return null;
        };
        $.ajax(options);
    }
    function BindUploadList(data, requestType, batchid) {
        // alert('Bind upload list');
        if (requestType == 'REPRINT') {
            // alert('in reprint');
            $('#tblReprint').show();
            $('#tblFresh').hide();
            $("#tblReprint tbody").empty();
            if (data != null) {
                var rw = '';


                // alert('am not null');
                //if (data.length > 0) {
                    for (var i = 0; i < data.length; i++) {

                        rw += '<tr title="' + data[i].ResponseMsg + '">'
                                + '<td>' + (i + 1) + '</td>'
                                + '<td>' + data[i].FirstName + '</td>'
                                + '<td>' + data[i].LastName + '</td>'
                                + '<td>' + data[i].DateString + '</td>'
                                + '<td>' + data[i].CardNo + '</td>'
                                + '<td>' + data[i].CustomerId + '</td>'
                                + '<td>' + data[i].PrimaryAcctNo + '</td>'
                                + '<td>' + data[i].AccountType + '</td>'
                                + '<td>' + data[i].PhoneNo + '</td>'
                              //  + '<td>' + data[i].LinkedAccountNo + '</td>'
                                + '<td>' + data[i].Address + '</td>'
                                + '<td>' + data[i].City + '</td>'
                                + '<td>' + data[i].Region + '</td>'
                                + '<td>' + data[i].BranchCode + '</td>'
                                + '</tr>';
                        // alert(rw);
                    };
                    $("#tblReprint tbody").html(rw);
                    // alert(batchid);
                    $("#lblBatchId").text(batchid);
                //}
                //else {
                //    alert('ama here');
                //    $("#lblBatchId").text('');
                //}

            }
        }
        else if (requestType == 'FRESH') {
            $('#tblReprint').hide();
            $('#tblFresh').show();
            $("#tblFresh tbody").empty();
            if (data != null) {
                var rw = '';

                //if (data.length > 0) {
                    // alert('am not null');
                    for (var i = 0; i < data.length; i++) {
                        //        ItbId, Accountno, b.ServiceID, ServiceName, ServiceExpiryDate,
                        //AllowedDailyAmount, TotalDailyAmount
                        rw += '<tr title="' + data[i].ResponseMsg + '">'
                                + '<td>' + (i + 1) + '</td>'
                                + '<td>' + data[i].FirstName + '</td>'
                                + '<td>' + data[i].LastName + '</td>'
                                + '<td>' + data[i].DateString + '</td>'
                               // + '<td>' + data[i].CardNo + '</td>'
                                + '<td>' + data[i].CustomerId + '</td>'
                                + '<td>' + data[i].PrimaryAcctNo + '</td>'
                                + '<td>' + data[i].AccountType + '</td>'
                                + '<td>' + data[i].LinkedAccountNo + '</td>'
                                + '<td>' + data[i].Address + '</td>'
                                + '<td>' + data[i].PhoneNo + '</td>'
                                + '<td>' + data[i].City + '</td>'
                                + '<td>' + data[i].Region + '</td>'
                                + '<td>' + data[i].BranchCode + '</td>'
                                + '</tr>';
                        // alert(rw);
                    };
                    $("#tblFresh tbody").html(rw);
                    //  alert(batchid);
                    $("#lblBatchId").text(batchid);
                //}
                //else
                //{
                //    alert('ama here 2');
                //    $("#lblBatchId").text('');
                //}
            }
        }
    }
    function BindStatusForm(data, reqtype) {
        // var d = data[0];
        // $('#StatReqType').text(d.RequestType);
        // $('#StatAtmStatus').val(d.Status);
        // $('#StatReqCardNo').text(d.CardNo);
        // $('#StatReqDate').text(d.RequestDate);
        // $('#StatQ').val(d.st);
        // $('#statRequireSms').prop('checked', false);
        // $('#StatQ').focus();     
        //if (reqtype != 'S') {
        table2.clear().draw();

        for (var i = 0; i < data.length; i++) {
            //alert(data[i]);
            table2.row.add(data[i]).draw();
        }
        if (data.length > 0)
        {
            $('#chkAllStat').prop('checked', true);
        }
        else {
            $('#chkAllStat').prop('checked', false);
        }

        // }
    }
    $('#btnCardEnquiry').on('click', function () {
        var fromDate = $('#FromDate').val();
        var toDate = $('#ToDate').val();
      
        if (fromDate === '')
        {
            displayDialogNoty('Notification', 'From Date is required.');
            return;
        }
        if (toDate === '') {
            displayDialogNoty('Notification', 'TO Date is required.');
            return;
        }
        GetCardEnquiryGroup(fromDate, toDate);
        $('#tblCardDetail').hide();
        $('#tblCardEnquiry').show();
    });

    function GetCardEnquiryGroup(fromDate,toDate) {
      
       
        var $reqLogin = {
            url: url + 'GetCardEnquiryGroup?FromDate=' + fromDate + '&ToDate=' + toDate,

            data: null,
            type: "Get",
            contentType: "application/json"
        };
        loaderSpin2(true);
        var ajax = new Ajax();
        ajax.send($reqLogin).done(function (response) {
            loaderSpin2(false);
            var data = response.data;
            $("#tblCardEnquiry tbody").empty();
            if (response.RespCode === 0) {
                if (data !== undefined) {
                    var rw = '';

                    if (data.length > 0) {
                        // alert('am not null');
                        for (var i = 0; i < data.length; i++) {

                            rw += '<tr style="text-align:center">'
                                   // + '<td>' + (i + 1) + '</td>'
                                    + '<td><a class="btn btn-primary btn-xs editor_edit" data-key="' + data[i].BatchId + '">' + '<i class="fa fa-edit"></i></a>'
                                    + ' <form id="frmDownload" action="' + BaseUrl() + 'Customer/DownloadExcelForProduct" method="post" style="display: inline-block;">'
                                    + '<button id="btnDownloadBatchCardEnquiryAll" class="btn btn-success btn-xs" style="margin-left:3px">Download <i class="fa fa-download"></i></button>'
                                    + '<input id="BatchId" value="' + data[i].BatchId + '" name="BatchId" type="hidden" />'
                                     + '<input id="ReqType" value="' + data[i].RequestType + '" name="ReqType" type="hidden" />'
                                    + '</form></td>'
                                    + '<td>' + data[i].BatchId + '</td>'
                                    + '<td>' + data[i].TranxCount + '</td>'
                                     + '<td>' + data[i].RequestType + '</td>'
                                   // + '<td>' + data[i].CardNo + '</td>'
                                    + '<td>' + data[i].DateCreatedString + '</td>'
                                    + '<td>' + data[i].FullName + '</td>'
                                    + '</tr>';
                            // alert(rw);
                        };
                    }
                    else {
                        rw += 'tr><td style="text-align:center" colspan="6">No Record Found</td></tr>';
                    }
                    $("#tblCardEnquiry tbody").html(rw);
                   
                }
            }
            else {
                displayDialogNoty('Notification',response.RespMessage);
            }
        }).fail(function (xhr, status, err) {
        
            loaderSpin2(false);
        });

    }

    $("#tblCardEnquiry").on("click", "a.editor_edit", editCardDetailServer);

    //$("#example1").on("click", ".current", setAsCurrent);
    var enqBatchItbId = '';
    function editCardDetailServer() {
          loaderSpin2(true);
        //  disableButton(true);
        var editLink = $(this).attr('data-key');
      //  col = $(this).parent();
       //  alert(editLink);
        // $('#lblEnqItbId').val(editLink);
        enqBatchItbId = editLink;
        var $reqLogin = {
            url: url + 'GetCardEnquiryDetail?BatchId=' + editLink,

            data: null,
            type: "Get",
            contentType: "application/json"
        };
      
        var ajax = new Ajax();
        ajax.send($reqLogin).done(function (response) {
            loaderSpin2(false);
            var data = response.data;
            $("#tblCardDetail tbody").empty();
            if (response.RespCode === 0) {
                if (data !== undefined) {
                    var rw = '';
                    for (var i = 0; i < data.length; i++) {

                        rw += '<tr>'
                               // + '<td>' + (i + 1) + '</td>'
                                //+ '<td><a class="btn btn-primary btn-xs editor_edit" data-key="' + data[i].BatchId + '">' + '<i class="fa fa-edit"></i></a></td>'
                                //+ '<td><input class="" type="checkbox" name="chkSingle" value="' + data[i].BatchId + '"/></td>'
                                + '<td>' + data[i].PrimaryAcctNo + '</td>'
                                + '<td>' + data[i].FullName + '</td>'
                                + '<td>' + data[i].CardNo + '</td>'
                                + '<td>' + data[i].RequestType + '</td>'
                                + '<td>' + data[i].DateCreatedString + '</td>'
                                + '<td>' + data[i].UserId + '</td>'
                                + '<td>' + data[i].Status + '</td>'
                                + '</tr>';
                        // alert(rw);
                    };
                    $("#tblCardDetail tbody").html(rw);
                    $("#tblCardEnquiry").fadeOut();
                    $("#tblCardDetail").fadeIn();
                    
                }
            }
            else {
                displayDialogNoty('Notification',response.RespMessage );
            }
        }).fail(function (xhr, status, err) {
           
        });

    }
     
      
    $('#tblCardDetail #btnBackList').click(function () {
        enqBatchItbId = '';
        $("#tblCardEnquiry").fadeIn();
        $("#tblCardDetail").fadeOut();
    });
    $('#tblCardDetail #btnDownloadCardEnquiry').click(function (e) {
        e.preventDefault();
         alert(enqBatchItbId);
        alert('i get clicked');
      
        var lst = new Array();
       
        lst.push(enqBatchItbId);
       
        loaderSpin2(true);
        DownLoadFile(lst);
    });
    $('#tblCardEnquiry #btnDownloadBatchCardEnquiry').click(function (e) {
        e.preventDefault();
       // alert('i get clicked');
        var cnt = $('#tblCardEnquiry input[name*="chkSingle"]:checkbox:checked').size();
       // alert(cnt);
        if (cnt == 0) {
            displayDialogNoty('Notification', 'No Record is Selected in the Grid');
            return;
        }
        var lst = new Array();
        $('#tblCardEnquiry input[name*="chkSingle"]:checkbox:checked').each(function (i, e) {
             //alert(i);
            //alert($(this).val());
            lst.push($(this).val());
        });
        //return;
        loaderSpin2(true);
        DownLoadFile(lst);
    });
    function DownLoadFile(bList)
    {
        $.ajax({
            type: "POST",
            url: url + 'DownloadExcelForProduct',
            contentType: "application/json",
            data: JSON.stringify({ selected_value: bList }),// JSON.stringify(data),
            success: function (response) {
                loaderSpin2(false);
                if (response.RespCode == 0) {

                }
                else {
                    //alert(response.RespMessage);
                    var msg = '<p>' + response.RespMessage + '</p>';
                    displayDialogNoty('Notification', msg);
                }
            },
            error: function (err) {
                //alert('Error Posting Record');
                console.log(err);
                loaderSpin2(false);
            }
        });
    }
});