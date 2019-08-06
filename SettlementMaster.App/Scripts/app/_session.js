$(document).ready(function () {
    // alert("success");
    var col;
    var table2 = $('.datatable').DataTable(
    );
    

  

    var url = '/Setting/';

    $("#FormSession").validate({
        rules: {
            SessionCode: "required",
            // ExamType: "required",
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
            SessionCode: {
                required: "Please Enter Session Code"

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
            if (btn == "Save") {

                urlTemp = url + 'CreateSession';
                postTemp = 'post';
                $('#ItbId').val(0);
                event = 'new';
            }
            else {

                urlTemp = url + 'CreateSession';
                //postTemp = 'put';
                postTemp = 'post';
                event = 'modify';
            };
            //  alert(urlTemp);
            var $reqLogin = {
                url: urlTemp,

                data: $('#FormSession').serialize(),
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
               // alert(response.ResponseCode);
                //console.log(res);

                if (response.ResponseCode === 0) {
                    new Util().clearform('#FormSession');

                    if (event == 'new') {
                        addGridItem(response.model);
                        $('#btnSubmit').removeAttr('disabled', 'disabled');

                        alert('Record Created Successfully');
                    }
                    else {
                        var btn = $('#btnSubmit').text('Save');
                        updateGridItem(response.model);
                        $('#btnSubmit').removeAttr('disabled', 'disabled');
                        $('#pnlAudit').css('display', 'none');
                        alert('Record Updated successfully');

                    }
                    $('#Itbid').val(0);
                };
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



   

    $("#Table1").on("click", ".edit", editDetailServer);

    $("#Table1").on("click", ".current", setAsCurrent);
    function setAsCurrent() {
        // alert('success');
        var editLink = $(this).attr('data-key');
       //  alert(editLink);
        //$('#Itbid').val(editLink);

        var $reqLogin = {
            url: url + 'SetCurrentSession/' + editLink,

            data: $('#FormCurrent').serialize(),
            type: 'post',
            contentType: "application/x-www-form-urlencoded"
        };
       // alert($reqLogin.url);
        // $('#ajax-loading').show();
        var ajax = new Ajax();
        ajax.send($reqLogin).done(function (response) {
            //  alert('sss');
            //  alert(response);
            // $('#grid').html(response);
            //    $("#grid").load(url + "TermGridPartial",
            //null);
            //table2 = $('.datatable').DataTable({
            //    "ajax": response.record,
            //    "columns": [
            //        {}
            //    ]
            //});
            //alert(response);
            if (response.ResponseCode === 0)
            {
                alert('Record Set as Current Successfully')
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
    function editDetailServer() {


        var editLink = $(this).attr('data-key');
        col = $(this).parent();
        //  alert(editLink);
        $('#Itbid').val(editLink);
        //  alert( $('#ItbId').val());
        var $reqLogin = {
            url: url + 'ViewSession/' + editLink,

            data: null,
            type: "Get",
            contentType: "application/json"
        };
        //  alert($reqLogin.url);
         //$('#ajax-loading').show();
        var ajax = new Ajax();
        ajax.send($reqLogin).done(function (response) {
          //  alert(response.model);
            if (response.ResponseCode === 0) {
               
                $('#SessionCode').val(response.model.SessionCode);
                $('#Itbid').val(response.model.Itbid);
                $('#CreatedBy').text(response.model.CreatedBy);
                $('#CreatedDate').text(response.model.DateString);
                $('#Status').text(response.model.Status);
                $('#pnlAudit').css('display', 'block');
                // $('').val(response.ExamName);
                $('#btnSubmit').text('Update');
            }
            else
            {
                alert(response.ResponseMessage);
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
        alert(model.SessionCode);
        // var cur
        table2.row.add([
       model.SessionCode,
        model.CurrentSession ? "Yes" : "No",
       model.Status,
        '<a id="edit" data-key="' + model.Itbid + '" class="edit btn btn-xs btn-primary"><i class="fa fa-edit"></i> Edit</a>' +
                       '<form id="FormCurrent" class="inline" method="post">' +

                            '<a id="current" data-key="' + model.Itbid + '" class="current btn btn-xs btn-success"><i class="fa fa-info"></i> Set as Current</a> </form>'
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
        d[0] = model.SessionCode;
       //  alert(d[0]);

        table2
     .row(rowIdx)
     .data(d)
     .draw();
    }
    
    $('#btnNew').click(function () {
       // formClear('#FormTerm');
        new Util().clearform('#FormSession');
        $('#pnlAudit').css('display', 'none');
        $('#btnSubmit').text('Save');
    });
});