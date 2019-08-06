$(document).ready(function () {
    /// GetRoles(1);

    var col;
    //var table2 = $('.datatable').DataTable(
    //);
    // var cnt = 0;
    var menuId = $('#menuId').val();
    //alert(menuId);
    var url = BaseUrl() + 'SettlementRule/';
    var table2 = $('.datatable').DataTable({
        ajax: url + "SettlementRuleList",
        columns: [

            { data: "DESCRIPTION" },
            { data: "CHANNEL_DESC" },
            { data: "STATUS" },
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
            }
        ]
    });


    validateForm();
    function validateForm() {

        var validator = $("#formRule").validate({
            rules: {
                DESCRIPTION: "required",
                CHANNEL: "required",
               
            },
            messages: {

                // equalTo: "Password must be Equal",
                Name: {
                    required: "Please Enter Last Name"
                },
            },
            submitHandler: function () {
                console.log($('#formParty').serialize());
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

                //        data: $('#formParty').serialize(),
                //        type: postTemp,
                //        contentType: "application/x-www-form-urlencoded"
                //    };
                //    console.log($('#formParty').serialize());
                //    return;
                //    loaderSpin2(true);
                //    disableButton(true);
                //    var ajax = new Ajax();
                //    ajax.send($reqLogin).done(function (response) {
                //        loaderSpin2(false);
                //        disableButton(false);
                //        if (response.RespCode === 0) {
                //            //new Util().clearform('#formParty');

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

    $(document).on('click', '#btnAddAcct', function () {
        $('#notyMsc').hide();
        var menuId = $('#m').val();
        var urlP = url + 'AddAcct?m=' + menuId;
        console.log(urlP);
        loaderSpin2(true);
        $('#myModalAcct').load(
            urlP, function () {
                loaderSpin2(false);
                $('#myModalAcct').modal({ backdrop: 'static', keyboard: false });
                validateAcctForm();

            });

    });
    $(document).on('click', '#tableAcct a#edit', function (e) {
        e.preventDefault();
        $('#notyMsc').hide();
        var menuId = $('#m').val();
        // alert(menuId);
        var editLink = $(this).attr('data-key');
       // alert(editLink);
        var urlP = url + 'EditAcct/' + editLink + '?m=' + encodeURIComponent(menuId);
       
        //console.log(urlP);
        loaderSpin2(true);
        $('#myModalAcct').load(
            urlP, function () {
                loaderSpin2(false);
                $('#myModalAcct').modal({ backdrop: 'static', keyboard: false });
                validateAcctForm();
            });
    });
    $(document).on('click', '#tableAcct a#delete', function (e) {
        e.preventDefault();
        if (confirm('Are you sure you want to delete record?')) {
            //$('#notyMsc').hide();
            var menuId = $('#m').val();
            // alert(menuId);
            var editLink = $(this).attr('data-key');
            // alert(editLink);
            var urlP = url + 'DeleteAcct/' + editLink + '?m=' + encodeURIComponent(menuId);
            //var acq = $('#CBN_CODE').val();

            //console.log(urlP);
            loaderSpin2(true);
            $.get(
                urlP, function (response) {
                    loaderSpin2(false);
                    if (response.RespCode == 0) {
                        //console.log(response.data_msc);
                        $('#divAcct').html(response.data_html);
                    }
                    else {
                        displayDialogNoty('Notification', response.RespMessage);
                    }
                });
        }
    });
    $(document).on('click', '#tableAcct a#undo', function (e) {
        e.preventDefault();
        //if (confirm('Are you sure you want to delete record?')) {
        //$('#notyMsc').hide();
        var menuId = $('#m').val();
        // alert(menuId);
        var editLink = $(this).attr('data-key');
        // alert(editLink);
        var urlP = url + 'UndoAcct/' + editLink + '?m=' + encodeURIComponent(menuId);
        //var acq = $('#CBN_CODE').val();

        //console.log(urlP);
        loaderSpin2(true);
        $.get(
            urlP, function (response) {
                loaderSpin2(false);
                if (response.RespCode == 0) {
                    //console.log(response.data_msc);
                    $('#divAcct').html(response.data_html);
                }
                else {
                    displayDialogNoty('Notification', response.RespMessage);
                }
            });
        //}
    });
    function validateAcctForm() {

        var validator = $("#formRuleOption").validate({
            rules: {
                PARTYTYPE_CODE: "required",
                PARTYTYPE_VALUE: {
                    required:true,
                    number:true},
                PARTYTYPE_CAP: {
                    //required: true,
                    number: true
                },
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
                var btn = $('#btnSave').val();
                var urlTemp;
                var postTemp;
                var event;
                urlTemp = url + 'AddAcct';
                postTemp = 'post';
                //if (btn == "1") {

                //    urlTemp = url + 'Create';
                //    postTemp = 'post';
                //    $('#RoleId').val(0);
                //    event = 'new';
                //}
                //else {

                //    urlTemp = url + 'Create';
                //    //postTemp = 'put';
                //    postTemp = 'post';
                //    event = 'modify';
                //};
                //  alert(urlTemp);
                var $reqLogin = {
                    url: urlTemp,

                    data: $('#formRuleOption').serialize(),
                    type: postTemp,
                    contentType: "application/x-www-form-urlencoded"
                };
                console.log($reqLogin.url);
                //return;
                loaderSpin(true);
                disableButton2(true, 'btnAddAcctGrid');
                var ajax = new Ajax();
                ajax.send($reqLogin).done(function (response) {
                    loaderSpin(false);
                    disableButton2(false, 'btnAddAcctGrid');
                    if (response.RespCode === 0) {

                        $('#myModalAcct').modal('hide');
                        $('#divAcct').html(response.data_html);
                        //console.log(JSON.stringify(response.data));
                        //console.log(JSON.stringify(response.data_msc));
                    }
                    else {
                        $('#notyMsc').show();
                        $('#notyMscMsg').html(response.RespMessage);
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

    //$(document).on('change', '#PARTYTYPE_CODE', function () {
    //    var selected = $(this).val();;

    //    var urlP = url + 'GetIfCodeRequired/' + selected;
    //    console.log(urlP)
    //    loaderSpin2(true);
    //    $.get(
    //        urlP, function (response) {
    //            loaderSpin2(false);
    //            if (response.RespCode == 0) {
    //                //console.log(response.CodeRequired);
    //                if (response.CodeRequired) {
    //                    $('#pCode').css('display', 'block');
    //                    $('#PARTYCODEREQUIRED').val('Y');
    //                }
    //                else {
    //                    $('#pCode').css('display', 'none');
    //                    $('#PARTYCODEREQUIRED').val('N');
    //                }
    //            }
    //            else {
    //                //displayDialogNoty('Notification', response.RespMessage);
    //            }
    //        });

    //});
    
    //$(document).on('change', "#SET_INTL", function () {
    //    //alert('checked');
    //    if ($("#SET_INTL").is(":checked")) {
    //        $('.intlmsc').css('display', 'block');
    //    }
    //    else {
    //        $('.intlmsc').css('display', 'none');
    //    }
    //});
});