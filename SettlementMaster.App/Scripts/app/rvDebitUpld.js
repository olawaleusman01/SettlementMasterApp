$(document).ready(function () {
    /// GetRoles(1);

    var col;
    var menuId = $('#menuId').val();
    //alert(menuId);
    var url = BaseUrl() + 'RevenueHead/';
   
    $(document).on('click', '#upload #btnUpload', function (e) {
        //alert('am clicked
        $("#tblUpld tbody").empty();
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
                url: url + '/UploadRvDrAcctFiles',
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
    $(document).on('click', '#upload #btnValidate', function (e) {
        e.preventDefault();
        loaderSpin2(true);
        $.ajax({
            type: "POST",
            url: url + 'ValidateRvDrAcct',
            contentType: "application/json",
            data:null, // JSON.stringify({ INSTITUTION_ID: instId }),// JSON.stringify(data),
            success: function (response) {
                loaderSpin2(false);
                if (response.RespCode == 0) {
                    var msg = '<p>' + response.SucCount + ' Records Successfully Validated ' + '</p>';
                    msg += '<p>' + response.FailCount + ' Records Failed Validation' + '</p>';
                    displayDialogNoty('Notification', msg);
                    if (response.SucCount > 0) {
                        $('#btnSave').removeAttr('disabled');
                    }
                    else {
                        $('#btnSave').prop('disabled', 'disabled');
                    }
                    if (response.data_html) {
                        $('#divGrid').html(response.data_html);
                        return;
                    }
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
    $(document).on('click', '#upload #btnSave', function (e) {
        e.preventDefault();
        var data = {};
        loaderSpin2(true);
        $.ajax({
            type: "POST",
            url: url + 'ProcessRvDrAcct',
            contentType: "application/json",
            //processData: false,
            data: JSON.stringify(data),
            success: function (response) {
                loaderSpin2(false);
                if (response.RespCode == 0) {
                    $("#tblUpld tbody").empty();
                    $('#btnSave').prop('disabled', 'disabled');
                    $('#btnValidate').prop('disabled', 'disabled');
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

    $(document).on('click', '#upload #btnClear', function (e) {
        e.preventDefault();
        $('#tblUpld tbody').empty();
        $('#btnSave').prop('disabled', 'disabled');
        $('#btnValidate').prop('disabled', 'disabled');
    })


});