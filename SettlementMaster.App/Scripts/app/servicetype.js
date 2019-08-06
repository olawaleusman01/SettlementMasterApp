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
    var url = BaseUrl() + 'ServiceType/';
    var table2 = $('.datatable').DataTable({
        ajax: url + "ServiceTypeList",
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
            { data: "ServiceId" },
            { data: "ServiceName" },
            { data: "ChargeAmount" },
            //{ data: "ChargeGlAccount" },
            { data: "Status" },
            {
                data: null,
                className: "center_column",
                //defaultContent: '<a class="btn btn-primary btn-xs editor_edit" data-key="' + data.ServiceId + '"><i class="fa fa-edit"></i></a>' +
                //    '  <a class="btn btn-warning btn-xs editor_remove" data-key="' + data.ServiceId + '"><i class="fa fa-trash-o"></i></a>',
                render: function (data, type, row) {
                    // Combine the first and last names into a single table field
                    var html = '<a class="btn btn-primary btn-xs editor_edit" data-key="' + data.ServiceId + '"><i class="fa fa-edit"></i></a>' 

                    return html;
                }
            }
        ]
    });





    var validator = $("#formServiceType").validate({
        rules: {
            ServiceId: "required",
            ServiceName: "required",
            //ChargeAmount: "required",
           // ChargeGlAccount: "required",
        },
        //messages: {

        //    // equalTo: "Password must be Equal",
        //    Name: {
        //        required: "Please Enter Last Name"
        //    },


        //},
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
                $('#ServiceId').val(0);
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

                data: $('#formServiceType').serialize(),
                type: postTemp,
                contentType: "application/x-www-form-urlencoded"
            };
            //alert($('#formServiceType').serialize());
            loaderSpin(true);
            disableButton(true);
            var ajax = new Ajax();
            ajax.send($reqLogin).done(function (response) {
                loaderSpin(false);
                disableButton(false);
                if (response.RespCode === 0) {
                    new Util().clearform('#formServiceType');

                    if (event == 'new') {
                        addGridItem(response.data);
                        $('#myModal').modal('hide');
                        $('#ServiceId').val(0);
                        // alert('Record Created Successfully');
                        displayNoty('alert-success', 'Record Created Successfully');
                    }
                    else {
                        var btn = $('#btnSave').html('<i class="fa fa-save"></i> Save');
                        updateGridItem(response.data);
                        $('#myModal').modal('hide');
                        $('#ServiceId').val(0);
                        // alert('Record Updated successfully');
                        displayNoty('alert-success', 'Record Updated Successfully');

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
        //  loaderSpin(true);
        //  disableButton(true);
        var editLink = $(this).attr('data-key');
        col = $(this).parent();
        // alert(editLink);
        $('#ServiceId').val(editLink);

        //  alert( $('#ServiceId').val());
        var $reqLogin = {
            url: url + 'ViewServiceType/' + editLink,

            data: null,
            type: "Get",
            contentType: "application/json"
        };
        // alert($reqLogin.url);
        //$('#ajax-loading').show();
        var ajax = new Ajax();
        ajax.send($reqLogin).done(function (response) {
            //  alert(response.model);
            if (response.RespCode === 0) {

                $('#Name').val(response.model.Name);
                //alert(response.model.ServiceId);
                $('#ServiceId').val(response.model.ServiceId);
                $('#ServiceName').val(response.model.ServiceName);
                $('#ChargeAmount').val(response.model.ChargeAmount);
                $('#ChargeGlAccount').val(response.model.ChargeGlAccount);

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
        d.ServiceTypeId = model.ServiceTypeId;
        d.ServiceName = model.ServiceName
        d.ChargeAmount = model.ChargeAmount;
        d.ChargeGlAccount = model.ChargeGlAccount;
        d.Status = model.Status

        table2
     .row(rowIdx)
     .data(d)
     .draw();

    }

    $('a.editor_create').click(function () {
        // formClear('#FormTerm');
        new Util().clearform('#formServiceType');

        validator.resetForm();
        $('#pnlAudit').css('display', 'none');
        $('#ServiceId').val(0);
        $('#btnSave').val(1);
        $('#btnSave').html('<i class="fa fa-save"></i> Save');
        // $('#UserName').removeAttr('disabled', 'disabled');
        $('a.editor_reset').show();
        $('#myModal').modal({ backdrop: 'static', keyboard: false });
    });
    $('a.editor_reset').click(function () {
        // formClear('#FormTerm');
        new Util().clearform('#formServiceType');

        validator.resetForm();
        $('#pnlAudit').css('display', 'none');
        $('#ServiceId').val(0);

    });
});