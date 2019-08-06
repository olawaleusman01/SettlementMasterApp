$(document).ready(function () {
   /// GetRoles(1);

    var col;
    //var table2 = $('.datatable').DataTable(
    //);
    // var cnt = 0;
  
        var url = BaseUrl() + 'Roles/';
        var table2 = $('.datatable').DataTable({
            ajax: url + "RoleList",
            columns: [

                { data: "ROLENAME" },
                { data: "STATUS" },

                {
                    data: null,
                    className: "center_column",
                    //defaultContent: '<a class="btn btn-primary btn-xs editor_edit" data-key="' + data.RoleId + '"><i class="fa fa-edit"></i></a>' +
                    //    '  <a class="btn btn-warning btn-xs editor_remove" data-key="' + data.RoleId + '"><i class="fa fa-trash-o"></i></a>',
                    render: function (data, type, row) {
                        // Combine the first and last names into a single table field
                        var html = '<a class="btn btn-primary btn-xs editor_edit" data-key="' + data.ROLEID + '"><i class="fa fa-edit"></i></a>' +
                        '  <a class="btn btn-warning btn-xs editor_remove" href="' + url + 'RolePriviledge/' + data.ROLEID + '"><i class="fa fa-check"></i> Assign Priviledge</a>';
                        return html;
                    }
                }
            ]
        });

    

        function validateForm() {

            var validator = $("#formRoles").validate({
                rules: {
                    ROLENAME: "required",
                    ROLEBASE: "required",
                    DEPARRTMENT_CODE: "required"
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
                        $('#RoleId').val(0);
                        event = 'new';
                    }
                    else {

                        urlTemp = url + 'Create';
                        //postTemp = 'put';
                        postTemp = 'post';
                        event = 'modify';
                    };
                    //  alert(urlTemp);
                    var $reqLogin = {
                        url: urlTemp,

                        data: $('#formRoles').serialize(),
                        type: postTemp,
                        contentType: "application/x-www-form-urlencoded"
                    };
                    //console.log($('#formRoles').serialize());
                    //return;
                    loaderSpin2(true);
                    disableButton(true);
                    var ajax = new Ajax();
                    ajax.send($reqLogin).done(function (response) {
                        loaderSpin2(false);
                        disableButton(false);
                        if (response.RespCode === 0) {
                            new Util().clearform('#formRoles');

                            $('a.editor_return').click();
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

        }



    $("#example1").on("click", "a.editor_edit", editDetailServer);

    //$("#example1").on("click", ".current", setAsCurrent);

    function editDetailServer() {
          loaderSpin2(true);
        //  disableButton(true);
        var editLink = $(this).attr('data-key');
        col = $(this).parent();
        // alert(editLink);
        $('#RoleId').val(editLink);
        var menuId = $('#menuId').val();
       // alert(menuId);
        //  alert( $('#RoleId').val());
        var $reqLogin = {
            url: url + 'ViewRole/' + editLink  + '?m=' + menuId,

            data: null,
            type: "Get",
            contentType: "application/json"
        };
       // alert($reqLogin.url);
        loaderSpin2(true);
        $('.panel-form').load(
          $reqLogin.url, function () {
              loaderSpin2(false);
              // $('#myModalMsc').modal({ backdrop: 'static', keyboard: false });
              $('#pnlForm').fadeIn();
              $('#pnlGrid').fadeOut();
              // validator.resetForm();
              validateForm();
              $('a.editor_create').hide();
              $('a.editor_return').show();
          });
        //$('#ajax-loading').show();
        //////var ajax = new Ajax();
        //////ajax.send($reqLogin).done(function (response) {
        //////    loaderSpin2(false);
        //////    if (response.RespCode === 0) {

        //////        $('#RoleName').val(response.model.RoleName);
        //////        //alert(response.model.RoleId);
        //////        $('#RoleId').val(response.model.RoleId);
              
        //////        //  $('#UserName').attr('disabled','disabled');
        //////        $('#CreatedBy').text(response.model.CreatedBy);
        //////        $('#CreatedDate').text(response.model.DateString);
        //////        //  $('#Status').text(response.model.Status);
        //////        $('#pnlAudit').css('display', 'block');
        //////        $('a.editor_reset').hide();
        //////        // $('').val(response.ExamName);
        //////        $('#btnSave').html('<i class="fa fa-edit"></i> Update');
        //////        $('#btnSave').val(2);

        //////        $('#myModal').modal({ backdrop: 'static', keyboard: false });
        //////    }
        //////    else {
        //////        alert(response.RespMessage + 'edit error');
        //////    }
        //////}).fail(function (xhr, status, err) {
        //////    loaderSpin2(false);
        //////    $('#ajax-loading').hide();
        //////    $('#display-error').show();
        //////    $('#errorMessage').text("No network connectivity. Please check your network connections.");
        //////    // errorMessage
        //////    // showPopDialog("No network connectivity. Please check your network connections.", handlerError);
        //////});

    }

    function addGridItem(model) {
        table2.row.add(model).draw();
    }
    function updateGridItem(model) {
        var rowIdx = table2
    .cell(col)
    .index().row;

        var d = table2.row(rowIdx).data();
        d.RoleName = model.RoleName;
        d.Status = model.Status
       
        table2
     .row(rowIdx)
     .data(d)
     .draw();

    }

    //$('a.editor_create').click(function () {
    //    // formClear('#FormTerm');
    //    new Util().clearform('#formRoles');

    //    validator.resetForm();
    //    $('#pnlAudit').css('display', 'none');
    //    $('#RoleId').val(0);
    //    $('#btnSave').val(1);
    //    $('#btnSave').html('<i class="fa fa-save"></i> Save');
    //    // $('#UserName').removeAttr('disabled', 'disabled');
    //    $('a.editor_reset').show();
    //    $('#myModal').modal({ backdrop: 'static', keyboard: false });
    //});
    $(document).on('click', 'a.editor_create', function () {
        var menuId = $('#menuId').val();
       // alert(menuId);
        $('a.editor_reset').show();
        //$('#divStatus').hide();
        var urlP = url + 'ViewRole?m=' + menuId;
        //alert(urlP);
        loaderSpin2(true);
        $('.panel-form').load(
            urlP, function () {
                loaderSpin2(false);
                // $('#myModal').modal({ backdrop: 'static', keyboard: false });
                $('#pnlForm').fadeIn();
                $('#pnlGrid').fadeOut();
                // validator.resetForm();
                validateForm();
                $('a.editor_create').hide();
                $('a.editor_return').show();
            });

    });
    $(document).on('click','a.editor_reset',function () {
        // formClear('#FormTerm');
        new Util().clearform('#formUsers');

        //validator.resetForm();
        $('#pnlAudit').css('display', 'none');
        $('#ItbId').val(0);

    });
    $(document).on('click','a.editor_return',function () {
        $('a.editor_return').hide();
        $('a.editor_create').show();
        $('#pnlForm').fadeOut();
        $('#pnlGrid').fadeIn();
       // alert('am clicked');
    });
    $(document).on('change', '#ROLEBASE', function () {
        var selected = $(this).val();
        //alert(selected);
        if(selected == '1')
        {
            $('.divDept').show();
        }
        else {
            $('.divDept').hide();
        }
    })

   
});