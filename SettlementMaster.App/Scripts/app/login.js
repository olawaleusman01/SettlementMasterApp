$(document).ready(function () {
    // alert("success");
    //startProcessing();
    //setTimeout(function () {
    //    endProcessing();
    //}, 10000);

    var url = BaseUrl() + 'Account';
    //var url =  '/Account';
    var validator = $("#formLogin").validate({
        rules: {
            UserName: "required",
            Password: "required",
        },
        messages: {

            // equalTo: "Password must be Equal",
            UserName: {
                required: "Please Enter UserName"
            },
            Password: {
                required: "Please Enter Password"
            },

        },
        submitHandler: function () {
             // e.preventDefault();
            // $('.noty').hide();
            $('.form-row--error').hide();
            var btn = $('#btnSave').val();
            var urlTemp;
            var postTemp;
            var event;
        
           
                urlTemp = url + '/Login';
                postTemp = 'post';
                $('#ItbId').val(0);
                event = 'new';
          
            var $reqLogin = {
                url: urlTemp,

                data: $('#formLogin').serialize(),
                type: postTemp,
                contentType: "application/x-www-form-urlencoded"
            };
           
            $btnSave = $('#btnSave');
            //$loader = $('.loader');
            //disableButton(true);
            $('#btnSave').prop('disabled', 'disabled');
            // $btnSave.hide();
            $btnSave.html('Processing...');
            //return;
            var ajax = new Ajax();
            ajax.send($reqLogin).done(function (response) {
                //$btnSave.show();
               // $loader.hide();
              
                if (response.RespCode === 0) {
                   // new Util().clearform('#formRoles');
                    $btnSave.html('Redirecting...');

                    window.location.href =  response.ReturnUrl;

                }
                else if (response.RespCode === 2) {
                    alert(response.RespMessage);
                    window.location.href = response.ReturnUrl;
                 }
                else {
                    //displayNoty('alert-warning', response.RespMessage, true);
                    $btnSave.html('SIGN IN');
                    $btnSave.removeAttr('disabled');
                    $('.login-error').show();
                    $('.noty').html(response.RespMessage);
                    //displayNoty('alert-warning', response.RespMessage, true);

                }


            }).fail(function (xhr, status, err) {
                $btnSave.show();
                $loader.hide();
                displayNoty('alert-warning', 'No network connectivity. Please check your network connections.', true);
                //$('#display-error').show();
                //$('#errorMessage').text("No network connectivity. Please check your network connections.");
                // errorMessage
                // showPopDialog("No network connectivity. Please check your network connections.", handlerError);
            });


        }
    });
    //$('#formReset').submit(function () {
    //    loaderSpin2();
    //});
    var validator2 = $("#formReset").validate({
        rules: {
            LoginId: "required",
            Password: "required",
            OldPassword: "required",
            //ConfirmPassword: {
            //    required: true,
            //    compare: 'Password'
            //}
        },
        messages: {

            // equalTo: "Password must be Equal",
            UserName: {
                required: "Please Enter UserName"
            },
            Password: {
                required: "Please Enter Password"
            },

        },
        submitHandler: function () {
            // e.preventDefault();
            //alert($('#formReset').serialize());
            $('#notySuccess').hide();
           
            var btn = $('#btnSave').val();
            var urlTemp;
            var postTemp;
            urlTemp = url + '/ResetPassword';
            postTemp = 'post';
            //alert($('#formReset').serialize());
            var $reqLogin = {
                url: urlTemp,
                data: $('#formReset').serialize(),
                type: postTemp,
                contentType: "application/x-www-form-urlencoded"
            };

            $btnSave = $('#btnSave');
            //$loader = $('.loader');
            //disableButton(true);
            $('#btnSave').prop('disabled', 'disabled');
            // $btnSave.hide();
            $btnSave.html('Processing...');
            //return;
            var ajax = new Ajax();
            ajax.send($reqLogin).done(function (response) {
                //$btnSave.show();
                // $loader.hide();

                if (response.RespCode === 0) {
                    //alert(response.RespMessage);
                    $btnSave.html('Reset');
                    $('#notySuccess').show();
                    //$('#notySuccess').css('display', 'block');
                    // new Util().clearform('#formRoles');
                    //$btnSave.html('Redirecting...');
                    //window.location.href = response.ReturnUrl;

                }
                else if (response.RespCode === 2) {
                    alert(response.RespMessage);
                    //$('#notySuccess').css('display', 'block');
                    //window.location.href = response.ReturnUrl;
                    $('#btnSave').removeAttr('disabled');
                    $btnSave.html('Reset');
                }
                else {
                    //displayNoty('alert-warning', response.RespMessage, true);
                    $btnSave.html('Reset');
                    $btnSave.removeAttr('disabled');
                    //displayNoty('alert-warning', response.RespMessage, true);
                    alert(response.RespMessage);
                    $('#btnSave').removeAttr('disabled');
                    $btnSave.html('Reset');
                }


            }).fail(function (xhr, status, err) {
                $btnSave.show();
                $loader.hide();
                displayNoty('alert-warning', 'No network connectivity. Please check your network connections.', true);
                //$('#display-error').show();
                //$('#errorMessage').text("No network connectivity. Please check your network connections.");
                // errorMessage
                // showPopDialog("No network connectivity. Please check your network connections.", handlerError);
            });


        }
    });
    var validator3 = $("#formForgot").validate({
        rules: {
            LoginId: "required",
            //ConfirmPassword: {
            //    required: true,
            //    compare: 'Password'
            //}
        },
        messages: {

            // equalTo: "Password must be Equal",
            UserName: {
                required: "Please Enter UserName"
            },
            Password: {
                required: "Please Enter Password"
            },

        },
        submitHandler: function () {
            // e.preventDefault();
            //alert($('#formReset').serialize());
            $('#notySuccess').hide();

            var btn = $('#btnSave').val();
            var urlTemp;
            var postTemp;
            urlTemp = url + '/ForgotPassword';
            postTemp = 'post';
            //alert($('#formReset').serialize());
            var $reqLogin = {
                url: urlTemp,
                data: $('#formForgot').serialize(),
                type: postTemp,
                contentType: "application/x-www-form-urlencoded"
            };

            $btnSave = $('#btnSave');
            //$loader = $('.loader');
            //disableButton(true);
            $('#btnSave').prop('disabled', 'disabled');
            // $btnSave.hide();
            $btnSave.html('<i class="fa fa-spin fa-spinner"></i> Processing...');
            //return;
            var ajax = new Ajax();
            ajax.send($reqLogin).done(function (response) {
                //$btnSave.show();
                // $loader.hide();

                if (response.RespCode === 0) {
                    //alert(response.RespMessage);
                    $btnSave.html('<i class="fa fa-refresh"></i> Reset');
                    $('#notySuccess').show();
                    //$('#notySuccess').css('display', 'block');
                    // new Util().clearform('#formRoles');
                    //$btnSave.html('Redirecting...');
                    //window.location.href = response.ReturnUrl;

                }
                else if (response.RespCode === 2) {
                    alert(response.RespMessage);
                    //$('#notySuccess').css('display', 'block');
                    //window.location.href = response.ReturnUrl;
                    $('#btnSave').removeAttr('disabled');
                    $btnSave.html('<i class="fa fa-refresh"></i> Reset');

                }
                else {
                    //displayNoty('alert-warning', response.RespMessage, true);
                    $btnSave.html('<i class="fa fa-refresh"></i> Reset');

                    $btnSave.removeAttr('disabled');
                    //displayNoty('alert-warning', response.RespMessage, true);
                    alert(response.RespMessage);
                }


            }).fail(function (xhr, status, err) {
                //$btnSave.show();
                $loader.hide();
                displayNoty('alert-warning', 'No network connectivity. Please check your network connections.', true);
                //$('#display-error').show();
                //$('#errorMessage').text("No network connectivity. Please check your network connections.");
                // errorMessage
                // showPopDialog("No network connectivity. Please check your network connections.", handlerError);
            });


        }
    });
});