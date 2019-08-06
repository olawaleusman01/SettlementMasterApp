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
    var url = BaseUrl() + 'Account/';
    GetRoles();
    BindCombo();
    var table2 = $('.datatable').DataTable({
        ajax: url + "UserList",
        columns: [
            { data: "UserName" },
            { data: "FullName" },
            { data: "Email" },
            { data: "INSTITUTION_NAME" },
            { data: "Status" },

            {
                data: null,
                className: "center_column",
                //defaultContent: '<a class="btn btn-primary btn-xs editor_edit" data-key="' + data.ItbId + '"><i class="fa fa-edit"></i></a>' +
                //    '  <a class="btn btn-warning btn-xs editor_remove" data-key="' + data.ItbId + '"><i class="fa fa-trash-o"></i></a>',
                render: function (data, type, row) {
                    // Combine the first and last names into a single table field
                    var html = '<a class="btn btn-primary btn-xs editor_edit" data-key="' + data.ItbId + '"><i class="fa fa-edit"></i></a>';
                    //'  <a class="btn btn-warning btn-xs editor_remove" data-key="' + data.ItbId + '"><i class="fa fa-times"></i></a>' ;
                    return html;
                }
            }
        ]
    });
    validated = -1; validatedUser = '';

    function validateForm() {
        var validator = $("#formUsers").validate({
            rules: {
               // RoleId: "required",
                UserName: "required",
                FullName: "required",
                InstitutionId: "required",
                LastName: "required",
                FirstName: "required",
                RoleId: "required",
                DeptCode: "required",
                Email: {
                    email: true,
                    required: true,
                }
            },
            messages: {
                UserName: {
                    required: "Please Enter Login Id"
                },
                RoleId: {
                    required: "Please Select Role"
                },
            },
            submitHandler: function () {
                var userName = $.trim($('#UserName').val());

                var btn = $('#btnSave').val();
                var urlTemp;
                var postTemp;
                var event;
                if (btn == "1") {
                    //if (validated !== 0 && validatedUser !== userName) {
                    //    displayModalNoty('alert-warning', 'Please Enter UserName and Click Validate to get user details.');
                    //    return;
                    //}
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
                //console.log($('#formUsers').serialize());
                //return;
                loaderSpin2(true);
                disableButton(true);
                var ajax = new Ajax();
                ajax.send($reqLogin).done(function (response) {
                    loaderSpin2(false);
                    disableButton(false);
                    if (response.RespCode === 0) {
                        new Util().clearform('#formUsers');
                        $('a.editor_return').click();
                            //$('#ItbId').val(0);
                            // alert('Record Created Successfully');
                            displayDialogNoty('Notification', response.RespMessage);
                    }
                    else {
                        // alert(response.RespMessage)
                        displayDialogNoty('Notification', response.RespMessage);

                    }


                }).fail(function (xhr, status, err) {
                    loaderSpin2(false);
                    disableButton(false);
                    displayDialogNoty('alert-warning', 'No network connectivity. Please check your network connections.', true);
                    //$('#display-error').show();
                    //$('#errorMessage').text("No network connectivity. Please check your network connections.");
                    // errorMessage
                    // showPopDialog("No network connectivity. Please check your network connections.", handlerError);
                });


            }
        });

    }

  
    $("#example1").on("click", "a.editor_edit", editDetailServer);

    //$("#example1").on("click", ".current", setAsCurrent);
   
    function editDetailServer() {
          loaderSpin2(true);
        //  disableButton(true);
        var editLink = $(this).attr('data-key');
        col = $(this).parent();
       // alert(editLink);
        //$('#ItbId').val(editLink);

         //alert( $('#ItbId').val());
        var $reqLogin = {
            url: url + 'Add/' + editLink,

            data: null,
            type: "Get",
            contentType: "application/json"
        };
        console.log($reqLogin.url);
        loaderSpin2(true);
        $('.panel-form').load(
          $reqLogin.url, function () {
              loaderSpin2(false);
              // $('#myModalMsc').modal({ backdrop: 'static', keyboard: false });
              $('#pnlForm').fadeIn();
              $('#pnlGrid').fadeOut();
              // validator.resetForm();
              validateForm();
             // alert($('#ItbId').val());
              $('a.editor_create').hide();
              $('a.editor_return').show();
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
        d.Status = model.Status;
        d.Email = model.Email;
        d.FullName = model.FullName;
        d.LocationName = model.LocationName;
        table2
     .row(rowIdx)
     .data(d)
     .draw();

    }
    $('#btnSearch').on('click', function (e) {
        e.preventDefault();
        GetUserDetailFromWebService();
    });
    $('a.editor_create').click(function () {
        var menuId = $('#menuId').val();
       // $('a.editor_reset').show();
        //$('#divStatus').hide();
        var urlP = url + 'Add?m=' + menuId;
        console.log(urlP);
        loaderSpin2(true);
        $('.panel-form').load(
            urlP, function () {
                loaderSpin2(false);
                // $('#myModal').modal({ backdrop: 'static', keyboard: false });
                $('#pnlForm').fadeIn();
                $('#pnlGrid').fadeOut();
                // validator.resetForm();
                
                validateForm();
               // alert($('#ItbId').val());
                $('a.editor_create').hide();
                $('a.editor_return').show();
            });
     
    });
    $('a.editor_reset').click(function () {
        // formClear('#FormTerm');
        new Util().clearform('#formUsers');

        //validator.resetForm();
        $('#pnlAudit').css('display', 'none');
        $('#ItbId').val(0);
      
    });
    $('a.editor_return').click(function () {
        $(this).hide();
        $('a.editor_create').show();
        $('#pnlForm').fadeOut();
        $('#pnlGrid').fadeIn();

    });
    
    $(document).on('change','#RoleId', function () {
        $('#RoleName').val($('#RoleId option:selected').val());
    })
    function BindCombo() {
        try
        {
            var $reqLogin = {
                url: BaseUrl() + 'Settings/GetLocation',

                data: null,
                type: "Get",
                contentType: "application/json"
            };

            var ajax = new Ajax();
            ajax.send($reqLogin).done(function (response) {
                if (response.data.length > 0) {
                    $("#LocationItbId").empty();
                    $("#LocationItbId").append("<option value=''> --Select Location-- </option>");
                    for (var i = 0; i < response.data.length; i++) {
                        $("#LocationItbId").append("<option value='" + response.data[i].ItbId + "'>" +
                        response.data[i].LocationNameFull + "</option>");
                    }
                }
            }).fail(function (xhr, status, err) {
                return null;
            });
        }
        catch(err)
        {}
    }
    $(document).on('change', '#InstitutionId', function () {
        if ($(this).val() === "1") {
            $('#divDept').show();
            $("#RoleId").empty();
            $("#RoleId").append("<option value=''> --Select Role-- </option>");
            $("#RoleId").val('').trigger('change');
            $("#DeptCode").val('').trigger('change');
        }
        else {
            bindRole('');
            $('#divDept').hide();
        }
    })
    $(document).on('change', '#DeptCode', function () {
        var selected = $(this).val();
      
        if (selected !== "") {
            bindRole(selected);
        }
        else {
            //$('#divDept').hide();
        }
    })

    function bindRole(deptCode) {
        $("#RoleId").empty();
        $("#RoleId").append("<option value=''> --Select Role-- </option>");
        var $reqLogin = {
            url: url + 'GetRole/' + deptCode,
            data: null,
            type: "Get",
            contentType: "application/json"
        };

        var ajax = new Ajax();
        ajax.send($reqLogin).done(function (response) {
            if (response.data.length > 0) {
                for (var i = 0; i < response.data.length; i++) {
                    $("#RoleId").append("<option value='" + response.data[i].ROLEID + "'>" +
                    response.data[i].ROLENAME + "</option>");
                }
            }
        }).fail(function (xhr, status, err) {
            return null;
        });
    }
    $(document).on('change', '#LastName,#FirstName', function () {
        var key = $('#ItbId').val();
        if (key <= 0) {
            var lastName = $('#LastName').val();
            var firstName = $('#FirstName').val();
            //alert(firstName + '.' + lastName);
            bindUserName(lastName, firstName);
           // $('#UserName').val(firstName + '.' + lastName);
        }
    })

    function bindUserName(lastName,firstName) {
       
        var $reqLogin = {
            url: url + 'GetUserName?lastName=' + lastName + '&firstName=' + firstName,
            data: null,
            type: "Get",
            contentType: "application/json"
        };
        console.log($reqLogin.url);
        var ajax = new Ajax();
        ajax.send($reqLogin).done(function (response) {
            if (response.RespCode == 0) {
                $('#UserName').val(response.UserName);
            }
        }).fail(function (xhr, status, err) {
            return null;
        });
    }
});