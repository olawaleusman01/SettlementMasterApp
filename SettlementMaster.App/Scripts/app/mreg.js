$(document).ready(function () {
    var col;
    var menuId = $('#menuId').val();
    var url = BaseUrl() + 'Mreg/';
    var url2 = BaseUrl() + 'Settings/';
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
            default:
                {
                    setActiveTab(a);
                }
        }
        
    });
    /* Merchant Acct Tab */
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
                var mid = $('#formMReg #mObj_MERCHANTID').val();
                //alert(mid);
                $('#formAcct #MERCHANTID').val(mid);
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
    $(document).on('click', '#btnAddAcct', function () {
        $('#notyMsc').hide();
      
        var urlP = url + 'AddAcct';
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
        var editLink = $(this).attr('data-key');
       // alert(editLink);
        var urlP = url + 'EditAcct/' + editLink ;
       
        console.log(urlP);
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
   /* Merchant Acct Tab End */
   
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

            //console.log(urlP);
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
            var mid = $.trim($('#mObj_MERCHANTID').val());
            //alert(mid);
            var urlP = url + 'AddTerminal/' + mid ;
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

            console.log(urlP);
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
                    ACCOUNTID: "required",
                    SETTLEMENT_FREQUENCY: "required",
                    SETTLEMENT_CURRENCY: "required",
                    TRANSACTION_CURRENCY: "required",
                    PTSP: "required",
                    PTSA: "required",
                    MASTACQUIRERIDNO: {
                        number: true,
                        required: true,
                    },
                    TERMINALOWNER_CODE: {
                        number: true,
                        required:true,
                    },
                    TERMINALOWNER_CODE: {
                        number: true,
                        required: true,
                    },
                    VISAACQUIRERIDNO: {
                        number: true,
                        required: true,
                    },
                    VERVACQUIRERIDNO: {
                        number: true,
                        required: true,
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
                            //console.log(JSON.stringify(response.data_html));
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
        $(document).on('change', '#formTerminal #ACCOUNTID', function () {
            var selected = $.trim($(this).val());
            //alert(selected);
            var urlP = url + 'GetAcctDetail/' + selected;
            console.log(urlP);
            //return;
            $('.lblAcctName').html('');
            $('.lblBankName').html('');
            if (selected != '') {
                $.get(urlP, function (response) {
                    //alert('here');
                    if (response.RespCode == 0) {
                        //Bind Merchant Detail Form
                        $('.lblAcctName').html(response.data.DEPOSIT_ACCTNAME);
                        $('.lblBankName').html(response.data.DEPOSIT_BANKCODE + '-' + response.data.DEPOSIT_BANKNAME);
                    }
                });
            }
        })
    //end of merchant terminal tab

        var validatorAdd = $("#formMReg").validate({
            rules: {
                "mObj.MERCHANTNAME": "required",
                "mObj.MERCHANTID": "required",
                "mObj.CONTACTNAME": "required",
                "mObj.INSTITUTION_COUNTRY": "required",
                //INSTITUTION_CITY: "required",

                "mObj.PHONENO": {
                    number: true,
                },
                "mObj.BUSINESS_CODE": {
                    number: true,
                },
                "mObj.EMAIL": {
                    email: true
                },
                "mObj.STATUS": "required",
                "mObj.COUNTRY_CODE": "required",
                "mObj.STATE_CODE": "required",
                "mObj.INSTITUTION_CBNCODE": "required",
                "mObj.MCC_CODE": "required",
                "mObj.ADDRESS": "required",
            },
            messages: {
            },
            submitHandler: function () {
                if (confirm('Are you sure you want to process this request?')) {
                    var $reqLogin = {
                        url: url + 'Add',
                        data: $('#formMReg').serialize(),
                        type: 'POST',
                        contentType: "application/x-www-form-urlencoded"
                    };
                    console.log($('#formMReg').serialize());
                    //return;
                    loaderSpin2(true);
                    disableButton2(true, 'btnSubmitFinal');
                    var ajax = new Ajax();
                    ajax.send($reqLogin).done(function (response) {
                        loaderSpin2(false);
                        disableButton2(false, 'btnSubmitFinal');
                        if (response.RespCode === 0) {
                            clearMerchantForm(true);
                            $('#tableAcct tbody').empty();
                            $('#tableTid tbody').empty();
                            //$('#rootwizard').find("a[href*='taba']").trigger('click');
                            //location.href = location.href;

                            displayDialogNoty('Notification', response.RespMessage);

                        }
                        else {
                            //$('#notyTerm').show();
                            //$('#notyTermMsg').html(response.RespMessage);
                            displayDialogNoty('Notification', response.RespMessage);
                        }
                    }).fail(function (xhr, status, err) {
                        loaderSpin2(false);
                        disableButton2(false, 'btnSubmitFinal');
                        displayModalNoty('Notification', 'No network connectivity. Please check your network connections.', true);
                    });

                }
            }
        });

        $(document).on('click', '#btnSaveTerm', function (e) {
            e.preventDefault();
            //alert('term click');
            var urlTemp;
            var postTemp;
            urlTemp = url + 'EditMerchantTerminal';
            //console.log(urlTemp);
            postTemp = 'post';
            var $reqLogin = {
                url: urlTemp,
                data: $('#formMerchantTerminal').serialize(),
                type: postTemp,
                contentType: "application/x-www-form-urlencoded"
            };
            //console.log($('#formMerchantTerminal').serialize());
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
        $('#rootwizard').bootstrapWizard({
            'tabClass': 'nav nav-tabs',
            'nextSelector': '.button-next', 'previousSelector': '.button-previous',

            'onNext': function (tab, navigation, index) {
                var $valid = $("#formMReg").valid();
                if (!$valid) {
                    validatorAdd.focusInvalid();
                    return false;
                }
                if (index === 2) {
                    $('.button-final').show();
                }
                else {
                    $('.button-final').hide();
                }

            },
            'onPrevious': function (tab, navigation, index) {
                //var $valid = $("#formMReg").valid();
                //if (!$valid) {
                //    validatorAdd.focusInvalid();
                //    return false;
                //}
                if (index === 2) {
                    $('.button-final').show();
                }
                else {
                    $('.button-final').hide();
                }

            },
            'onTabClick': function (tab, navigation, index) {
                //var $valid = $("#formMReg").valid();
                ////alert($valid);
                //if (!$valid) {
                //    validatorAdd.focusInvalid();
                //    return false;
                //}
                //if (index === 2) {
                //    $('.button-final').show();
                //}
                //else {
                //    $('.button-final').hide();
                //}
                return false;
            }
        });
        $('#rootwizard #btnSubmitFinal').click(function (e) {
            e.preventDefault();
            //alert('final click');
            if (confirm('Are you sure you want to process this request?')) {
                var $reqLogin = {
                    url: url + 'Add',
                    data: $('#formMReg').serialize(),
                    type: 'POST',
                    contentType: "application/x-www-form-urlencoded"
                };
                console.log($('#formMReg').serialize());
                //return;
                loaderSpin2(true);
                disableButton2(true, 'btnSubmitFinal');
                var ajax = new Ajax();
                ajax.send($reqLogin).done(function (response) {
                    loaderSpin2(false);
                    disableButton2(false, 'btnSubmitFinal');
                    if (response.RespCode === 0) {
                        clearMerchantForm(true);
                        $('#tableAcct tbody').empty();
                        $('#tableTid tbody').empty();
                        $('#rootwizard').find("a[href*='taba']").trigger('click');

                        displayDialogNoty('Notification', response.RespMessage);
                      
                    }
                    else {
                        //$('#notyTerm').show();
                        //$('#notyTermMsg').html(response.RespMessage);
                        displayDialogNoty('Notification', response.RespMessage);
                    }
                }).fail(function (xhr, status, err) {
                    loaderSpin2(false);
                    disableButton2(false, 'btnSubmitFinal');
                    displayModalNoty('Notification', 'No network connectivity. Please check your network connections.', true);
                });

            }

        });
        $(document).on('change', '#mObj_MERCHANTID', function () {
            var selected = $.trim($(this).val());
            //console.log(selected);
            var urlP = url + 'GetMerchantDetail/' + selected;
            //console.log(urlP);
            //return;
            if(selected != '' && selected.length == 15)
            {
                $.get(urlP, function (response) {
                    //alert('here');
                    if (response.RespCode == 0) {
                        //Bind Merchant Detail Form
                        bindMerchantForm(response.data);
                    }
                    else {
                        // Clear Merchant Detail Form
                        clearMerchantForm();
                    }
                });
            }
            else {
                clearMerchantForm();
            }
        })
        function bindMerchantForm(data) {
            //console.log(JSON.stringify(response.data));

            $('#mObj_MERCHANTNAME').val(data.MERCHANTNAME);
            $('#mObj_INSTITUTION_CBNCODE').val(data.INSTITUTION_CBNCODE).trigger('change');
            $('#mObj_CONTACTNAME').val(data.CONTACTNAME);
            $('#mObj_PHONENO').val(data.PHONENO);
            $('#mObj_EMAIL').val(data.EMAIL);
            $('#mObj_COUNTRY_CODE').val(data.COUNTRY_CODE).trigger('change');
            $('#mObj_STATE_CODE').val(data.STATE_CODE).trigger('change');
            $('#mObj_MCC_CODE').val(data.MCC_CODE).trigger('change');
            $('#mObj_ADDRESS').val(data.ADDRESS);
            $('#mObj_CITY_NAME').val(data.CITY_NAME);
            $('#mObj_BUSINESS_CODE').val(data.BUSINESS_CODE);
            $('#mObj_CONTACTTITLE').val(data.CONTACTTITLE);
        }
        function clearMerchantForm(dat) {
            if (dat === true) {
                $('#mObj_MERCHANTID').val('');
            }
            $('#mObj_MERCHANTNAME').val('');
            $('#mObj_INSTITUTION_CBNCODE').val('').trigger('change');
            $('#mObj_CONTACTNAME').val('');
            $('#mObj_PHONENO').val('');
            $('#mObj_EMAIL').val('');
            $('#mObj_COUNTRY_CODE').val('NGN').trigger('change');
            $('#mObj_STATE_CODE').val('').trigger('change');
            $('#mObj_MCC_CODE').val('').trigger('change');
            $('#mObj_ADDRESS').val('');
            $('#mObj_CITY_NAME').val('');
            $('#mObj_BUSINESS_CODE').val('');
            $('#mObj_CONTACTTITLE').val('');
        }
});