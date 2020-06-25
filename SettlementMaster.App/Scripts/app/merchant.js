$(document).ready(function () {
    var col;
    $.urlParam = function (name) {
        var results = new RegExp('[\?&]' + name + '=([^&#]*)')
            .exec(window.location.search);

        return (results !== null) ? results[1] || 0 : false;
    };

    console.log($.urlParam('tab')); //edit
  
    var menuId = $('#menuId').val();
    var url = BaseUrl() + 'Merchant/';
    var url2 = BaseUrl() + 'Settings/';
    validateMscForm();
    var table2 = $('.datatable').DataTable({
        ajax: url + "MerchantList?isdefault=Y",
        columns: [
            {
                data: null,
                className: "center_column",
                //defaultContent: '<a class="btn btn-primary btn-xs editor_edit" data-key="' + data.RoleId + '"><i class="fa fa-edit"></i></a>' +
                //    '  <a class="btn btn-warning btn-xs editor_remove" data-key="' + data.RoleId + '"><i class="fa fa-trash-o"></i></a>',
                render: function (data, type, row) {
                    // Combine the first and last names into a single table field

                    var encodedUrl = url + 'Add/' + data.MERCHANTID + '?m=' + encodeURIComponent(menuId);
                    //alert(encodedUrl);
                    var html = '<a class="btn btn-primary btn-xs editor_edit" href="' + encodedUrl + '"><i class="fa fa-edit"></i></a>';
                    return html;
                }
            },
            { data: "MERCHANTID" },
            { data: "MERCHANTNAME" },
            { data: "MCC_CODE" },
            { data: "EMAIL" },
            { data: "CONTACTNAME" },
            { data: "PHONENO" },
            { data: "STATUS" },
        ]
    });

    if ($.urlParam('tab') === 'tabmr') {
        GetMerchantRevenue("#" + $.urlParam('tab'));
    }

    $(document).on('click', '#formSearch #btnSearch', function (e) {
        e.preventDefault();
        var q = $.trim($('#formSearch #q').val());
        var opt = $("#formSearch input[name$='option']:checked").val();
        //alert(q);
        //alert(opt);
       
        if(q === '')
        {
            displayDialogNoty('Notification', 'Search Parameter cannot be empty.');
            return;
        }
        var urlP = url + 'MerchantList?q=' + q + '&option=' + opt;
        //console.log(urlP);
        table2.clear().draw();
        loaderSpin2(true);
        $.get(urlP, function (response) {
            loaderSpin2(false);
            if(response.data)
            {
                for(i = 0;i < response.data.length; i++)
                {
                    table2.row.add(response.data[i]).draw();
                }
            }
        });
    });
    validateMerchantForm();
    function validateMerchantForm() {
        var validatorM = $("#formMerchant").validate({
            rules: {
                MERCHANTNAME: "required",
                MERCHANTID: "required",
                CONTACTNAME: "required",
                INSTITUTION_COUNTRY: "required",
                //INSTITUTION_CITY: "required",

                PHONENO: {
                    number: true,
                },
                EMAIL: {
                    email: true
                },
                STATUS: "required",
                COUNTRY_CODE: "required",
                STATE_CODE: "required",
                "INSTITUTION_CBNCODE": "required",
                "MCC_CODE": "required",
                "ADDRESS": "required",
            },
            messages: {

                // equalTo: "Password must be Equal",
                Name: {
                    required: "Please Enter Last Name"
                },
            },
            submitHandler: function () {
                //console.log($('#formInstitution').serialize());
                //return;
                //loaderSpin2(true);
                //form.submit();
                //  e.preventDefault();
                // alert('success');
                    var btn = $('#btnSaveMerchant').val();
                    var urlTemp;
                    var postTemp;
                    var event;
                    if (btn == "1") {

                        urlTemp = url + 'AddMerchantDetail';
                        postTemp = 'post';
                        $('#ITBID').val(0);
                        event = 'new';
                    }
                    else {
                        urlTemp = url + 'AddMerchantDetail';
                        postTemp = 'post';
                        event = 'modify';
                    };
                    //  alert(urlTemp);
                    var $reqLogin = {
                        url: urlTemp,

                        data: $('#formMerchant').serialize(),
                        type: postTemp,
                        contentType: "application/x-www-form-urlencoded"
                    };
                    console.log($('#formMerchant').serialize());
                    //return;
                    loaderSpin2(true);
                    disableButton(true);
                    var ajax = new Ajax();
                    ajax.send($reqLogin).done(function (response) {
                        loaderSpin2(false);
                        disableButton(false);
                        if (response.RespCode === 0) {
                           
                            displayDialogNoty('Notification', response.RespMessage);
                        }
                        else {
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

    $(document).on('click', '#btnAddAcct', function () {
        $('#notyMsc').hide();
        var menuId = $('#m').val();
        var urlP = url + 'AddAcct?m=' + menuId;
        //console.log(urlP);
        loaderSpin2(true);
        $('#myModalAcct').load(
            urlP, function () {
                loaderSpin2(false);
                $('#myModalAcct').modal({ backdrop: 'static', keyboard: false });
                validateAcctForm();

            });

    });

    /*Merchant MSC*/
    $(document).on('click', '#tableMsc a#edit', function (e) {
        e.preventDefault();
        $('#notyMsc').hide();
        var menuId = $('#m').val();
        // alert(menuId);
        var editLink = $(this).attr('data-key');
         //alert(editLink);
        var urlP = url + 'EditMsc/' + editLink + '?m=' + encodeURIComponent(menuId);

        //console.log(urlP);
        loaderSpin2(true);
        $.get(
            urlP, function (data) {
                loaderSpin2(false);
                if(data.RespCode == 0)
                {
                    $('#pnlMscForm').html(data.data_html);
                    $('#pnlMscForm').css('display', 'block');
                    $('#pnlMscGrid').css('display', 'none');
                }
            });
    });
  
    /*End of Merchant Msc*/
    /*Domestic MSC 2 Subsidy*/
    $(document).on('click', '#tableDomMsc2 a#addDomMsc2', function () {
        // alert('here');
        $('#notyDomMsc2').hide();
        var urlP = url + 'AddDomMsc2Party';
        //console.log(urlP);
        loaderSpin2(true);
        $('#myModalDomMsc2').load(
            urlP, function () {
                loaderSpin2(false);
                $('#myModalDomMsc2').modal({ backdrop: 'static', keyboard: false });
                validateDomMsc2Form();
            });

    });
    $(document).on('click', '#tableDomMsc2 a#edit', function (e) {
        e.preventDefault();
        $('#notyDomMsc2').hide();
        // alert(menuId);
        var editLink = $(this).attr('data-key');
        // alert(editLink);
        var urlP = url + 'EditDomMsc2Party/' + editLink;

        //console.log(urlP);
        loaderSpin2(true);
        $('#myModalDomMsc2').load(
            urlP, function () {
                loaderSpin2(false);
                $('#myModalDomMsc2').modal({ backdrop: 'static', keyboard: false });
                validateDomMsc2Form();
            });
    });
    $(document).on('click', '#tableDomMsc2 a#delete', function (e) {
        e.preventDefault();
        if (confirm('Are you sure you want to delete record?')) {
           
            var editLink = $(this).attr('data-key');
           // alert(editLink);
            var urlP = url + 'DeleteDomMsc2Party/' + editLink;
            //alert(urlP);
            loaderSpin2(true);
            $.get(
                urlP, function (response) {
                    loaderSpin2(false);
                    if (response.RespCode == 0) {
                        $('#divDomMsc2').html(response.data_html);
                        $('#DOM_MSC2').val(response.data_msc2);
                        $('#DOM_MSC2CAP').val(response.data_cap2);
                        SetTotalMscDom();
                        SetTotalCapDom();
                    }
                    else {
                        displayDialogNoty('Notification', response.RespMessage);
                    }
                });
        }
    });
    function validateDomMsc2Form() {

        var validator = $("#formDomMsc2").validate({
            rules: {
                PARTY_ID: "required",
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
                var btn = $('#btnAddDomMsc2Grid').val();
                var urlTemp;
                var postTemp;
                $('#MCCMSC_ITBID').val($('#formMSC #ITBID').val());
                //alert($('#formMSC #ITBID').val());
                urlTemp = url + 'AddDomMsc2';
                postTemp = 'post';
                var $reqLogin = {
                    url: urlTemp,
                    data: $('#formDomMsc2').serialize(),
                    type: postTemp,
                    contentType: "application/x-www-form-urlencoded"
                };
                //console.log($('#formDomMsc2').serialize());
                //return;
                loaderSpin(true);
                disableButton2(true, 'btnAddDomMsc2Grid');
                var ajax = new Ajax();
                ajax.send($reqLogin).done(function (response) {
                    loaderSpin(false);
                    disableButton2(false, 'btnAddDomMsc2Grid');
                    if (response.RespCode === 0) {

                        $('#myModalDomMsc2').modal('hide');
                        $('#divDomMsc2').html(response.data_html);
                        $('#DOM_MSC2').val(response.data_msc2);
                        $('#DOM_MSC2CAP').val(response.data_cap2);
                        SetTotalMscDom();
                        SetTotalCapDom();
                       // console.log(response.data_msc2);
                        //console.log(JSON.stringify(response.data_html));
                    }
                    else {
                        $('#notyDomMsc2').show();
                        $('#notyDomMsc2Msg').html(response.RespMessage);
                        // displayModalNoty('Notification', response.RespMessage);
                    }


                }).fail(function (xhr, status, err) {
                    loaderSpin(false);
                    disableButton2(false, 'btnAddDomMsc2Grid');
                    displayModalNoty('Notification', 'No network connectivity. Please check your network connections.', true);
                });
            }
        });
    }
    $(document).on('change', "#formDomMsc2 #splitincome", function () {
        //alert('checked');
        if ($("#splitincome").is(":checked")) {
            $('#divShareIncome').css('display', 'block');
        }
        else {
            $('#divShareIncome').css('display', 'none');
        }
    });
    $(document).on('change', "#formDomMsc2 #PARTY_ID", function () {
        var selected = $(this).val();
        //alert(selected);
        if(selected !== '')
        {
            if(selected === "1|I")
            {
                $('#divIncomeChk').css('display', 'block');
            }
            else {
                $('#divIncomeChk').css('display', 'none');
            }
        }
    })
    /*End Domestic MSC 2 Subsidy*/
    function setActiveTab(tab) {
        //alert(tab);
        $('.nav-tabs a[href="' + tab + '"]').tab('show');
        //var a = $(tab).split('tablink')[1];
        //alert(a)
        //$('div.hide:not(#tab' + a + ')').hide();
        //$('#tab' + a).fadeIn();
    }

    $('.nav-tabs a').on('click', function (e) {
        e.preventDefault();
        var a = $(this).attr('href');
        //if ($('#formProfile #StaffId').val() === '0' && a != '#taba') {
        //alert(a);
        // setActiveTab('#taba');
        //displayDialogNoty('Notification', 'You have to save Personal Information before you can proceed to other information');

        // return;
        //}
        switch(a){
            case "#tabmsc":{
                //alert('#tabmsc');
                GetMerchantMsc(a);
                break;
            }
            case "#tabma":{
             GetMerchantAcct(a);
                break;
            }
            case "#tabmt": {
                GetMerchantTerminal(a);
                break;
            }
            case "#tabmr": {
                GetMerchantRevenue(a);
                break;
            }
            default:
                {
                    setActiveTab(a);
                }
        }
        
    });

    function validateAcctForm() {

        var validator = $("#formAcct").validate({
            rules: {
                DEPOSIT_ACCTNAME: "required",
                DEPOSIT_ACCOUNTNO: {
                    required: true,
                    number: true
                },
                DEPOSIT_BANKCODE: "required",
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

                    data: $('#formAcct').serialize(),
                    type: postTemp,
                    contentType: "application/x-www-form-urlencoded"
                };
                //console.log($reqLogin.url);
                //return;
                loaderSpin(true);
                disableButton2(true, 'btnAddAcctGrid');
                var ajax = new Ajax();
                ajax.send($reqLogin).done(function (response) {
                    loaderSpin(false);
                    disableButton2(false, 'btnAddAcctGrid');
                    if (response.RespCode === 0) {

                        $('#myModalAcct').modal('hide');
                        $('#divMA').html(response.data_html);
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
    
    $(document).on('click', '#btnAddMscToGrid', function (e) {
        // var btn = $('#btnSave').val();
        e.preventDefault();
        var urlTemp;
        var postTemp;
       
        urlTemp = url + 'AddMsc';
        postTemp = 'post';
      
        var $reqLogin = {
            url: urlTemp,

            data: $('#formMSC').serialize(),
            type: postTemp,
            contentType: "application/x-www-form-urlencoded"
        };
        //console.log($('#formMSC').serialize());
        //return;
        loaderSpin2(true);
        disableButton2(true, 'btnAddMscToGrid');
        var ajax = new Ajax();
        ajax.send($reqLogin).done(function (response) {
            loaderSpin2(false);
            disableButton2(false, 'btnAddMscToGrid');
            if (response.RespCode === 0) {
                $('#pnlMscGrid #divMSC').html(response.data_html);
                $('#pnlMscForm').css('display', 'none');
                $('#pnlMscGrid').css('display', 'block');
                //console.log(JSON.stringify(response.data));
                //console.log(JSON.stringify(response.data_msc));
            }
            else {
               // $('#notyMsc').show();
                //$('#notyMscMsg').html(response.RespMessage);
                 displayModalNoty('Notification', response.RespMessage);
            }


        }).fail(function (xhr, status, err) {
            loaderSpin2(false);
            disableButton2(false, 'btnAddMscToGrid');
            displayModalNoty('Notification', 'No network connectivity. Please check your network connections.', true);
        });
    });
    $(document).on('click', '#btnReturnToGrid', function (e) {
        e.preventDefault();
       // alert('return');
        $('#pnlMscForm').css('display', 'none');
        $('#pnlMscGrid').css('display', 'block');
    });
    $(document).on('click', '#btnSaveMsc', function (e) {
        // var btn = $('#btnSave').val();
        e.preventDefault();
        var urlTemp;
        var postTemp;
        urlTemp = url + 'AddMerchantMsc';
        postTemp = 'post';
        var $reqLogin = {
            url: urlTemp,
            data: $('#formMerchantMsc').serialize(),
            type: postTemp,
            contentType: "application/x-www-form-urlencoded"
        };
        //console.log($('#formMerchantMsc').serialize());
        //return;
        loaderSpin2(true);
        disableButton2(true, 'btnSaveMsc');
        var ajax = new Ajax();
        ajax.send($reqLogin).done(function (response) {
            loaderSpin2(false);
            disableButton2(false, 'btnSaveMsc');
            if (response.RespCode === 0) {
                displayDialogNoty('Notification', response.RespMessage);
                $('#btnSaveMsc').prop('disabled', true);
            }
            else {
                displayDialogNoty('Notification', response.RespMessage);
            }
        }).fail(function (xhr, status, err) {
            loaderSpin2(false);
            disableButton2(false, 'btnSaveMsc');
            displayDialogNoty('Notification', 'No network connectivity. Please check your network connections.', true);
        });
    });
    $(document).on('click', '#btnSaveAcct', function (e) {
        // var btn = $('#btnSave').val();
        $('#formMerchantAcct #MID').val($('#MERCHANTID').val());
        e.preventDefault();
        var urlTemp;
        var postTemp;
        urlTemp = url + 'AddMerchantAcct';
        postTemp = 'post';
        var $reqLogin = {
            url: urlTemp,
            data: $('#formMerchantAcct').serialize(),
            type: postTemp,
            contentType: "application/x-www-form-urlencoded"
        };
        //console.log($('#formMerchantAcct').serialize());
        //return;
        loaderSpin2(true);
        disableButton2(true, 'btnSaveAcct');
        var ajax = new Ajax();
        ajax.send($reqLogin).done(function (response) {
            loaderSpin2(false);
            disableButton2(false, 'btnSaveAcct');
            if (response.RespCode === 0) {
                displayDialogNoty('Notification', response.RespMessage);
                $('#btnSaveAcct').prop('disabled', true);
            }
            else {
                displayDialogNoty('Notification', response.RespMessage);
            }
        }).fail(function (xhr, status, err) {
            loaderSpin2(false);
            disableButton2(false, 'btnSaveAcct');
            displayDialogNoty('Notification', 'No network connectivity. Please check your network connections.', true);
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
                        $('#divMA').html(response.data_html);
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
                    $('#divMA').html(response.data_html);
                }
                else {
                    displayDialogNoty('Notification', response.RespMessage);
                }
            });
        //}
    });
    function GetMerchantMsc(tab) {
        var mid = $('#MERCHANTID').val();
        var menuId = $('#m').val();
        var urlP = url + "GetMerchantMsc/" + mid + "?m=" + menuId;
        //console.log(urlP);
        loaderSpin2(true);
        $.get(
            urlP, function (response) {
                loaderSpin2(false);
                if (response.RespCode == 0) {
                    //console.log(response.data_html);
                    $('#divMSC').html(response.data_html);
                    $('#pnlMscForm').css('display', 'none');
                    $('#pnlMscGrid').css('display', 'block');
                    setActiveTab(tab);
                }
                else {
                    displayDialogNoty('Notification', response.RespMessage);

                }
            });
    }
    function GetMerchantAcct(tab) {
        var mid = $('#MERCHANTID').val();
        var menuId = $('#m').val();
        var urlP = url + "GetMerchantAcct/" + mid + "?m=" + menuId;
        //console.log(urlP);
        loaderSpin2(true);
        $.get(
            urlP, function (response) {
                loaderSpin2(false);
                if (response.RespCode == 0) {
                    //console.log(response.data_html);
                    $('#divMA').html(response.data_html);
                    setActiveTab(tab);
                }
                else {
                    displayDialogNoty('Notification', response.RespMessage);

                }
            });
    }
    function GetMerchantTerminal(tab) {
        var mid = $('#MERCHANTID').val();
        var menuId = $('#m').val();
        var urlP = url + "GetMerchantTerminal/" + mid + "?m=" + menuId;
        //console.log(urlP);
        loaderSpin2(true);
        $.get(
            urlP, function (response) {
                loaderSpin2(false);
                if (response.RespCode === 0) {
                    console.log(response.data_html);
                    $('#divMT').html(response.data_html);
                    setActiveTab(tab);
                }
                else {
                    displayDialogNoty('Notification', response.RespMessage);

                }
            });
    }

      
    function validateMscForm() {
        // domestic validation
        // domestic controls
        var $txtDomMsc1 = $('#DOM_MSCVALUE');
        var $txtDomMsc1Cap = $('#DOMCAP');
        var $txtDomMsc2 = $('#DOM_MSC2');
        var $txtDomMsc2Cap = $('#DOM_MSC2CAP');
        var $txtDomSharedMsc = $('#DOM_SHAREDMSC');
        var $txtDomUnSharedMsc = $('#DOM_UNSHAREDMSC');
        var $txtDomSharedCap = $('#DOM_SHAREDCAP');
        var $txtDomUnsharedCap = $('#DOM_UNSHAREDCAP');
        var $txtDomSubsidy = $('#DOM_MSCSUBSIDY');
        var lblTotalDomCap = $('#divTotalDomCap');
        var dom_msc1 = 0;
        var dom_cap1 = 0;
        var dom_sharedCap1 = 0;
        var dom_sharedmsc1 = 0;
        var dom_unsharedmsc1 = 0;
        var dom_msc2 = 0;
        var dom_cap2 = 0;
        var dom_subsidy = 0;

        // international controls
        var $txtIntMsc1 = $('#INT_MSCVALUE');
        var $txtIntMsc1Cap = $('#INTLCAP');
        var $txtIntMsc2 = $('#INT_MSC2');
        var $txtIntMscCap = $('#INT_MSC2CAP');
        var $txtIntSharedMsc = $('#INT_SHAREDMSC');
        var $txtIntUnsharedMsc = $('#INT_UNSHAREDMSC');
        var $txtIntsharedCap = $('#INT_SHAREDCAP');
        var $txtIntUnsharedCap = $('#INT_UNSHAREDCAP');
        var $txtIntSubsidy = $('#INT_MSCSUBSIDY');
        var lblTotalIntMsc = $('#divIntTotalMsc');
        var lblTotalIntCap = $('#divIntTotalCap');
        var int_msc1 = 0;
        var int_cap1 = 0;
        var int_sharedCap1 = 0;
        var int_sharedmsc1 = 0;
        var int_unsharedmsc1 = 0;
        var int_msc2 = 0;
        var int_cap2 = 0;
        var int_subsidy = 0;
        $(document).on('change', '#DOM_MSCVALUE', function (e) {
            var $txtDomMsc1 = $('#DOM_MSCVALUE');
            var $txtDomSharedMsc = $('#DOM_SHAREDMSC');
            var $txtDomUnSharedMsc = $('#DOM_UNSHAREDMSC');
            //alert('domestic');
            //alert($txtDomMsc1.val());
            dom_msc1 = parseFloat($(this).val());
            dom_sharedmsc1 = parseFloat($($txtDomSharedMsc).val());
            //alert(dom_msc1);
            //alert(dom_sharedmsc1);
            if ($.isNumeric(dom_msc1)) {
                //alert('am numeric');
                if ($.isNumeric(dom_sharedmsc1)) {
                    //alert('am also numeric');
                    if (dom_msc1 < 0) {
                        $(this).val('0');
                        SetTotalMscDom();
                        displayDialogNoty('Notification','MSC1 Value cannot be a Negative Value');
                    }
                    else if (dom_msc1 < dom_sharedmsc1) {
                        //alert('am less than shared msc1');
                        $(this).val('0');
                        SetTotalMscDom();
                        //alert('MSC1 Value cannot be lesser than MSC1 Unshared Value');
                        displayDialogNoty('Notification','MSC1 Value cannot be lesser than MSC1 Unshared Value');
                    }
                    else {
                        //alert((dom_msc1 - dom_sharedmsc1).toFixed(2));
                        var diff = dom_msc1 - dom_sharedmsc1; //do the difference btw msc1 and shaed msc1 to unshared msc1
                        $($txtDomUnSharedMsc).val(diff.toFixed(2));
                        SetTotalMscDom();
                    };
                }
            }
            else {

                $(this).val('0'); // default field value to zero
                SetTotalMscDom();
                displayDialogNoty('Notification','Only numeric value is expected for MSC 1');
            };
        });
        $(document).on('change', '#DOM_SHAREDMSC', function (e) {
            var $txtDomMsc1 = $('#DOM_MSCVALUE');
            var $txtDomSharedMsc = $('#DOM_SHAREDMSC');
            var $txtDomUnSharedMsc = $('#DOM_UNSHAREDMSC');
            dom_sharedmsc1 = parseFloat($(this).val());
            dom_msc1 = parseFloat($($txtDomMsc1).val());
            // alert(dom_msc1); alert(dom_sharedmsc1);
            if (!isNaN(dom_sharedmsc1) && dom_sharedmsc1 >= 0) {
                //alert(dom_sharedmsc1);
                if ($.isNumeric(dom_msc1) && dom_msc1 > 0) {
                    if (dom_msc1 < dom_sharedmsc1) {
                        $(this).val('0');
                        SetTotalMscDom();
                        displayDialogNoty('Notification', 'MSC1 Value cannot be lesser than MSC1 Unshared Cap Value');
                    }
                    else {
                        //alert((dom_msc1 - dom_sharedmsc1).toFixed(2));
                        var diff = dom_msc1 - dom_sharedmsc1; //do the difference btw msc1 and shaed msc1 to unshared msc1
                        $($txtDomUnSharedMsc).val(diff.toFixed(2));
                        SetTotalMscDom();
                    };
                }
            }
            else if (dom_sharedmsc1 < 0) {
                $(this).val('0'); // default field value to zero
                SetTotalMscDom();
                displayDialogNoty('Notification', 'MSC 1 Unshared Value cannot be set to negative value');
            }
            else if (isNaN(dom_sharedmsc1)) {
                $(this).val('0'); // default field value to zero
                $($txtDomUnSharedMsc).val(dom_msc1);
                SetTotalMscDom();
                displayDialogNoty('Notification', 'Only numeric value is expected for MSC 1 Unshared Value');
            }
        });
        $(document).on('change', '#DOMCAP', function (e) {

            var $txtDomMsc1Cap = $('#DOMCAP');
            var $txtDomSharedCap = $('#DOM_SHAREDCAP');
            var $txtDomUnsharedCap = $('#DOM_UNSHAREDCAP');

            dom_cap1 = parseFloat($(this).val());
            dom_sharedCap1 = parseFloat($($txtDomSharedCap).val());
            
            if (!isNaN(dom_cap1) && dom_cap1 > 0) {
                if (!isNaN(dom_sharedCap1) && dom_sharedCap1 > dom_cap1) {
                    $($txtDomSharedCap).val('0');
                    SetTotalCapDom();
                    displayDialogNoty('Notification', 'MSC 1 Shared Cap cannot be greater than MSC 1 Cap if MSC 1 Cap is set.');
                }
                else if (!isNaN(dom_sharedCap1)) {
                    var diff = dom_cap1 - dom_sharedCap1; //do the difference btw msc1 and shaed msc1 to unshared msc1
                    $($txtDomUnsharedCap).val(diff.toFixed(2));
                    SetTotalCapDom();
                }
                else {
                    $($txtDomUnsharedCap).val(dom_cap1);
                    SetTotalCapDom();
                    // if (!isNaN(dom_sharedCap1) && dom_sharedCap1 > dom_cap1)
                }
            }
            else if (dom_cap1 == 0) {
                $($txtDomUnsharedCap).val('');
                SetTotalCapDom();
            }
            else if (dom_cap1 < 0) {
                $(this).val('0'); // default field value to zero
                $($txtDomUnsharedCap).val('');
                SetTotalCapDom();
                alert('MSC 1 Cap cannot be set to negative value');
            }
            else if (isNaN(dom_cap1)) {
                $(this).val('0'); // default field value to zero
                $($txtDomUnsharedCap).val('');
            }
        });
        $(document).on('change', '#DOM_SHAREDCAP', function (e) {
            var $txtDomMsc1Cap = $('#DOMCAP');
            var $txtDomSharedCap = $('#DOM_SHAREDCAP');
            var $txtDomUnsharedCap = $('#DOM_UNSHAREDCAP');

            dom_sharedCap1 = parseFloat($(this).val());
            dom_cap1 = parseFloat($($txtDomMsc1Cap).val());
        
            if (!isNaN(dom_sharedCap1) && dom_sharedCap1 >= 0) {
                if (dom_cap1 != 0 && dom_sharedCap1 > dom_cap1) {
                    $(this).val('0'); // default field value to zero

                    // var diff = dom_cap1 - dom_sharedCap1; //do the difference btw msc1 and shared msc1 to unshared msc1
                    $($txtDomUnsharedCap).val(dom_cap1);
                    SetTotalCapDom();
                    displayDialogNoty('Notification', 'MSC 1 Shared Cap cannot be greater than MSC 1 Cap if MSC 1 Cap is set.');
                }
                else if (dom_cap1 != 0) {
                    var diff = dom_cap1 - dom_sharedCap1; //do the difference btw msc1 and shared msc1 to unshared msc1
                    $($txtDomUnsharedCap).val(diff);
                    SetTotalCapDom();
                }
                else {
                    // alert((dom_cap1 - dom_sharedCap1).toFixed(2));
                    // var diff = dom_cap1 - dom_sharedCap1; //do the difference btw msc1 and shared msc1 to unshared msc1
                    $($txtDomUnsharedCap).val('');
                    SetTotalCapDom();
                };
            }
            else if (!isNaN(dom_sharedCap1) && dom_sharedCap1 < 0) {
                $(this).val('0'); // default field value to zero
                SetTotalCapDom();
                displayDialogNoty('Notification', 'MSC Unshared Cap cannot be set to negative value');
            }
                //else if ($(this).val().trim() == '') {

                //    $(this).val('0'); // default field value to zero
                //    $(txtDomUnsharedCap).val(dom_cap1);
                //    SetTotalCapDom();

                //    //  alert('Only numeric value is expected for MSC 1 Unshared Value');
                //}
            else if (isNaN(dom_sharedCap1)) {

                $(this).val('0'); // default field value to zero
                SetTotalCapDom();
                displayDialogNoty('Notification', 'Only numeric value is expected for MSC 1 Unshared Cap');
                // $(txtDomUnsharedCap).val(dom_cap1);
            };
        });
        $(document).on('change', '#DOM_MSCSUBSIDY', function (e) {
            var $txtDomMsc1 = $('#DOM_MSCVALUE');
            var $txtDomSubsidy = $('#DOM_MSCSUBSIDY');
            dom_subsidy = parseFloat($(this).val());
            dom_msc1 = parseFloat($($txtDomMsc1).val());

            if ($.isNumeric(dom_subsidy)) {
                //alert(dom_subsidy);
                if (dom_subsidy < 0) {

                    $(this).val('0'); // default field value to zero

                    displayDialogNoty('Notification', 'Subsidy Value cannot be negative value');
                }
                else if ($.isNumeric(dom_msc1)) {
                    if (dom_msc1 < dom_subsidy) {
                        $(this).val('0'); // default field value to zero
                        displayDialogNoty('Notification', 'Subsidy Granted cannot be more than Domestic MSC 1 Value');
                    }
                }
                else {
                    $(this).val('0');
                    displayDialogNoty('Notification', 'Please Set Domestic MSC 1 Value before setting Subsidy Vale');
                }
            }
            else {

                $(this).val('0'); // default field value to zero
                //  SetTotalMscDom();
                displayDialogNoty('Notification', 'Only numeric value is expected for Subsidy');
            };
        });
       
      
    }
        function SetTotalMscDom() {
            var $txtDomMsc1 = $('#DOM_MSCVALUE');
            var $txtDomMsc2 = $('#DOM_MSC2');
            var divDomTotalMsc = $('#divTotalDomMsc');
            dom_msc1 = parseFloat($txtDomMsc1.val());
            dom_msc2 = parseFloat($txtDomMsc2.val());
             //alert('b4');
            if (dom_msc1 && dom_msc2) {

                divDomTotalMsc.html((dom_msc1 + dom_msc2).toFixed(2));
            }
            else if (dom_msc1) {
                // alert('1');
                divDomTotalMsc.html(dom_msc1);
            }
            else if (dom_msc2) {
                // alert('2');
                divDomTotalMsc.html(dom_msc2);
            }
        }
        function SetTotalCapDom() {
            var $txtDomMsc1Cap = $('#DOMCAP');
            var $txtDomMsc2Cap = $('#DOM_MSC2CAP');
            var divDomTotalCap = $('#divTotalDomCap');
            dom_cap1 = parseFloat($txtDomMsc1Cap.val());
            dom_cap2 = parseFloat($txtDomMsc2Cap.val());
            // alert('b4');
            if ($.isNumeric(dom_cap1) && $.isNumeric(dom_cap2)) {

                divDomTotalCap.html((dom_cap1 + dom_cap2).toFixed(2));
            }
            else if ($.isNumeric(dom_cap1)) {
                // alert('1');
                divDomTotalCap.html(dom_cap1);
            }
            else if ($.isNumeric(dom_cap2)) {
                // alert('2');
                divDomTotalCap.html(dom_cap1);
            }
        }
        function SetTotalMscInt() {
            int_msc1 = parseFloat($txtIntMsc1.val());
            int_msc2 = parseFloat($txtIntMsc2.val());
            // alert('b4');
            if ($.isNumeric(int_msc1) && $.isNumeric(int_msc2)) {
                //alert((int_msc1 + int_msc2).toFixed(2));
                divIntTotalMsc.val((int_msc1 + int_msc2).toFixed(2));
            }
            else if ($.isNumeric(int_msc1)) {
                // alert('1');
                divIntTotalMsc.val(int_msc1);
            }
            else if ($.isNumeric(int_msc2)) {
                // alert('2');
                divIntTotalMsc.val(int_msc2);
            }
        }
        function SetTotalCapInt() {
            int_cap1 = parseFloat($txtIntMsc1Cap.val());
            int_cap2 = parseFloat($txtIntMscCap.val());
            // alert('b4');
            if ($.isNumeric(int_cap1) && $.isNumeric(int_cap2)) {

                divIntTotalCap.val((int_cap1 + int_cap2).toFixed(2));
            }
            else if ($.isNumeric(int_cap1)) {
                // alert('1');
                divIntTotalCap.val(int_cap1);
            }
            else if ($.isNumeric(int_cap2)) {
                // alert('2');
                divIntTotalCap.val(int_cap2);
            }
        }

        $(document).on('change', '#COUNTRY_CODE', function () {
            //alert($(this).val());
            BindState($(this).val(), '#STATE_CODE');
        });
        //$(document).on('change', '#STATE_CODE', function () {
        //    //alert($(this).val());
        //    BindCity($(this).val(), '#CITY_NAME');
        //});
        function BindState(countryCode, $id) {
            try {
                $($id).empty();
                $($id).append("<option value=''> --Select State-- </option>");
                $('#CITY_NAME').empty();
                $('#CITY_NAME').append("<option value=''> --Select City-- </option>");
                var urlP = url2 + "StateList?countryCode=" + countryCode 
                $.get(urlP, function (response) {
                    if (response.data.length > 0) {
                        for (var i = 0; i < response.data.length; i++) {
                            $($id).append("<option value='" + response.data[i].STATECODE + "'>" +
                            response.data[i].STATENAME + "</option>");
                        }

                    }
                });
            }
            catch (err) {

            }
        }
        function BindCity(countryCode,stateCode, $id) {
            try {
                $($id).empty();
                $($id).append("<option value=''> --Select City-- </option>");
                var urlP = url2 + "CityList?countryCode=" + countryCode + '&stateCode=' + stateCode;
                $.get(urlP, function (response) {
                    if (response.data.length > 0) {
                        for (var i = 0; i < response.data.length; i++) {
                            $($id).append("<option value='" + response.data[i].STATECODE + "'>" +
                            response.data[i].STATENAME + "</option>");
                        }

                    }
                });
            }
            catch (err) {

            }
        }

    //Merchant Terminal Tab
        $(document).on('click', '#tableTidLocal a.editor_edit', function (e) {
            e.preventDefault();
            $('#notyTerm').hide();
            //var menuId = $('#m').val();
           // alert(menuId);
            var editLink = $(this).attr('data-key');
            //alert(editLink);
            var urlP = url + 'EditTerminalLocal/' + editLink;

            console.log(urlP);
            loaderSpin2(true);
            $('#myModalTerm').load(
                urlP, function () {
                    loaderSpin2(false);
                    $('#myModalTerm').modal({ backdrop: 'static', keyboard: false });
                    validateTerminalForm();
                });
        });
        $(document).on('click', '#btnAddTerminal', function () {
            $('#notyTerm').hide();
            //var menuId = $('#m').val();
           
            var urlP = url + 'AddTerminal/' + mid;
            loaderSpin2(true);
            $('#myModalTerm').load(
                urlP, function () {
                    loaderSpin2(false);
                    $('#myModalTerm').modal({ backdrop: 'static', keyboard: false });
                    validateTerminalForm();

                });

        });
        $(document).on('click', '#tableTid a#edit', function (e) {
            e.preventDefault();
            $('#notyTerm').hide();
            //var menuId = $('#m').val();
            // alert(menuId);
            var editLink = $(this).attr('data-key');
            // alert(editLink);
            var urlP = url + 'EditTerminal/' + editLink;

            //console.log(urlP);
            loaderSpin2(true);
            $('#myModalTerm').load(
                urlP, function () {
                    loaderSpin2(false);
                    $('#myModalTerm').modal({ backdrop: 'static', keyboard: false });
                    validateTerminalForm();
                });
        });
        $(document).on('click', '#tableTid a#delete', function (e) {
            e.preventDefault();
            if (confirm('Are you sure you want to delete record?')) {
                //$('#notyMsc').hide();
               // var menuId = $('#m').val();
                // alert(menuId);
                var editLink = $(this).attr('data-key');
                // alert(editLink);
                var urlP = url + 'DeleteTerminal/' + editLink; //+ '?m=' + encodeURIComponent(menuId);
                //var acq = $('#CBN_CODE').val();

                //console.log(urlP);
                loaderSpin2(true);
                $.get(
                    urlP, function (response) {
                        loaderSpin2(false);
                        if (response.RespCode == 0) {
                            //console.log(response.data_msc);
                            $('#divTermQueue').html(response.data_html);
                        }
                        else {
                            displayDialogNoty('Notification', response.RespMessage);
                        }
                    });
            }
        });
        $(document).on('click', '#tableTid a#undo', function (e) {
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
        function validateTerminalForm() {

            var validator = $("#formTerminal").validate({
                rules: {
                    TERMINALID: {
                        required: true,
                        minlength: 8,
                        maxlength:8
                    },
                    ACCOUNT_ID: "required",
                    SETTLEMENT_FREQUENCY: "required",
                    SETTLEMENT_CURRENCY: "required",
                    TRANSACTION_CURRENCY: "required",
                    PTSP: "required",
                    PTSA: "required",
                    MASTACQUIRERIDNO: {
                        required: true,
                        number: true,
                        minlength: 6,
                        maxlength:6
                    },
                    VISAACQUIRERIDNO: {
                        required: true,
                        number: true,
                        minlength: 6,
                        maxlength: 6
                    },
                    VERVACQUIRERIDNO: {
                        required: true,
                        number: true,
                        minlength: 6,
                        maxlength: 6
                    },
                    TERMINALOWNER_CODE: {
                        required: true,
                        number: true,
                        //minlength: 6,
                        //maxlength: 6
                    }
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
                    urlTemp = url + 'AddTerminal';
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

                        data: $('#formTerminal').serialize(),
                        type: postTemp,
                        contentType: "application/x-www-form-urlencoded"
                    };
                    //console.log($('#formTerminal').serialize());
                    //return;
                    loaderSpin(true);
                    disableButton2(true, 'btnAddTermGrid');
                    var ajax = new Ajax();
                    ajax.send($reqLogin).done(function (response) {
                        loaderSpin(false);
                        disableButton2(false, 'btnAddTermGrid');
                        if (response.RespCode === 0) {
                            $('#myModalTerm').modal('hide');
                            $('#divTermQueue').html(response.data_html);
                            //console.log(JSON.stringify(response.data));
                            //console.log(JSON.stringify(response.data_msc));
                        }
                        else {
                            $('#notyTerm').show();
                            $('#notyTermMsg').html(response.RespMessage);
                            // displayModalNoty('Notification', response.RespMessage);
                        }
                    }).fail(function (xhr, status, err) {
                        loaderSpin(false);
                        disableButton2(false, 'btnAddTermGrid');
                        displayModalNoty('Notification', 'No network connectivity. Please check your network connections.', true);
                    });
                }
            });

        }
        $(document).on('click', '#btnSaveTerm', function (e) {
            e.preventDefault();
            //alert('term click');
            var urlTemp;
            var postTemp;
            urlTemp = url + 'EditMerchantTerminal';
            console.log(urlTemp);
            postTemp = 'post';
            var $reqLogin = {
                url: urlTemp,
                data: $('#formMerchantTerminal').serialize(),
                type: postTemp,
                contentType: "application/x-www-form-urlencoded"
            };
            console.log($('#formMerchantTerminal').serialize());
            loaderSpin2(true);
            disableButton2(true, 'btnSaveTerm');
            var ajax = new Ajax();
            ajax.send($reqLogin).done(function (response) {
                loaderSpin2(false);
                disableButton2(false, 'btnSaveTerm');
                if (response.RespCode === 0) {
                    displayDialogNoty('Notification', response.RespMessage);
                    $('#divTermQueue').html(response.data_html);
                    $('#btnSaveTerm').prop('disabled', true);
                }
                else {
                    displayDialogNoty('Notification', response.RespMessage);
                }
            }).fail(function (xhr, status, err) {
                loaderSpin2(false);
                disableButton2(false, 'btnSaveTerm');
                displayDialogNoty('Notification', 'No network connectivity. Please check your network connections.', true);
            });
        });
        $(document).on('change', '#formDomMsc2 #PARTY_ID', function (e) {
            var selected = $(this).val();
            //console.log(selected);
            var $id = '#formDomMsc2 #ACCOUNT_ID';
            $($id).empty();
            $($id).append("<option value=''> --Select Account-- </option>");

            if (selected != '') {
                try {
                    
                    var urlP = url + "GetPartyAcct/" + selected;
                    //.log(urlP);
                    //return;
                    $.get(urlP, function (response) {
                        if (response.data.length > 0) {
                            for (var i = 0; i < response.data.length; i++) {
                                $($id).append("<option value='" + response.data[i].Code + "'>" +
                                response.data[i].Description + "</option>");
                            }

                        }
                    });
                }
                catch (err) {

                }
            }
        });

    //Merchant Revenue Tab
    function GetMerchantRevenue(tab) {
        var mid = $('#MERCHANTID').val();
        var menuId = $('#m').val();
        var urlP = url + "RvGroupList/" + mid + "?m=" + menuId;
        //console.log(urlP);
        loaderSpin2(true);
        $.get(
            urlP, function (response) {
                loaderSpin2(false);
                if (response.RespCode === 0) {
                    //console.log(response.data_html);
                    $('#divMR').html(response.data_html);
                    setActiveTab(tab);
                }
                else {
                    displayDialogNoty('Notification', response.RespMessage);

                }
            });
    }

  
});