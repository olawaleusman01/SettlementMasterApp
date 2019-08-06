$(document).ready(function () {

    var col;

   // var url = BaseUrl() + '/ItemType/';
    var url = BaseUrl() + 'ItemType/';
    var table2 = $('.datatable').DataTable({
        ajax: url + "ItemTypeList",
        columns: [

            { data: "Name" },
             { data: "Description" },
              //{ data: "Quantity" },
              {
                  data: null,
                  className: "center_column",
                  //defaultContent: '<a class="btn btn-primary btn-xs editor_edit" data-key="' + data.ItbId + '"><i class="fa fa-edit"></i></a>' +
                  //    '  <a class="btn btn-warning btn-xs editor_remove" data-key="' + data.ItbId + '"><i class="fa fa-trash-o"></i></a>',
                  render: function (data, type, row) {
                      // Combine the first and last names into a single table field
                      var html = '<span class="badge" style="background-color:red">' + data.Quantity + '</span>';
                      return html;
                  }
              },
            { data: "Status" },
            {
                data: null,
                className: "center_column",
                //defaultContent: '<a class="btn btn-primary btn-xs editor_edit" data-key="' + data.ItbId + '"><i class="fa fa-edit"></i></a>' +
                //    '  <a class="btn btn-warning btn-xs editor_remove" data-key="' + data.ItbId + '"><i class="fa fa-trash-o"></i></a>',
                render: function (data, type, row) {
                    // Combine the first and last names into a single table field
                    var html = '<a class="btn btn-primary btn-xs editor_edit" data-key="' + data.ItbId + '"><i class="fa fa-edit"></i></a>';
                    return html;
                }
            }
        ]
    });

    var validator = $("#formItemType").validate({
        rules: {
            Name: "required",
            //Amount: {
            //    required: true,
            //    number:true
            //}
        },
        messages: {

            // equalTo: "Password must be Equal",
            //Name: {
            //    required: "Please Enter Last Name"
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

                urlTemp = url + 'Create';
                postTemp = 'post';
                $('#ItbId').val(0);
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

                data: $('#formItemType').serialize(),
                type: postTemp,
                contentType: "application/x-www-form-urlencoded"
            };
            //alert($('#formItemType').serialize());
            loaderSpin(true);
            disableButton(true);
            var ajax = new Ajax();
            ajax.send($reqLogin).done(function (response) {
                loaderSpin(false);
                disableButton(false);
                if (response.RespCode === 0) {
                    new Util().clearform('#formItemType');

                    if (event == 'new') {
                        addGridItem(response.data);
                        $('#myModal').modal('hide');
                        $('#ItbId').val(0);
                        // alert('Record Created Successfully');
                        displayDialogNoty('Notification', 'Record Created Successfully');
                    }
                    else {
                        var btn = $('#btnSave').html('<i class="fa fa-save"></i> Save');
                        updateGridItem(response.data);
                        $('#myModal').modal('hide');
                        $('#ItbId').val(0);
                        // alert('Record Updated successfully');
                        displayDialogNoty('Notification', 'Record Updated Successfully');

                    }

                }
                else {
                    // alert(response.RespMessage)
                    displayModalNoty('alert-warning', response.RespMessage, true);

                }


            }).fail(function (xhr, status, err) {
                loaderSpin(false);
                disableButton(false);
                displayModalNoty('alert-warning', 'No network connectivity. Please check your network connections.', true);
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

       // alert( $('#ItbId').val());
        var $reqLogin = {
            url: url + 'ViewDetail/' + editLink,

            data: null,
            type: "Get",
            contentType: "application/json"
        };
        // alert($reqLogin.url);
        //$('#ajax-loading').show();
        var ajax = new Ajax();
        ajax.send($reqLogin).done(function (response) {
            loaderSpin2(false);
            if (response.RespCode === 0) {

                $('#Description').val(response.model.Description);
                $('#Name').val(response.model.Name);
               // $('#Qu').val(response.model.Name);
                //$('#Amount').val(response.model.Amount);
                //$('#RateType').val(response.model.RateType);
                //if (response.model.ApplyToAllStaff) {
                //    $('#ApplyToAllStaff').prop('checked', true);
                //    $('#ApplyToAllStaff').prop('value', 'true');
                //}
                //else
                //{
                //    $('#ApplyToAllStaff').prop('checked', false);
                //    $('#ApplyToAllStaff').prop('value', 'false');
                //}
                //$('#Deduction_Start').val(response.model.Deduction_Start);
                //$('#Deduction_End').val(response.model.Deduction_End);
                //alert(response.model.ItbId);
                $('#ItbId').val(response.model.ItbId);

                //  $('#UserName').attr('disabled','disabled');
                $('#CreatedBy').text(response.model.CreatedBy);
                $('#CreatedDate').text(response.model.DateString);
                $('#Status').val(response.model.Status);
                $('#pnlAudit').css('display', 'block');
                $('a.editor_reset').hide();
                // $('').val(response.ExamName);
                $('#btnSave').html('<i class="fa fa-edit"></i> Update');
                $('#btnSave').val(2);
                $('#divStatus').show();

                $('#myModal').modal({ backdrop: 'static', keyboard: false });
            }
            else {
                displayDialogNoty('Notification', response.RespMessage);
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
    $('#ApplyToAllStaff').on('click', function () {
        // alert('here');
        if ($('#ApplyToAllStaff').is(':checked')) {
            // alert('checked');
            $('#ApplyToAllStaff').prop('value', 'true');
        }
        else {
            //alert('not checked');
            $('#ApplyToAllStaff').prop('value', 'false');
        }
    });
    function addGridItem(model) {
        table2.row.add(model).draw();
    }
    function updateGridItem(model) {
        var rowIdx = table2
    .cell(col)
    .index().row;

        var d = table2.row(rowIdx).data();
        d.Description = model.Description;
        d.Name = model.Name;

        //d.AmountString = model.AmountString;
        //d.ApplyToAllStaff = model.ApplyToAllStaff;
        //d.RateType = model.RateType;
        d.Status = model.Status

        table2
     .row(rowIdx)
     .data(d)
     .draw();

    }

    $('a.editor_create').click(function () {
        // formClear('#FormTerm');
        new Util().clearform('#formItemType');

        validator.resetForm();
        $('#divStatus').hide();
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
        new Util().clearform('#formItemType');

        validator.resetForm();
        $('#pnlAudit').css('display', 'none');
        $('#ItbId').val(0);

    });
});