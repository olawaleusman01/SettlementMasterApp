$(document).ready(function () {

    var col;

    // var url = BaseUrl() + '/ItemType/';
    var url = BaseUrl() + 'Location/';
    var url2 = BaseUrl() + 'Settings/';
    BindCombo();
    var table2 = $('.datatable').DataTable({
        ajax: url + "LocationList",
        columns: [
            { data: "LocationNameFull" },
            { data: "LocationNameShort" },
            { data: "ParentLocation" },
            //{ data: "Status" },
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

    var validator = $("#formLocation").validate({
        rules: {
            LocationNameFull: "required",
            UnitShortCode: "required",
            SellingPriceMargin: {
               // required: true,
                number:true
            },
            Email: {
        // required: true,
        email:true
    }
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

                data: $('#formLocation').serialize(),
                type: postTemp,
                contentType: "application/x-www-form-urlencoded"
            };
            //alert($('#formLocation').serialize());
            loaderSpin(true);
            disableButton(true);
            var ajax = new Ajax();
            ajax.send($reqLogin).done(function (response) {
                loaderSpin(false);
                disableButton(false);
                if (response.RespCode === 0) {
                    new Util().clearform('#formLocation');
                   
                    $("#CountryCode").val('NG');
                    $('.select2').trigger('change');
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
                //alert(response.model.LocationAddress);
                $('#LocationNameFull').val(response.model.LocationNameFull);
                $('#LocationNameShort').val(response.model.LocationNameShort);
                $('#LocationAddress').val(response.model.LocationAddress);
                $('#City').val(response.model.City);
                $('#ParentLocation_ItbId').val(response.model.ParentLocation_ItbId);
                $('#CountryCode').val(response.model.CountryCode);
                //alert(response.model.StateCode);
                $('#StateCode').val(response.model.StateCode);
                $('.select2').trigger('change');
                $('#ContactName').val(response.model.ContactName);
                $('#Email').val(response.model.Email);
                $('#MobileNo1').val(response.model.MobileNo1);
                $('#SellingPriceMargin').val(response.model.SellingPriceMargin);
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
        d.LocationNameFull = model.LocationNameFull;
        d.LocationNameShort = model.LocationNameShort;
        d.ParentLocation = model.ParentLocation;
       // d.Status = model.Status

        table2
     .row(rowIdx)
     .data(d)
     .draw();

    }

    $('a.editor_create').click(function () {
        // formClear('#FormTerm');
        new Util().clearform('#formLocation');
        $("#CountryCode").val('NG');
        $('.select2').trigger('change');
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
        new Util().clearform('#formLocation');

        validator.resetForm();
        $('#pnlAudit').css('display', 'none');
        $('#ItbId').val(0);

    });

     function BindCombo() {
      
        //  Bind Country
        var $reqLogin = {
            url: url2 + 'GetCountry',
            data: null,
            type: "Get",
            contentType: "application/json"
        };

        var ajax = new Ajax();
        ajax.send($reqLogin).done(function (response) {

            var exist = false;
            if (response.data.length > 0) {
                $("#CountryCode").empty();
              
                $("#CountryCode").append("<option value=''> --Select a Country-- </option>");
              //  $("#Kin_Country").append("<option value=''> --Select a Country-- </option>");
               // $("#formRef #Country").append("<option value=''> --Select a Country-- </option>");

                for (var i = 0; i < response.data.length; i++) {
                    $("#CountryCode").append("<option value='" + response.data[i].CountryCode + "'>" +
                    response.data[i].CountryName + "</option>");
                
                }
                $("#CountryCode").val('NG');
               
            }
            // return response.data;
        }).fail(function (xhr, status, err) {
            return null;

        });

        //  Bind Level
        var $reqLogin = {
            url: url2 + 'GetState',
            data: null,
            type: "Get",
            contentType: "application/json"
        };

        var ajax = new Ajax();
        ajax.send($reqLogin).done(function (response) {

            var exist = false;
            if (response.data.length > 0) {
                $("#StateCode").empty();
              
              
                $("#StateCode").append("<option value=''> --Select a State-- </option>");
               
                for (var i = 0; i < response.data.length; i++) {
                    $("#StateCode").append("<option value='" + response.data[i].StateCode + "'>" +
                    response.data[i].StateName + "</option>");
                }
            }
            // return response.data;
        }).fail(function (xhr, status, err) {
            return null;

        });
         try
         {
             //  Bind Level
             var $reqLogin = {
                 url: url2 + 'ParentLocation',
                 data: null,
                 type: "Get",
                 contentType: "application/json"
             };
             $("#ParentLocation_ItbId").empty();
             var ajax = new Ajax();
             ajax.send($reqLogin).done(function (response) {

                 var exist = false;
                 if (response.data.length > 0) {
                     $("#ParentLocation_ItbId").empty();
                     $("#ParentLocation_ItbId").append("<option value=''> --Select Parent Location-- </option>");
               
                     for (var i = 0; i < response.data.length; i++) {
                         $("#ParentLocation_ItbId").append("<option value='" + response.data[i].Code + "'>" +
                         response.data[i].Description + "</option>");
                     }
                 }
                 // return response.data;
             }).fail(function (xhr, status, err) {
                 return null;
             });
         }
         catch(err)
         {

         }
    }
});