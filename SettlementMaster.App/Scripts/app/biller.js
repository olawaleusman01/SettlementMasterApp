$(document).ready(function () {
    /// GetRoles(1);

    var col;
    var menuId = $('#menuId').val();
    //alert(menuId);
    var url = BaseUrl() + 'Biller/';
    var table2 = $('.datatable').DataTable({
        ajax: url + "BillerList",
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
            { data: "BILLER_CODE" },
            { data: "BILLER_DESC" },
            { data: "CHANNEL_DESC" },
            { data: "MERCHANTNAME" },
            { data: "STATUS" }
            
        ]
    });


    validateForm();
    function validateForm() {

        var validator = $("#formBiller").validate({
            rules: {
                "mObj.BILLER_CODE": {
                    required:true,
                },
                "mObj.BILLER_DESC": "required",
                "mObj.CHANNEL": "required",
                "mObj.MERCHANTID": "required",
                "mObj.COUNTRY_CODE": "required",
                
            },
            messages: {

                // equalTo: "Password must be Equal",
                Name: {
                    required: "Please Enter Last Name"
                },
            },
            submitHandler: function () {
                //console.log($('#formChannel').serialize());
                //return;
                var val_error = false;
                var calc_basis = $('#mBillerMscObj_FEE_CALCBASIS').val();
                var fee = $('#mBillerMscObj_FEE1').val();
                var mcalc_basis = $('#mBillerMscObj_DOM_MSC_CALCBASIS').val();
                var msc1 = $('#mBillerMscObj_DOM_MSC1').val();
                $('#mBillerMscObj_FEE_CALCBASIS').next().remove();
                $('#mBillerMscObj_DOM_MSC_CALCBASIS').next().remove();
                if (fee != '' && fee > 0) {
                    if (calc_basis === '') {
                        var label = '<label for="mBillerMscObj_FEE_CALCBASIS" class="required">This field is required.</label>';
                        $(label).insertAfter($('#mBillerMscObj_FEE_CALCBASIS'));
                        $('#mBillerMscObj_FEE_CALCBASIS').focus();
                        val_error = true;
                    }
                    if (val_error) {
                        return;
                    }
                }
                //alert(msc1);
                if (msc1 != '' && msc1 > 0) {
                    if (mcalc_basis === '') {
                        var label = '<label for="mBillerMscObj_DOM_MSC_CALCBASIS" class="required">This field is required.</label>';
                        $(label).insertAfter($('#mBillerMscObj_DOM_MSC_CALCBASIS'));
                        $('#mBillerMscObj_DOM_MSC_CALCBASIS').focus();
                        val_error = true;
                    }
                    if (val_error) {
                        return;
                    }
                }
                loaderSpin2(true);
                form.submit();
            }
        });
    }
    $("#example1").on("click", "a.editor_edit", editDetailServer);
    function editDetailServer() {
        loaderSpin2(true);
    }

    /*Fee Sharing Party*/
    $(document).on('click', '#tblFee a#addParty', function () {
        // alert('here');
        $('#notyParty').hide();
        var urlP = url + 'AddParty?opt=FEE1';
        console.log(urlP);
        loaderSpin2(true);
        $('#myModalParty').load(
            urlP, function () {
                loaderSpin2(false);
                $('#myModalParty').modal({ backdrop: 'static', keyboard: false });
                validatePartyForm();
            });

    });
    $(document).on('click', '#tblFee a#edit', function (e) {
        e.preventDefault();
        $('#notyParty').hide();
        // alert(menuId);
        var editLink = $(this).attr('data-key');
         //alert(editLink);
        var urlP = url + 'EditParty/' + editLink + '?opt=MSC1';

        console.log(urlP);
        loaderSpin2(true);
        $('#myModalParty').load(
            urlP, function () {
                loaderSpin2(false);
                $('#myModalParty').modal({ backdrop: 'static', keyboard: false });
                validatePartyForm();
            });
    });
    $(document).on('click', '#tblFee a#delete', function (e) {
        e.preventDefault();
        if (confirm('Are you sure you want to delete record?')) {

            var editLink = $(this).attr('data-key');
            // alert(editLink);
            var urlP = url + 'DeleteParty/' + editLink;
            //alert(urlP);
            loaderSpin2(true);
            $.get(
                urlP, function (response) {
                    loaderSpin2(false);
                    if (response.RespCode == 0) {
                        $('#divPaymentParty').html(response.data_html);
                        $('#mBillerMscObj_FEE1').val(response.data_fee);
                        //$('#DOM_MSC2CAP').val(response.data_cap2);
                        //SetTotalMscDom();
                        //SetTotalCapDom();
                        //SetTotalFee();
                    }
                    else {
                        displayDialogNoty('Notification', response.RespMessage);
                    }
                });
        }
    });
   
    function validatePartyForm() {

        var validator = $("#formParty").validate({
            rules: {
                PARTYTYPE_CODE: "required",
                SHARINGVALUE: {
                    required: true,
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
                var btn = $('#btnAddPartyGrid').val();
                var urlTemp;
                var postTemp;
                //$('#MCCMSC_ITBID').val($('#formMSC #ITBID').val());
                //alert($('#formMSC #ITBID').val());
                urlTemp = url + 'AddParty';
                postTemp = 'post';
                var $reqLogin = {
                    url: urlTemp,
                    data: $('#formParty').serialize(),
                    type: postTemp,
                    contentType: "application/x-www-form-urlencoded"
                };
                //console.log($('#formParty').serialize());
                //return;
                loaderSpin(true);
                disableButton2(true, 'btnAddPartyGrid');
                var ajax = new Ajax();
                ajax.send($reqLogin).done(function (response) {
                    loaderSpin(false);
                    disableButton2(false, 'btnAddPartyGrid');
                    if (response.RespCode === 0) {
                        console.log(response.data_html);
                        $('#myModalParty').modal('hide');
                        $('#divPaymentParty').html(response.data_html);
                        $('#mBillerMscObj_FEE1').val(response.data_fee);
                        $('#mscUpdated').val('Y');
                    }
                    else {
                        $('#notyParty').show();
                        $('#notyPartyMsg').html(response.RespMessage);
                        // displayModalNoty('Notification', response.RespMessage);
                    }


                }).fail(function (xhr, status, err) {
                    loaderSpin(false);
                    disableButton2(false, 'btnAddPartyGrid');
                    displayModalNoty('Notification', 'No network connectivity. Please check your network connections.', true);
                });
            }
        });
    }
    
    /*End Fee Sharing Party*/

    /*MSC1 Sharing Party*/
    $(document).on('click', '#tblMsc1 a#addParty', function () {
        // alert('here');
        $('#notyParty').hide();
        var urlP = url + 'AddParty?opt=MSC1';
        console.log(urlP);
        loaderSpin2(true);
        $('#myModalParty').load(
            urlP, function () {
                loaderSpin2(false);
                $('#myModalParty').modal({ backdrop: 'static', keyboard: false });
                validateMsc1Form();
            });

    });
    $(document).on('click', '#tblMsc1 a#edit', function (e) {
        e.preventDefault();
        $('#notyParty').hide();
        // alert(menuId);
        var editLink = $(this).attr('data-key');
        //alert(editLink);
        var urlP = url + 'EditParty/' + editLink + '?opt=MSC1';

        //console.log(urlP);
        loaderSpin2(true);
        $('#myModalParty').load(
            urlP, function () {
                loaderSpin2(false);
                $('#myModalParty').modal({ backdrop: 'static', keyboard: false });
                validateMsc1Form();
            });
    });
    $(document).on('click', '#tblMsc1 a#delete', function (e) {
        e.preventDefault();
        if (confirm('Are you sure you want to delete record?')) {

            var editLink = $(this).attr('data-key');
            // alert(editLink);
            var urlP = url + 'DeleteParty/' + editLink;
            //alert(urlP);
            loaderSpin2(true);
            $.get(
                urlP, function (response) {
                    loaderSpin2(false);
                    if (response.RespCode == 0) {
                        $('#divMsc1Party').html(response.data_html);
                        $('#mBillerMscObj_DOM_MSC1').val(response.data_fee);
                        //$('#DOM_MSC2CAP').val(response.data_cap2);
                        //SetTotalMscDom();
                        //SetTotalCapDom();
                        //SetTotalFee();
                    }
                    else {
                        displayDialogNoty('Notification', response.RespMessage);
                    }
                });
        }
    });

    function validateMsc1Form() {

        var validator = $("#formParty").validate({
            rules: {
                PARTYTYPE_CODE: "required",
                SHARINGVALUE: {
                    required: true,
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
                var btn = $('#btnAddPartyGrid').val();
                var urlTemp;
                var postTemp;
                //$('#MCCMSC_ITBID').val($('#formMSC #ITBID').val());
                //alert($('#formMSC #ITBID').val());
                urlTemp = url + 'AddParty';
                postTemp = 'post';
                var $reqLogin = {
                    url: urlTemp,
                    data: $('#formParty').serialize(),
                    type: postTemp,
                    contentType: "application/x-www-form-urlencoded"
                };
                //console.log($('#formParty').serialize());
                //return;
                loaderSpin(true);
                disableButton2(true, 'btnAddPartyGrid');
                var ajax = new Ajax();
                ajax.send($reqLogin).done(function (response) {
                    loaderSpin(false);
                    disableButton2(false, 'btnAddPartyGrid');
                    if (response.RespCode === 0) {
                        console.log(response.data_html);
                        $('#myModalParty').modal('hide');
                        $('#divMsc1Party').html(response.data_html);
                        $('#mBillerMscObj_DOM_MSC1').val(response.data_fee);
                        $('#mscUpdated').val('Y');
                    }
                    else {
                        $('#notyParty').show();
                        $('#notyPartyMsg').html(response.RespMessage);
                        // displayModalNoty('Notification', response.RespMessage);
                    }


                }).fail(function (xhr, status, err) {
                    loaderSpin(false);
                    disableButton2(false, 'btnAddPartyGrid');
                    displayModalNoty('Notification', 'No network connectivity. Please check your network connections.', true);
                });
            }
        });
    }

    /*End MSC1 Sharing Party*/

    function SetTotalFee() {
     
        var fee1 = ($('#mChanRuleObj_PAYMENT_FEE').val() != '') ? parseFloat($('#mChanRuleObj_PAYMENT_FEE').val()) : 0;// parseFloat($('#mChanRuleObj_PAYMENT_FEE').val());
        var fee2 = ($('#mChanRuleObj_REDEMPTION_FEE').val() != '') ? parseFloat($('#mChanRuleObj_REDEMPTION_FEE').val()) : 0;// parseFloat($('#mChanRuleObj_REDEMPTION_FEE').val());
        //alert(fee1);
        //alert(fee2);
        var tot = fee1 + fee2;
        //alert(tot);
        $('#divTotalFee').html(tot);
    }
});