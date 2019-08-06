
$(document).ready(function () {
    var col;
    var url = BaseUrl() + 'CompanyProfile/';
    var url2 = BaseUrl() + 'Settings/';
        var validator = $("#formCompany").validate({
            rules: {
                COMPANY_NAME: "required",
                COMPANY_CODE: "required",
                COMPANY_ADDRESS: "required",
                COMPANY_PHONE1: "required",
                COMPANY_WEBSITE: "required",
                COMPANY_CODE: "required",
                COMPANY_COUNTRY: "required",
                COMPANY_STATE: "required",
                //COMPANY_CITY: "required",
                COMPANY_EMAIL: {
                    required: true,
                    email:true,
                },
                NIBSSRATE: {
                    //required: true,
                    number:true,
                },
                WHTAXRATE: {
                    //required: true,
                    number:true,
                },
                WHTACCOUNT: {
                    //required: true,
                    number:true,
                    minlength: 10,
                    maxlength:10
                },
                NIBSSACCOUNT: {
                    number: true,
                    minlength: 10,
                    maxlength: 10
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
                var btn = $('#btnSave').val();
                var urlTemp;
                var postTemp;
                var event;
                if (btn == "1") {

                    urlTemp = url + 'Create';
                    postTemp = 'post';
                    //$('#RoleId').val(0);
                    event = 'new';
                }
                else {

                    urlTemp = url + 'Create';
                    //postTemp = 'put';
                    postTemp = 'post';
                    event = 'modify';
                };
               // alert(urlTemp);
                var $reqLogin = {
                    url: urlTemp,

                    data: $('#formCompany').serialize(),
                    type: postTemp,
                    contentType: "application/x-www-form-urlencoded"
                };
                console.log($('#formCompany').serialize());
                //return;
                loaderSpin2(true);
                disableButton(true);
                var ajax = new Ajax();
                ajax.send($reqLogin).done(function (response) {
                    loaderSpin2(false);
                    disableButton(false);
                    if (response.RespCode === 0) {
                        //new Util().clearform('#formCompany');

                       // $('a.editor_return').click();
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
        $(document).on('change', '#COMPANY_COUNTRY', function () {
            //alert($(this).val());
            BindState($(this).val(), '#COMPANY_STATE');
        });
        //$(document).on('change', '#COMPANY_STATE', function () {
        //    //alert($(this).val());
        //    var country = $('#COMPANY_COUNTRY').val();
        //    //alert(country);
        //    BindCity(country, $(this).val(), '#CITY_NAME');

        //});
        function BindState(countryCode, $id) {
            try {
                $($id).empty();
                $($id).append("<option value=''> --Select State-- </option>");
                //$('#CITY_NAME').empty();
                //$('#CITY_NAME').append("<option value=''> --Select City-- </option>");
                //alert('here');
                var urlP = url2 + "StateList?countryCode=" + countryCode
                //console.log(urlP);
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
        function BindCity(countryCode, stateCode, $id) {
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
});