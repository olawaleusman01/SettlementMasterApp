$(document).ready(function () {
    /// GetRoles(1);

    var col;
    var menuId = $('#menuId').val();
    //alert(menuId);
    var url = BaseUrl() + 'SetReconciliation/';
    var table2 = $('.datatable').DataTable({
        ajax: url + "SetReconList",
        columns: [
            {
                data: null,
                className: "center_column",
                //defaultContent: '<a class="btn btn-primary btn-xs editor_edit" data-key="' + data.RoleId + '"><i class="fa fa-edit"></i></a>' +
                //    '  <a class="btn btn-warning btn-xs editor_remove" data-key="' + data.RoleId + '"><i class="fa fa-trash-o"></i></a>',
                render: function (data, type, row) {
                    // Combine the first and last names into a single table field

                    var encodedUrl = url + 'Add/' + data.ITBID + '?m=' + encodeURIComponent(menuId);
                    //alert(encodedUrl);
                    var html = '<a class="btn btn-primary btn-xs editor_edit" href="' + encodedUrl + '"><i class="fa fa-edit"></i></a>';
                    return html;
                }
            },
            { data: "CBNCODE" },
            { data: "INSTITUTION_NAME" },
            { data: "CARDSCHEME" },
            { data: "BIN" },

        ]
    });


    validateForm();
    function validateForm() {

        var validator = $("#formSetRecon").validate({
            rules: {
                CARDSCHEME: "required",
                CBNCODE: "required",
                BIN: {
                    required: true,
                    number: true,
                }
            },
            messages: {

                // equalTo: "Password must be Equal",
                Name: {
                    required: "Please Enter Last Name"
                },
            },
            submitHandler: function () {
                //console.log($('#formCardscheme').serialize());
                //return;
                loaderSpin2(true);
                form.submit();
                //  e.preventDefault();
                // alert('success');
                //    var btn = $('#btnSave').val();
                //    var urlTemp;
                //    var postTemp;
                //    var event;
                //    if (btn == "1") {

                //        urlTemp = url + 'Add';
                //        postTemp = 'post';
                //        $('#ITBID').val(0);
                //        event = 'new';
                //    }
                //    else {
                //        urlTemp = url + 'Add';
                //        postTemp = 'post';
                //        event = 'modify';
                //    };
                //    //  alert(urlTemp);
                //    var $reqLogin = {
                //        url: urlTemp,

                //        data: $('#formCardscheme').serialize(),
                //        type: postTemp,
                //        contentType: "application/x-www-form-urlencoded"
                //    };
                //    console.log($('#formCardscheme').serialize());
                //    return;
                //    loaderSpin2(true);
                //    disableButton(true);
                //    var ajax = new Ajax();
                //    ajax.send($reqLogin).done(function (response) {
                //        loaderSpin2(false);
                //        disableButton(false);
                //        if (response.RespCode === 0) {
                //            //new Util().clearform('#formCardscheme');

                //            //$('a.editor_return').click();
                //            displayDialogNoty('Notification', response.RespMessage);
                //        }
                //        else {
                //            // alert(response.RespMessage)
                //            // $('a.editor_return').click();
                //            displayDialogNoty('Notification', response.RespMessage);

                //        }


                //    }).fail(function (xhr, status, err) {
                //        loaderSpin2(false);
                //        disableButton(false);
                //        displayDialogNoty('Notification', 'No network connectivity. Please check your network connections.', true);
                //    });
            }
        });
    }
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
            $('#btnValidate, #btnDownload').prop('disabled', true);

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
                        //alert('pop');
                    var msg = '<p> Upload Process Completed</p>';
                        displayDialogNoty('Notification', msg);
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
            url: url + 'Validate',
            contentType: "application/json",
            data: null, // JSON.stringify({ INSTITUTION_ID: instId }),// JSON.stringify(data),
            success: function (response) {
                loaderSpin2(false);
                if (response.RespCode == 0) {
                    var msg = '<p> Reconcillation Process Completed</p>';
                    //msg += '<p>' + response.FailCount + ' Records Failed Validation' + '</p>';
                    displayDialogNoty('Notification', msg);
                    $('#btnDownload').removeAttr('disabled');
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
            url: url + 'Process',
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