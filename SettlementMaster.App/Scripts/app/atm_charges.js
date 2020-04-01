$(document).ready(function () {
    /// GetRoles(1);

    var col;
    //var table2 = $('.datatable').DataTable(
    //);
    // var cnt = 0;
    var menuId = $('#menuId').val();
    //alert(menuId);
    var url = BaseUrl() + 'ATM_Charges/';
    //var table2 = $('.datatable').DataTable({
    //    ajax: url + "MCCList",
    //    columns: [

    //        { data: "MCC_CODE" },
    //        { data: "MCC_DESC" },
    //        { data: "SECTOR_NAME" },
    //        { data: "STATUS" },

    //        {
    //            data: null,
    //            className: "center_column",
    //            //defaultContent: '<a class="btn btn-primary btn-xs editor_edit" data-key="' + data.RoleId + '"><i class="fa fa-edit"></i></a>' +
    //            //    '  <a class="btn btn-warning btn-xs editor_remove" data-key="' + data.RoleId + '"><i class="fa fa-trash-o"></i></a>',
    //            render: function (data, type, row) {
    //                // Combine the first and last names into a single table field
                  
    //                var encodedUrl = url + 'Add/' + data.ITBID + '?m=' + encodeURIComponent(menuId);
    //                //alert(encodedUrl);
    //                var html = '<a class="btn btn-primary btn-xs editor_edit" href="' + encodedUrl + '"><i class="fa fa-edit"></i></a>';
    //                return html;
    //            }
    //        }
    //    ]
    //});


    validateForm();
    function validateForm() {

        var validator = $("#formATMCharges").validate({
            rules: {
                //MCC_CODE: "required",
                //MCC_DESC: "required",
                //SECTOR_CODE: "required"
            },
            messages: {

                // equalTo: "Password must be Equal",
                Name: {
                    required: "Please Enter Last Name"
                },
            },
            submitHandler: function () {
                    console.log($('#formMCC').serialize());
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

            //        data: $('#formMCC').serialize(),
            //        type: postTemp,
            //        contentType: "application/x-www-form-urlencoded"
            //    };
            //    console.log($('#formMCC').serialize());
            //    return;
            //    loaderSpin2(true);
            //    disableButton(true);
            //    var ajax = new Ajax();
            //    ajax.send($reqLogin).done(function (response) {
            //        loaderSpin2(false);
            //        disableButton(false);
            //        if (response.RespCode === 0) {
            //            //new Util().clearform('#formMCC');

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



   // $("#example1").on("click", "a.editor_edit", editDetailServer);

    //$("#example1").on("click", ".current", setAsCurrent);

    //function editDetailServer() {
    //    loaderSpin2(true);
    //    //  disableButton(true);
    //    var editLink = $(this).attr('data-key');
    //    col = $(this).parent();
    //    // alert(editLink);
    //    $('#RoleId').val(editLink);
    //    var menuId = $('#menuId').val();
    //    // alert(menuId);
    //    //  alert( $('#RoleId').val());
    //    var $reqLogin = {
    //        url: url + 'ViewDetail/' + editLink + '?m=' + menuId,

    //        data: null,
    //        type: "Get",
    //        contentType: "application/json"
    //    };
    //    // alert($reqLogin.url);
    //    loaderSpin2(true);
    //    $('.panel-form').load(
    //      $reqLogin.url, function () {
    //          loaderSpin2(false);
    //          // $('#myModal').modal({ backdrop: 'static', keyboard: false });
    //          $('#pnlForm').fadeIn();
    //          $('#pnlGrid').fadeOut();
    //          // validator.resetForm();
    //          validateForm();
    //          $('a.editor_create').hide();
    //          $('a.editor_return').show();
    //      });
    //    //$('#ajax-loading').show();
    //    //////var ajax = new Ajax();
    //    //////ajax.send($reqLogin).done(function (response) {
    //    //////    loaderSpin2(false);
    //    //////    if (response.RespCode === 0) {

    //    //////        $('#RoleName').val(response.model.RoleName);
    //    //////        //alert(response.model.RoleId);
    //    //////        $('#RoleId').val(response.model.RoleId);

    //    //////        //  $('#UserName').attr('disabled','disabled');
    //    //////        $('#CreatedBy').text(response.model.CreatedBy);
    //    //////        $('#CreatedDate').text(response.model.DateString);
    //    //////        //  $('#Status').text(response.model.Status);
    //    //////        $('#pnlAudit').css('display', 'block');
    //    //////        $('a.editor_reset').hide();
    //    //////        // $('').val(response.ExamName);
    //    //////        $('#btnSave').html('<i class="fa fa-edit"></i> Update');
    //    //////        $('#btnSave').val(2);

    //    //////        $('#myModal').modal({ backdrop: 'static', keyboard: false });
    //    //////    }
    //    //////    else {
    //    //////        alert(response.RespMessage + 'edit error');
    //    //////    }
    //    //////}).fail(function (xhr, status, err) {
    //    //////    loaderSpin2(false);
    //    //////    $('#ajax-loading').hide();
    //    //////    $('#display-error').show();
    //    //////    $('#errorMessage').text("No network connectivity. Please check your network connections.");
    //    //////    // errorMessage
    //    //////    // showPopDialog("No network connectivity. Please check your network connections.", handlerError);
    //    //////});

    //}
    //$(document).on('click', 'a.editor_create', function () {
    //    var menuId = $('#menuId').val();
    //    // alert(menuId);
    //    $('a.editor_reset').show();
    //    //$('#divStatus').hide();
    //    var urlP = url + 'ViewDetail?m=' + menuId;
    //    //alert(urlP);
    //    loaderSpin2(true);
    //    $('.panel-form').load(
    //        urlP, function () {
    //            loaderSpin2(false);
    //            // $('#myModal').modal({ backdrop: 'static', keyboard: false });
    //            $('#pnlForm').fadeIn();
    //            $('#pnlGrid').fadeOut();
    //            // validator.resetForm();
    //            validateForm();
    //            $('a.editor_create').hide();
    //            $('a.editor_return').show();
    //        });

    //});
    //$(document).on('click', 'a.editor_reset', function () {
    //    // formClear('#FormTerm');
    //    new Util().clearform('#formUsers');

    //    //validator.resetForm();
    //    $('#pnlAudit').css('display', 'none');
    //    $('#ItbId').val(0);

    //});
    //$(document).on('click', 'a.editor_return', function () {
    //    $('a.editor_return').hide();
    //    $('a.editor_create').show();
    //    $('#pnlForm').fadeOut();
    //    $('#pnlGrid').fadeIn();
    //    // alert('am clicked');
    //});
    $(document).on('click', '#btnAddCharges', function () {
        $('#notyMsc').hide();
        var menuId = $('#menuId').val();
        var urlP = url + 'AddCharges?m=' + menuId;
        //var acq = $('#CBN_CODE').val();
        //alert(acq);
        //if (acq == '')
        //{
        //    $('#CBN_CODE').focus();
        //    displayDialogNoty('Notification', 'Please Select an Acquirer before adding MSC');
        //    return;
        //}
        //console.log(urlP);
        loaderSpin2(true);
        $('#myModalCharges').load(
            urlP, function () {
                loaderSpin2(false);
                $('#myModalCharges').modal({ backdrop: 'static', keyboard: false });
                validateChargesForm();
               
            });
        
    });
    $(document).on('click', '#tableCharges a#edit', function (e) {
        e.preventDefault();
        $('#notyMsc').hide();
        var menuId = $('#m').val();
       // alert(menuId);
        var editLink = $(this).attr('data-key');
       // alert(editLink);
        var urlP = url + 'EditATMCharges/' +editLink + '?m=' + encodeURIComponent(menuId);
        //var acq = $('#CBN_CODE').val();

        //console.log(urlP);
        loaderSpin2(true);
        $('#myModalCharges').load(
            urlP, function () {
                loaderSpin2(false);
                $('#myModalCharges').modal({ backdrop: 'static', keyboard: false });
                validateChargesForm();
            });
    });
    $(document).on('click', '#tableCharges a#delete', function (e) {
        e.preventDefault();
        if (confirm('Are you sure you want to delete record?')) {
            //$('#notyMsc').hide();
            var menuId = $('#m').val();
            // alert(menuId);
            var editLink = $(this).attr('data-key');
            // alert(editLink);
            var urlP = url + 'DeleteATMCharges/' + editLink + '?m=' + encodeURIComponent(menuId);
            //var acq = $('#CBN_CODE').val();

            //console.log(urlP);
            loaderSpin2(true);
            $.get(
                urlP, function (response) {
                    loaderSpin2(false);
                    if (response.RespCode == 0) {
                        console.log(response.data_msc);
                        $('#divAcqMsc').html(response.data_msc);
                    }
                    else {
                        displayDialogNoty('Notification', response.RespMessage);
                    }
                });
        }
    });
    $(document).on('click', '#tableCharges a#undo', function (e) {
        e.preventDefault();
        //if (confirm('Are you sure you want to delete record?')) {
            //$('#notyMsc').hide();
            var menuId = $('#m').val();
            // alert(menuId);
            var editLink = $(this).attr('data-key');
            // alert(editLink);
            var urlP = url + 'UndoMsc/' + editLink + '?m=' + encodeURIComponent(menuId);
            //var acq = $('#CBN_CODE').val();

            //console.log(urlP);
            loaderSpin2(true);
            $.get(
                urlP, function (response) {
                    loaderSpin2(false);
                    if (response.RespCode == 0) {
                        console.log(response.data_msc);
                        $('#divAcqMsc').html(response.data_msc);
                    }
                    else {
                        displayDialogNoty('Notification', response.RespMessage);
                    }
                });
        //}
    });
    function validateChargesForm() {

        var validator = $("#formCharges").validate({
            rules: {
                REQUESTTYPE_CODE: "required",
                TRAN_CODE: "required",
                OPERATORTYPE: { required: true },
                CALCBASIS_ITBID: "required",
                PartyValue: { required: true }
            },
            messages: {

                // equalTo: "Password must be Equal",
                Name: {
                    required: "Please Enter Last Name"
                },


            },
            submitHandler: function () {
                //  e.preventDefault();
                 //alert('success');
                var btn = $('#btnSave').val();
                var urlTemp;
                var postTemp;
                var event;
                urlTemp = url + 'AddCharges';
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

                    data: $('#formCharges').serialize(),
                    type: postTemp,
                    contentType: "application/x-www-form-urlencoded"
                };
                console.log($reqLogin.url);
                //return;
                loaderSpin(true);
                disableButton2(true, 'btnAddChargesGrid');
                var ajax = new Ajax();
                ajax.send($reqLogin).done(function (response) {
                    loaderSpin(false);
                    disableButton2(false, 'btnAddChargesGrid');
                    if (response.RespCode === 0) {
                       
                        $('#myModalCharges').modal('hide');
                        $('#divAcqMsc').html(response.data_msc);
                        //console.log(JSON.stringify(response.data));
                        console.log(JSON.stringify(response.data_msc));
                    }
                    else {
                        $('#notyMsc').show();
                        $('#notyMscMsg').html(response.RespMessage);
                       // displayModalNoty('Notification', response.RespMessage);
                    }


                }).fail(function (xhr, status, err) {
                    loaderSpin(false);
                    disableButton2(false, 'btnAddChargesGrid');
                    displayModalNoty('Notification', 'No network connectivity. Please check your network connections.', true);
                });


            }
        });

    }


    var partyValueNumeric = {
        PartyValue:
        {
            required: true,
            //number: 50
        },
    };
    var partyValueCustom = {
        PartyValue:
        {
            required: true,
            number: true
        },
    };
    function addPartyRules() {
        //$('#' + $id).rules('add', rule);
        $('#PartyValue').rules('add', {
            number: true   // overwrite an existing rule
        });
    }

    function removePartyRules() {
        //  $('#' + $id).rules('remove');
        $('#PartyValue').rules('remove', 'number');
    }

    $(document).on('change', '#CALCBASIS_ITBID', function () {
        var $id = $(this).attr('id');
       // alert($id);
        if ($(this).val() === '3') {
            //alert('here custom');
            removePartyRules();
        } else {
           // alert('here nicht custom');
            addPartyRules();
        }
    })
    $(document).on('change', '#CBN_CODE', function () {
        var selected = $(this).val();
        var mcc_code = $('#MCC_CODE').val();
        //$('#formMCC #CBN_CODE').val(selected);
       
            var urlP = url + 'GetAcquirerMsc?mcc=' + mcc_code + '&cbn=' + selected;
            console.log(urlP)
            loaderSpin2(true);
            $.get(
                urlP, function (response) {
                    loaderSpin2(false);
                    if (response.RespCode == 0) {
                        console.log(response.data_msc);
                        $('#divAcqMsc').html(response.data_msc);
                    }
                    else {
                        $('#divAcqMsc').html('');
                        displayDialogNoty('Notification', response.RespMessage);
                    }
                });
       
    });
    $(document).on('change',"#SET_INTL",function () {
        //alert('checked');
        if ($("#SET_INTL").is(":checked")) {
            $('.intlmsc').css('display', 'block');
        }
        else {
            $('.intlmsc').css('display', 'none');
        }
    });
    $(document).on('change', '#OPERATORTYPE', function () {
        //alert('HERE');
        if ($(this).val() !== '') {
            $('#OPERATORTYPE_DESC').val($("#OPERATORTYPE option:selected").text());
        }
    })
});