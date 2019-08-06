$(document).ready(function () {
    var url = BaseUrl() + 'ApprovalRoute/';
    validateForm();
    function validateForm() {

        var validator = $("#formAppRoute").validate({
            rules: {
                NoLevel: "required",
                MenuId: "required",
                //SECTOR_CODE: "required"
            },
            messages: {

                // equalTo: "Password must be Equal",
                Name: {
                    required: "Please Enter Last Name"
                },
            },
            submitHandler: function () {
                //console.log($('#formAppRoute').serialize());
                //return;
                //loaderSpin2(true);
                //form.submit();
                    var urlTemp;
                    var postTemp;
                    var event;

                        urlTemp = url + 'Add';
                        postTemp = 'post';
                   
                    //  alert(urlTemp);
                    var $reqLogin = {
                        url: urlTemp,
                        data: $('#formAppRoute').serialize(),
                        type: postTemp,
                        contentType: "application/x-www-form-urlencoded"
                    };
                   // console.log($('#formAppRoute').serialize());
                   // return;
                    loaderSpin2(true);
                    disableButton(true);
                    var ajax = new Ajax();
                    ajax.send($reqLogin).done(function (response) {
                        loaderSpin2(false);
                        disableButton(false);
                        if (response.RespCode === 0) {
                            //new Util().clearform('#formAppRoute');

                           clearForm();
                            displayDialogNoty('Notification', response.RespMessage);
                        }
                        else {
                            // alert(response.RespMessage)
                            // $('a.editor_return').click();
                            displayDialogNoty('Notification', response.RespMessage);

                        }

                    }).fail(function (xhr, status, err) {
                        loaderSpin2(false);
                        disableButton(false);
                        displayDialogNoty('Notification', 'No network connectivity. Please check your network connections.', true);
                    });
            }
        });
    }
    $(document).on('change', '#MenuId', function () {
        var selected = $(this).val();
        $('#tblRoute tbody').empty();
        $('#tblRouteLocal tbody').empty();
        $('#NoLevel').val('');
        //alert(selected);
        if (selected != '') {
            var urlString = url + 'ApproverList/' + selected
            //console.log(urlString);
            loaderSpin2(true);
            $.get(urlString, function (response) {
                loaderSpin2(false);
                if (response.RespCode === 0) {
                    $('#NoLevel').val(response.data_level);
                    $('#divAccta').html(response.data_html);
                }
                else {
                    displayDialogNoty('Notification', response.RespMessage);
                }
            });
        }
        else {
            $('#btnAddRoute').hide();
        }
    })
    
    $(document).on('click', '#tblRouteLocal a.editor_delete', function (e) {
        e.preventDefault();
        $('#notyMsc').hide();
        var menuId = $('#m').val();
        // alert(menuId);
        var editLink = $(this).attr('data-key');
        // alert(editLink);
        var urlP = url + 'DeleteRouteOfficerLocal/' + editLink;

        if (confirm('Are you sure you want to delete record?')) {
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
    $(document).on('click', '#btnAddRoute', function () {
        $('#notyMsc').hide();
        var menuid = $('#MenuId').val();
        var urlP = url + 'AddRouteOfficer?menuid=' + menuid;
         //console.log(urlP);
        loaderSpin2(true);
        $('#myModalRoute').load(
            urlP, function () {
                loaderSpin2(false);
                $('#myModalRoute').modal({ backdrop: 'static', keyboard: false });
                validateRouteForm();

            });

    });
    $(document).on('click', '#tblRoute a#edit', function (e) {
        e.preventDefault();
        $('#notyMsc').hide();
        var menuId = $('#m').val();
        // alert(menuId);
        var editLink = $(this).attr('data-key');
        // alert(editLink);
        var urlP = url + 'EditRouteOfficer/' + editLink + '?m=' + encodeURIComponent(menuId);

        //console.log(urlP);
        loaderSpin2(true);
        $('#myModalRoute').load(
            urlP, function () {
                loaderSpin2(false);
                $('#myModalRoute').modal({ backdrop: 'static', keyboard: false });
                validateAcctForm();
            });
    });
    $(document).on('click', '#tblRoute a#delete', function (e) {
        e.preventDefault();
        if (confirm('Are you sure you want to delete record?')) {
            //$('#notyMsc').hide();
            var menuId = $('#m').val();
            // alert(menuId);
            var editLink = $(this).attr('data-key');
            // alert(editLink);
            var urlP = url + 'DeleteRouteOfficer/' + editLink + '?m=' + encodeURIComponent(menuId);
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

    function validateRouteForm() {

        var validator = $("#formRouteOfficer").validate({
            rules: {
                APPROVER_ID: "required",
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
                urlTemp = url + 'AddRouteOfficer';
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

                    data: $('#formRouteOfficer').serialize(),
                    type: postTemp,
                    contentType: "application/x-www-form-urlencoded"
                };
                //console.log($('#formRvCode').serialize());
                //;
                loaderSpin(true);
                disableButton2(true, 'btnAddAcctGrid');
                var ajax = new Ajax();
                ajax.send($reqLogin).done(function (response) {
                    loaderSpin(false);
                    disableButton2(false, 'btnAddAcctGrid');
                    if (response.RespCode === 0) {
                        $('#myModalRoute').modal('hide');
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
    function clearForm() {
        $('#NoLevel').val('');
        $('#MenuId').val('').trigger('change');
        $('#tblRoute tbody').empty();
        $('#tblRouteLocal tbody').empty();
    }
})