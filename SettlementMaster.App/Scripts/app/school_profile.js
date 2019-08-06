$(document).ready(function () {
    // alert("success");
    var col;
    var table2 = $('.datatable').DataTable(
    );
    

    var examTypeArray = [];
    //var exanTypeObj = new Object;

    var url = '/Setting/';

    $("#FormSchool").validate({
        rules: {
            SchoolName: "required",
            SchoolAddress: "required",
            CountryId: "required",
            StateId: "required",
            Email: "required",
            //Password: {
            //    minlength: 6
            //},
            //ConfirmPassword: {
            //    minlength: 6,
            //    equalTo: "#Password"
            //},
     
        },
        messages: {

            // equalTo: "Password must be Equal",
            SchoolName: {
                required: "Please Enter School Name"

            },
            //Email: {
            //    required: "Please Select an examination Type"

            //},

        },
        submitHandler: function (form) {
            //  e.preventDefault();
            //  alert('success');
            var btn = $('#btnSubmit').text();
            var urlTemp;
            var postTemp;
           // var event;
           

                urlTemp = url + 'CreateSchoolProfile';
                //postTemp = 'put';
                postTemp = 'post';
              //  event = 'modify';
            
            //  alert(urlTemp);
            var $reqLogin = {
                url: urlTemp,

                data: $('#FormSchool').serialize(),
                type: postTemp,
                contentType: "application/x-www-form-urlencoded"
            };
            // alert("after submit");
             $('#ajax-loading').show();
            $('#btnSubmit').attr('disabled', 'disabled');
            var ajax = new Ajax();
            ajax.send($reqLogin).done(function (response) {
                   $('#ajax-loading').hide();
                //  $('#display-error').hide();
                //  var res = JSON.stringify(response);
                //alert(response);
                //console.log(res);
                $('#btnSubmit').removeAttr('disabled', 'disabled');
                $('#pnlAudit').css('display', 'none');
                if (response.ResponseCode === 0) {
                
                     
                        alert('Record Updated successfully');
                }
                else
                {
                    alert(response.ResponseCode + '-' + response.ResponseMessage);
                }
                //  $('#SubjectID').val('0');
                // updateLocalBank(response);

            }).fail(function (xhr, status, err) {
                //Disable Button
                // loginbtn.hide();
                //hide login loader
                $('#ajax-loading').hide();
                $('#display-error').show();
                $('#errorMessage').text("No network connectivity. Please check your network connections.");
                // errorMessage
                // showPopDialog("No network connectivity. Please check your network connections.", handlerError);
            });


        }
    });



   


    function formClear(ele) {
        //alert("i got here");
        $(ele).find(':input').each
        (function () {
            switch (this.type) {
                case 'text':
                case 'textarea':
                case 'single-text':
                    $(this).val('');
                case 'radiobutton':
                case 'checkbox':
                    $(this).val = false;
                    break;
            }

            var tag = this.tagName.toLowerCase();
            if (tag == 'select') {
                this.selectedIndex = -1;
                // alert("succe");
            };



        });

    };

  });