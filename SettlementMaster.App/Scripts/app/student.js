$(document).ready(function () {
     //alert("success");
    var col;
    var table2 = $('.datatable').DataTable(
    );


    var url = '/Student/';

    $("#FormStudent").validate({
        rules: {
            LastName: "required",
            FirstName: "required",
            CountryCode: "required",
            StateCode: "required",
            HomeAddress: "required",
            Gender: "required",
            CurrentClass: "required",
            DOB: "required",
            //Password: "required",
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
            LastName: {
                required: "Please Enter Last Name"

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
            var event;
            urlTemp = url + 'CreateStudent';
            if (btn == "Save") {


                postTemp = 'post';
                $('#ItbId').val(0);
                event = 'new';
            }
            else {


                //postTemp = 'put';
                postTemp = 'post';
                event = 'modify';
            };
             alert($('#FormStudent').serialize());
            var $reqLogin = {
                url: urlTemp,

                data: $('#FormStudent').serialize(),
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

                if (response.ResponseCode === 0) {


                    if (event == 'new') {
                        addGridItem(response.model);
                      

                        alert('Record Created Successfully');
                        $('#divDetail').fadeToggle();
                        $('#divGrid').fadeToggle();
                    }
                    else {
                        var btn = $('#btnSubmit').text('Save');
                        updateGridItem(response.model);
             
                        $('#pnlAudit').css('display', 'none');
                        alert('Record Updated successfully');
                        $('#divDetail').fadeToggle();
                        $('#divGrid').fadeToggle();

                    }
                    $('#ItbId').val(0);
                    $('#btnSubmit').removeAttr('disabled', 'disabled');
                    new Util().clearform('#FormSubject');
                }
                else
                {
                    $('#btnSubmit').removeAttr('disabled', 'disabled');
                    
                        alert(response.ResponseMessage);
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


    $("#FormParent").validate({
        rules: {
            LastName: "required",
            FirstName: "required",
            CountryCode: "required",
            StateCode: "required",
            HomeAddress: "required",
            Email: "required",
            Occupation: "required",
            MobilePhone: "required",
           
            //Password: "required",
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
            LastName: {
                required: "Please Enter Last Name"

            },
            //Email: {
            //    required: "Please Select an examination Type"

            //},

        },
        submitHandler: function (form) {
            //  e.preventDefault();
            //  alert('success');
            var btn = $('#btnSubmitParent').text();
            var urlTemp;
            var postTemp;
            var event;
            urlTemp = url + 'CreateParent';
            alert(btn);
            //if (btn == "Save") {
               

            //    postTemp = 'post';
               
            //    event = 'new';
            //}
            //else {


            //    //postTemp = 'put';
            //    postTemp = 'post';
            //    event = 'modify';
            //};
            $('#ParentId').val(0);
            alert($('#ParentId').val());
            postTemp = 'post';
            alert($('#FormParent').serialize());
            var $reqLogin = {
                url: urlTemp,

                data: $('#FormParent').serialize(),
                type: postTemp,
                contentType: "application/x-www-form-urlencoded"
            };
            // alert("after submit");
            $('#ajax-loading2').show();
            $('#btnSubmitParent').attr('disabled', 'disabled');
            var ajax = new Ajax();
            ajax.send($reqLogin).done(function (response) {
                $('#ajax-loading2').hide();
                //  $('#display-error').hide();
                //  var res = JSON.stringify(response);
                //alert(response);
                //console.log(res);

                if (response.ResponseCode === 0) {


                   
                      //  addGridItem(response.model);

                        $("#FormStudent #ParentId").append("<option value='" +  response.model.ParentId + "'>" +
                   response.model.ParentFullName + "</option>");
                        $("#FormStudent #ParentId").val(response.model.ParentId);
                        alert('Record Created Successfully');
                        //$('#divDetail').fadeToggle();
                        //$('#divGrid').fadeToggle();
                    
                   
                   // $('#ParentId').val(0);
                    $('#btnSubmitParent').removeAttr('disabled', 'disabled');
                   // $('#myModal').modal('hide');
                    alert('here');
                    $('#myModal').modal('hide');
                  //  new Util().clearform('#FormSubject');
                }
                else {
                    $('#btnSubmitParent').removeAttr('disabled', 'disabled');

                    alert(response.ResponseMessage);
                }
                //  $('#SubjectID').val('0');
                // updateLocalBank(response);

            }).fail(function (xhr, status, err) {
                //Disable Button
                // loginbtn.hide();
                //hide login loader
                $('#ajax-loading2').hide();
                $('#display-error').show();
                $('#errorMessage').text("No network connectivity. Please check your network connections.");
                // errorMessage
                // showPopDialog("No network connectivity. Please check your network connections.", handlerError);
            });


        }
    });



    $("#Table1").on("click", ".edit", editDetailServer);


    function editDetailServer() {


        var editLink = $(this).attr('data-key');
        col = $(this).parent();
        //  alert(editLink);
        $('#ItbId').val(editLink);
        // alert( $('#ItbId').val());
        var $reqLogin = {
            url: url + 'ViewStudent/' + editLink,

            data: null,
            type: "Get",
            contentType: "application/json"
        };
        //  alert($reqLogin.url);
        //$('#ajax-loading').show();
        var ajax = new Ajax();
        ajax.send($reqLogin).done(function (response) {
          //  alert(response.ResponseCode);
            if (response.ResponseCode === 0) {
                $('#LastName').val(response.model.LastName);
                $('#FirstName').val(response.model.FirstName);
                $('#MiddleName').val(response.model.MiddleName);
                $('#HomeAddress').val(response.model.HomeAddress);
                $('#StudentCode').val(response.model.StudentCode);
                $('#DOB').val(response.model.DOB);
                $('#Town').val(response.model.Town);
                $('#ItbId').val(response.model.ItbId);
                $('#Gender').val(response.model.Gender);
                $("#CountryCode").val(response.model.CountryCode);
                $("#StateCode").val(response.model.StateCode);
                $("#CurrentClass").val(response.model.CurrentClass);
                $("#CurrentClassArm").val(response.model.CurrentClassArm);
                $("#ParentId").val(response.model.ParentId);
                $('#CreatedBy').text(response.model.CreatedBy);
                $('#CreatedDate').text(response.model.DateString);
                $('#Status').text(response.model.Status);
                $('#pnlAudit').css('display', 'block');
                // $('').val(response.ExamName);
                $('#btnSubmit').text('Update');
                $('#divDetail').fadeToggle();
                $('#divGrid').fadeToggle();
            }
            else {
                alert(response.ResponseMessage)
            }
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

    function addGridItem(model) {
        // alert(model.TermCode);
        // var cur
        table2.row.add([
       model.StudentCode,
       model.LastName,
       model.OtherName,
       model.Gender == 'M' ? 'Male' : 'Female',
       model.PhoneNo,
       model.ClassName,
        //model.CurrentSession ? "Yes" : "No",
       model.Status,
        '<a id="edit" data-key="' + model.ItbId + '" class="edit btn btn-xs btn-primary"><i class="fa fa-edit"></i> Edit</a>' 
        ]).draw(false);



    }
    function updateGridItem(model) {
        // alert(model.TermCode);

        var rowIdx = table2
.cell(col)
.index().row;

        // alert(fg);
        //   alert(rowIdx);
        var d = table2.row(rowIdx).data();
        // alert(d[0]);
        d[1] = model.LastName;
        d[2] = model.OtherName;
        d[3] = model.Gender;
        d[4] = model.PhoneNo;
        d[5] = model.ClassName;
        d[6] = model.Status;
        // alert(d[0]);

        table2
     .row(rowIdx)
     .data(d)
     .draw();
    }

    $('#btnNew').click(function () {
        // formClear('#FormTerm');
        new Util().clearform('#FormStudent')
        $("#CountryCode").val('NGN');
      //  $("#StateCode").selectedIndex('NGN')
        $('#pnlAudit').css('display', 'none');
        $('#btnSubmit').text('Save');
       // $('#divDetail').css('display', 'block');
        $('#divDetail').fadeToggle();
        $('#divGrid').fadeToggle();
    });
    $('#lnkReturn').click(function (e) {
        e.preventDefault();
       // alert('suceess');
        $('#divDetail').fadeToggle();
        $('#divGrid').fadeToggle();
        $('#divGrid').focus();
    });
    var urlS = '/Setting/GetStates';
    // $("#StateId").prop("disabled", true);
    $("#FormParent #CountryCode").change(function () {
        alert('success');
        if ($(this).val() != "") {
            var options = {};
            options.url = urlS;
            options.type = "POST";
            options.data = JSON.stringify({ countryid: $(this).val() });
            options.dataType = "json";
            options.contentType = "application/json";
            options.success = function (states) {
                $("#FormParent #StateCode").empty();
                for (var i = 0; i < states.length; i++) {
                    $("#FormParent #StateCode").append("<option value='" + states[i].StateCode + "'>" +
                    states[i].StateName + "</option>");
                }
                $("#FormParent #StateCode").prop("disabled", false);
            };
            options.error = function () { alert("Error retrieving states!"); };
            $.ajax(options);
        }
        else {
            $("#FormParent #StateCode").empty();
            $("#FormParent #StateCode").prop("disabled", true);
        }
    });

    $('#pModal').click(function () {
       // alert('modal');
        $('#myModal').modal('show');
    });
});