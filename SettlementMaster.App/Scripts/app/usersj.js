$(document).ready(function () {
    // alert("success");
    //startProcessing();
    //setTimeout(function () {
    //    endProcessing();
    //}, 10000);
  
    var col;
    //var table2 = $('.datatable').DataTable(
    //);
   // var cnt = 0;
    var url = BaseUrl()  + '/Account/';
    var table2 = $('.datatable').DataTable({
        ajax: url + "UserList",
        columns: [

            //{
            //    data: null,
            //    className: "center_column",
            //    defaultContent: 1,
            //    //render: function (data, type, row) {
            //    //    // Combine the first and last names into a single table field
                  
            //    //    return 1;
            //    //}
            //},
            { data: "UserName" },
            { data: "FullName" },
            { data: "Email" },
            { data: "Status" },

            {
                data: null,
                className: "center_column",
                //defaultContent: '<a class="btn btn-primary btn-xs editor_edit" data-key="' + data.ItbId + '"><i class="fa fa-edit"></i></a>' +
                //    '  <a class="btn btn-warning btn-xs editor_remove" data-key="' + data.ItbId + '"><i class="fa fa-trash-o"></i></a>',
                render: function (data, type, row) {
                    // Combine the first and last names into a single table field
                    var html = '<a class="btn btn-primary btn-xs editor_edit" data-key="' + data.ItbId + '"><i class="fa fa-edit"></i></a>' +
                    '  <a class="btn btn-warning btn-xs editor_remove" data-key="' + data.ItbId + '"><i class="fa fa-times"></i></a>' ;
                    return html;
                }
            }
        ]
    });





   var validator = $("#formUsers").validate({
        rules: {
            LastName: "required",
            FirstName: "required",
            Email: "required",
            RoleId: "required",
            UserName: "required",
        },
        messages: {

            // equalTo: "Password must be Equal",
            LastName: {
                required: "Please Enter Last Name"
            },
            UserName: {
                required: "Please Enter Login Id"
            },
            FirstName: {
                required: "Please Enter First Name"
            },
            Email:
                 {
                     email: "Please enter a valid Email-Address",
                     required: "Please Enter E-mail"
                     // email:true
                 },
            RoleId: {
                required: "Please Select Role"
            },
            //Email: {
            //    required: "Please Select an examination Type"

            //},

        },
        submitHandler: function () {
            //  e.preventDefault();
            // alert('success');
            var btn = $('#btnSave').val();
            var urlTemp;
            var postTemp;
            var event;
            if (btn == "1") {

                urlTemp = url + 'Register';
                postTemp = 'post';
                //$('#ItbId').val(0);
                event = 'new';
            }
            else {

                urlTemp = url + 'Register';
                //postTemp = 'put';
                postTemp = 'post';
                event = 'modify';
            };
            //  alert(urlTemp);
            var $reqLogin = {
                url: urlTemp,

                data: $('#formUsers').serialize(),
                type: postTemp,
                contentType: "application/x-www-form-urlencoded"
            };
           // alert($('#formUsers').serialize());
            loaderSpin(true);
            disableButton(true);
            var ajax = new Ajax();
            ajax.send($reqLogin).done(function (response) {
                loaderSpin(false);
                disableButton(false);
                if (response.RespCode === 0) {
                    new Util().clearform('#formUsers');

                    if (event == 'new') {
                        addGridItem(response.data);
                        $('#myModal').modal('hide');
                        //$('#ItbId').val(0);
                        // alert('Record Created Successfully');
                        displayNoty('alert-success', 'Record Created Successfully');
                    }
                    else {
                        var btn = $('#btnSave').html('<i class="fa fa-save"></i> Save');
                        updateGridItem(response.data);
                        $('#myModal').modal('hide');
                        //$('#ItbId').val(0);
                        // alert('Record Updated successfully');
                        displayNoty('alert-success', 'Record Updated Successfully');

                    }

                }
                else {
                   // alert(response.RespMessage)
                  displayModalNoty('alert-warning', response.RespMessage,true);

                }


            }).fail(function (xhr, status, err) {
                loaderSpin(false);
                disableButton(false);
                displayModalNoty('alert-warning', 'No network connectivity. Please check your network connections.',true);
                //$('#display-error').show();
                //$('#errorMessage').text("No network connectivity. Please check your network connections.");
                // errorMessage
                // showPopDialog("No network connectivity. Please check your network connections.", handlerError);
            });


        }
    });
    $("#example1").on("click", "a.editor_edit", editDetailServer);

    //$("#example1").on("click", ".current", setAsCurrent);
   
    function editDetailServer() {
          loaderSpin2(true);
        //  disableButton(true);
        var editLink = $(this).attr('data-key');
        col = $(this).parent();
       // alert(editLink);
        $('#ItbId').val(editLink);

        //  alert( $('#ItbId').val());
        var $reqLogin = {
            url: url + 'ViewUser/' + editLink,

            data: null,
            type: "Get",
            contentType: "application/json"
        };
        // alert($reqLogin.url);
        //$('#ajax-loading').show();
        var ajax = new Ajax();
        ajax.send($reqLogin).done(function (response) {
            //  alert(response.model);
               loaderSpin2(false);
            if (response.RespCode === 0) {

                $('#LastName').val(response.model.LastName);
                $('#FirstName').val(response.model.FirstName);
                $('#Email').val(response.model.Email);
                $('#MobileNo').val(response.model.MobileNo);
                $('#Email').val(response.model.Email);
                $('#RoleId').val(response.model.RoleId);
                $('#ItbId').val(response.model.ItbId);
                $('#UserName').val(response.model.UserName);
              //  $('#UserName').attr('disabled','disabled');
                $('#CreatedBy').text(response.model.CreatedBy);
                $('#CreatedDate').text(response.model.DateString);
              //  $('#Status').text(response.model.Status);
                $('#pnlAudit').css('display', 'block');
                $('a.editor_reset').hide();
                // $('').val(response.ExamName);
                $('#btnSave').html('<i class="fa fa-edit"></i> Update');
                $('#btnSave').val(2);

                $('#myModal').modal({ backdrop: 'static', keyboard: false });
            }
            else {
                alert(response.RespMessage + 'edit error');
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
        table2.row.add(model).draw();
    }
    function updateGridItem(model) {
        var rowIdx = table2
    .cell(col)
    .index().row;

        var d = table2.row(rowIdx).data();
        d.UserName = model.UserName;
        d.Status = model.Status
        d.Email = model.Email
        d.FullName = model.FullName
        table2
     .row(rowIdx)
     .data(d)
     .draw();

    }

    $('a.editor_create').click(function () {
        // formClear('#FormTerm');
        new Util().clearform('#formUsers');
      
        validator.resetForm();
        $('#pnlAudit').css('display', 'none');
        $('#ItbId').val(0);
        $('#btnSave').val(1);
        $('#btnSave').html('<i class="fa fa-save"></i> Save');
       // $('#UserName').removeAttr('disabled', 'disabled');
        $('a.editor_reset').show();
        $('#myModal').modal({ backdrop: 'static', keyboard: false });
    });
    $('a.editor_reset').click(function () {
        // formClear('#FormTerm');
        new Util().clearform('#formUsers');

        validator.resetForm();
        $('#pnlAudit').css('display', 'none');
        $('#ItbId').val(0);
      
    });
});