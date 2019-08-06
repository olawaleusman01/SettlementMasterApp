var url = BaseUrl() + 'MerchantUpdate/';
$(document).on('click', '#btnUpload', function (e) {
    //alert('am clicked
    $("#example1 tbody").empty();
    e.preventDefault();
    loaderSpin2(true);
    if (window.FormData !== undefined) {
        var fileUpload = $('#upldfile').get(0);
        var files = fileUpload.files;
        //alert(files.length);
        if (files.length <= 0) {
            loaderSpin2(false);
            alert("Please select the file you are to upload!");
            return;
        }
        $('#btnValidate, #btnSave').prop('disabled', true);
        
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
                    $('#divGrid').html(response.data_html);
                    //BindUploadList(response.data, reqType, '#' + response.BatchId);
                    $('#btnValidate').removeAttr('disabled');
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
$(document).on('click','#btnValidate', function (e) {
    e.preventDefault();
   
    //var data = new FormData();
    //var bid = $("#lblBatchId").text();
    //alert('lol');
    var instId = $("#INSTITUTION_ID").val();
    //alert(instId);
    if (instId == '')
    {
        displayDialogNoty('Notification', 'Please you must select a bank before you validate');
        return;
    }
    loaderSpin2(true);
    $('#hidInstId').val(instId);
    //alert(reprocess);
    //data.append('INSTITUTION_ID', instId);
    //if (reprocess == '1') {
    //    data.append('Reprocess', true);
    //}
    $.ajax({
        type: "POST",
        url: url + 'Validate',
        contentType: "application/json",
        data: JSON.stringify({ INSTITUTION_ID: instId}),// JSON.stringify(data),
        success: function (response) {
            loaderSpin2(false);
            if (response.RespCode == 0) {
                var msg = '<p>' + response.SucCount + ' Records Successfully Validated ' + '</p>';
                msg += '<p>' + response.FailCount + ' Records Failed Validation' + '</p>';
                displayDialogNoty('Notification', msg);
                if (response.SucCount > 0)
                {
                    $('#btnSave').removeAttr('disabled');
                }
                else {
                    $('#btnSave').prop('disabled','disabled');
                }
                //$("#example1 tbody").empty();
                if (response.data_html) {
                    $('#divGrid').html(response.data_html);

                    // msg += '<p>Failed Transactions will remain on queue while Reason for failed Transaction can be viewed when you hover on each row</p>';

                    return;
                }
                //$("#hidReprocess").val('0');
                //var msg = '<p>' + response.SucCount + ' Records Posted Successfully' + '</p>';
                //msg += '<p>' + response.FailCount + ' Records Failed to Post' + '</p>';
                // msg += '<p>Failed Transactions will remain on queue while Reason for failed Transaction can be viewed when you hover on each row</p>';

                //  displayDialogNoty('Notification', response.RespMessage);
                //alert(response.RespMessage);
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

});
$(document).on('click', '#btnSave', function (e) {
    e.preventDefault();
    var data = {};
    // data.AcctNo = '123456';
    data.INSTITUTION_ID = $('#INSTITUTION_ID').val();
    data.VALIDATED_INST_ID = $('#hidInstId').val();
    if (data.INSTITUTION_ID == '') {
        displayDialogNoty('Notification', 'Please you must select a bank before you validate');
        return;
    }
    if (data.INSTITUTION_ID != data.VALIDATED_INST_ID) {
        displayDialogNoty('Notification', 'Institution selected when validating is different from the selected institution.Please Select the institution or revaildate before saving the record.');
        return;
    }
    //console.log(JSON.stringify(data));
    //return;
    loaderSpin2(true);
    $.ajax({
        type: "POST",
        url: url + 'Process',
        contentType: "application/json",
        //processData: false,
        data: JSON.stringify(data),
        success: function (response) {
            loaderSpin2(false);
            if (response.RespCode == 0) {
                $("#example1 tbody").empty();
                $('#INSTITUTION_ID').val('').trigger('change');
                var msg = response.RespMessage;
                displayDialogNoty('Notification', msg);
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

$(document).on('change', '#INSTITUTION_ID', function () {
    $('#btnSave').prop('disabled', 'disabled');
})
