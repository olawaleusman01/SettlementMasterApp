 
$(document).ready(function () {
    var url = BaseUrl() + 'NAPS/';
    $(document).on('click','#btnGenerate',function (e) {

        e.preventDefault();
        $('#divGrid tbody').empty();
        var dt = $.trim($('#formGenerate #SetDate').val());
        //var rt = $.trim($('#ReportType').val());
        //alert(rt);
        if (dt === '') {
            //loaderSpin2(true);
            displayDialogNoty('Validation Nofification', 'All Fields are Compulsory.');
            return;
        }

        var urlPath = url + "GenerateNaps";

        //console.log(url);
        loaderSpin2(true);
        //var reqData = JSON.stringify($('#formGenerate').serialize());
        //console.log(reqData);

        var $reqLogin = {
            url: urlPath,
            data: $('#formGenerate').serialize(),
            type: 'post',
            contentType: "application/x-www-form-urlencoded"
        };
        //console.log($('#formGenerate').serialize());
        //return;
        var ajax = new Ajax();
        ajax.send($reqLogin).done(function (data) {
            loaderSpin2(false);
            //alert(data.RespCode);
            if (data.RespCode == '0') {
                //console.log(data.data_html);
                $('#btnProcess').prop('disabled', false);
                $('#divGrid').html(data.data_html);
            }
            else {
                $('#btnProcess').prop('disabled', true);
                displayDialogNoty('Notification', data.RespMessage);

            }
        }).fail(function (xhr, status, err) {
            loaderSpin2(false);
            displayDialogNoty('Notification', 'No network connectivity. Please check your network connections.', true);
        });
    });
    $(document).on('click', '#btnProcess', function (e) {
        e.preventDefault();
        console.log('starting---------->>>>>');
        var dt = $.trim($('#formGenerate #SetDate').val());
        var navtype = $.trim($('#formGenerate #navtype').val());
        var approverId = $.trim($('#formGenerate #ApproverId').val());
        //alert(rt);
        console.log(navtype);
        if (dt === '' || approverId === '' || navtype =='') {
            //loaderSpin2(true);
            displayDialogNoty('Validation Nofification', 'All Fields are Compulsory.');
            return;
        }
        var cnt = $('#tblNaps input[id*="chkSingle"]:checkbox:checked').size();
        if (cnt == 0) {
            displayDialogNoty('Notification', 'No Record is Selected for process');
            return;
        }
        var lst = new Array();
        $('#tblNaps input[id*="chkSingle"]:checkbox:checked').each(function (i, e) {
            
            lst.push($(this).attr('data-key'));
        });
        var reqType = 'A'; // $('ReqType').val();
        //var approvalId = $('#formGenerate #ApproverId').val();
       // console.log(JSON.stringify({ selected: lst, ReqType: reqType }));// JSON.stringify(data),)
        loaderSpin2(true);
        $.ajax({
            type: "POST",
            url: url + 'Process',
            contentType: "application/json",
            data: JSON.stringify({ selected: lst, ReqType: reqType, ApproverId: approverId, Navtype2: navtype }),// JSON.stringify(data),
            success: function (response) {
                loaderSpin2(false);
                // alert(response.RespMessage);
                if (response.RespCode == 0) {
                    //var msg = '<p>' + response.SucCount + ' Records Posted Successfully' + '</p>';
                    //msg += '<p>' + response.FailCount + ' Records Failed to Post' + '</p>';
                    //msg += '<p>Failed Transactions will remain on queue while Reason for failed Transaction can be viewed when you hover on each row</p>';
                    $('#divGrid tbody').empty();
                    $('#formGenerate #SetDate').val('');
                    $('#formGenerate #ApproverId').val('').trigger('change');
                    displayDialogNoty('Notification', response.RespMessage);
                  
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
    $(document).on('change','#tblNaps #chkAll', function () {
        if ($('#tblNaps #chkAll').is(':checked')) {
            $("#tblNaps :checkbox[name ='chkSingle']").prop("checked", true);
        }
        else {
            $("#tblNaps :checkbox[name ='chkSingle']").prop("checked", false);
        }
    });
    
   

    $(document).on('change', '#formGenerate #SetDate', function () {
        $('#divGrid tbody').empty();
    })

    $(document).on('click', '#tblNaps a.editor_edit', function (e) {
        e.preventDefault();
        $('#notyNaps').hide();
        var editLink = $(this).attr('data-key');
        // alert(editLink);
        var urlP = url + 'EditNaps/' + editLink ;

        console.log(urlP);
        loaderSpin2(true);
        $('#myModalNaps').load(
            urlP, function () {
                loaderSpin2(false);
                $('#myModalNaps').modal({ backdrop: 'static', keyboard: false });
                validateNapsForm();
            });
    });
    function validateNapsForm() {

            var validator = $("#formNaps").validate({
                rules: {
                    CREDITAMOUNT:{
                        required: true,
                        number:true,
                    },
                BENEFICIARYNAME: {
                    required: true,
                    
                },
                BENEFICIARYACCTNO: {
                    required: true,
                    number: true,
                    minlength: 10,
                    maxlength:10
                },
                BENEFICIARYBANKCODE: {
                    required: true,
                    number: true,
                    minlength: 3,
                    maxlength: 3
                },
                DEBITACCTNO: {
                    required: true,
                    number: true,
                    minlength: 10,
                    maxlength: 10
                },
                DEBITBANKCODE: {
                    required: true,
                    number: true,
                    minlength: 3,
                    maxlength: 3
                },
                REASON: "required",
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
                //var btn = $('#btnSave').val();
                var urlTemp;
                var postTemp;
                var event;
                urlTemp = url + 'EditNaps';
                postTemp = 'post';
                
                var $reqLogin = {
                    url: urlTemp,

                    data: $('#formNaps').serialize(),
                    type: postTemp,
                    contentType: "application/x-www-form-urlencoded"
                };
                console.log($('#formRvCode').serialize());
                //;
                loaderSpin(true);
                disableButton2(true, 'btnAddAcctGrid');
                var ajax = new Ajax();
                ajax.send($reqLogin).done(function (response) {
                    loaderSpin(false);
                    disableButton2(false, 'btnAddAcctGrid');
                    if (response.RespCode === 0) {
                        $('#myModalNaps').modal('hide');
                        $('#divGrid').html(response.data_html);
                        //console.log(JSON.stringify(response.data));
                        //console.log(JSON.stringify(response.data_msc));
                    }
                    else {
                        $('#notyNaps').show();
                        $('#notyNapsMsg').html(response.RespMessage);
                        // displayModalNoty('Notification', response.RespMessage);
                    }
                }).fail(function (xhr, status, err) {
                    loaderSpin(false);
                    disableButton2(false, 'btnAddAcctGrid');
                    displayModalNoty('Notification', 'No network connectivity. Please check your network connections.', true);
                });
            }
        });

    }

    $(document).on('click', '#btnUpload', function (e) {
        //alert('am clicked
        $("#tblNapsUpld tbody").empty();
        e.preventDefault();
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
            $('#btnProcessUpld').prop('disabled', true);

            var data = new FormData();
            for (var x = 0; x < files.length; x++) {
                data.append(files[x].name + x, files[x]);
            }
            // var reqType = $("input[name$='upldRequestType']:checked").val();
            // alert(reqType);
            //data.append('requestType', reqType);
       
            $.ajax({
                type: "POST",
                url: url + '/UploadFiles',
                contentType: false,
                processData: false,
                data: data,
                success: function (response) {
                    loaderSpin2(false);
                    if (response.RespCode == 0) {
                        $('#upldfile').val('');
                        //  alert(response.RespMessage);
                        // alert(response.data);
                        $('#divGridUpld').html(response.data_html);
                        //BindUploadList(response.data, reqType, '#' + response.BatchId);
                        $('#btnProcessUpld').removeAttr('disabled');
                    }
                    else {
                        displayDialogNoty('Notification', response.RespMessage);
                    }
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
  
    $(document).on('click', '#btnProcessUpld', function (e) {
        e.preventDefault();
        var approverId = $.trim($('#formUpload #ApproverId').val());
        var navtype = $.trim($('#formUpload #navtype3').val());
        //alert(approverId);
        if (approverId === '') {
            //loaderSpin2(true);
            displayDialogNoty('Validation Nofification', 'All Fields are Compulsory.');
            return;
        }
    var data = {};
    // data.AcctNo = '123456';
    //data.ApproverId = approverId;
   
    //console.log(JSON.stringify(data));
    //return;
    loaderSpin2(true);
    $.ajax({
        type: "POST",
        url: url + 'ProcessUpld',
        contentType: "application/json",
        //processData: false,
        data: JSON.stringify({ApproverId: approverId, Navtype2: navtype }),// JSON.stringify(data),
        success: function (response) {
            loaderSpin2(false);
            if (response.RespCode == 0) {
              
                $('#divGridUpld tbody').empty();
                $('#formUpload #ApproverId').val('').trigger('change');
                $('#formUpload #navtype3').val('').trigger('change');
                displayDialogNoty('Notification', response.RespMessage);

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
})