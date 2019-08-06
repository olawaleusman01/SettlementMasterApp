
$(document).ready(function () {
    var col;
    var url = BaseUrl() + 'CompanyProfile/';
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
                COMPANY_CITY: "required",
                COMPANY_EMAIL: {
                    required: true,
                    email:true,
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

                    urlTemp = url + 'AddPolicy';
                    postTemp = 'post';
                    //$('#RoleId').val(0);
                    event = 'new';
                }
                else {

                    urlTemp = url + 'AddPolicy';
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

});