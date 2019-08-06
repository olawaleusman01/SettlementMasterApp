$(document).ready(function () {
    // alert("success");
    var col;
    var table2 = $('.datatable').DataTable(
    );
    //$('#Table1 tbody').on('click', 'tr', function () {
    //    alert(this);
    //    var d = table2.row(this).data();
    //    d[0] = 'fifth fifth term';
    //    alert(d[0]);
    //    table2
    //   .row(0)
    //   .data(d)
    //   .draw();
    //    //console.log();
    //});
 
    var examTypeArray = [];
    //var exanTypeObj = new Object;

    var url = '/Setting/';
  
    $("#FormTerm").validate({
        rules: {
            TermCode: "required",
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
            TermCode: {
                required: "Please Enter a Term Code"

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
                 
                urlTemp = url + 'CreateTerm';
                postTemp = 'post';
                $('#ItbId').val(0);
                event = 'new';
            }
            else {
       
                urlTemp = url + 'CreateTerm';
                //postTemp = 'put';
                postTemp = 'post';
                event = 'modify';
            };
           //  alert(urlTemp);
            var $reqLogin = {
                url: urlTemp,
               
                data: $('#FormTerm').serialize(),
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
                    formClear('#FormTerm');
                  
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
                    $('#ItbId').val(0);
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


   
    function BindGrid() {
        $("#Table1 tbody").empty();
        for (var i = 0; i < examTypeArray.length; i++) {
            //  alert(examTypeArray[i].ExamName);
            // examTypeArray.push(response[i]);

            $("#Table1").append("<tr><td>" + examTypeArray[i].SubjectName + "</td>   <td> <a  id='edit' data-key='" + examTypeArray[i].SubjectID + "'  class='edit'><i class='fa-times fa'></i> Edit | </a>  <a href='#' class='delete'><i class='fa-times fa'></i> Delete </a></td></tr>");


            // }
            //else {
            //    alert("Only image files are allowed. Other files will be ignored!");
            //}
        };


    }

     $("#Table1").on("click", ".edit", editDetailServer);
    //$('#Table1 tbody').on('click', '.edit', function () {
    //    alert('here');
    //    var txtword = $(this).text();
    //    // var colnum = parseInt($(this).index());
    //     col = $(this).parent();
    //    var colnum = parseInt(col.index());
   
    //    if (colnum === 3) {
    //            var editLink = $(this).attr('data-key');
    //             alert(editLink);
    //             $('#ItbId').val(editLink);
    //           //  alert( $('#ItbId').val());
    //            var $reqLogin = {
    //                url: url + 'ViewTerm/' + editLink,

    //                data: null,
    //                type: "Get",
    //                contentType: "application/json"
    //            };
    //            //  alert($reqLogin.url);
    //            // $('#ajax-loading').show();
    //            var ajax = new Ajax();
    //            ajax.send($reqLogin).done(function (response) {

    //                if (response.ResponseCode === 0) {
    //                    $('#TermCode').val(response.model.TermCode);
    //                    $('#ItbId').val(response.model.ItbId);
    //                    $('#CreatedBy').text(response.model.CreatedBy);
    //                    $('#CreatedDate').text(response.model.CreatedDate);
    //                    $('#Status').text(response.model.Status);
    //                    $('#pnlAudit').css('display', 'block');
    //                    // $('').val(response.ExamName);
    //                    $('#btnSubmit').text('Update');
    //                };
    //            }).fail(function (xhr, status, err) {
    //                //Disable Button
    //                // loginbtn.hide();
    //                //hide login loader
    //                $('#ajax-loading').hide();
    //                $('#display-error').show();
    //                $('#errorMessage').text("No network connectivity. Please check your network connections.");
    //                // errorMessage
    //                // showPopDialog("No network connectivity. Please check your network connections.", handlerError);
    //            });
          
    //    }
    //});
    $("#Table1").on("click", ".current", setAsCurrent);
    function setAsCurrent() {
       // alert('success');
        var editLink = $(this).attr('data-key');
       // alert(editLink);
        $('#ItbId').val(editLink);
     
        var $reqLogin = {
            url: url + 'SetCurrentTerm/' + editLink,
   
            data: $('#FormCurrent').serialize(),
            type: 'post',
            contentType: "application/x-www-form-urlencoded"
        };
   
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
         $('#ItbId').val(editLink);
       //  alert( $('#ItbId').val());
        var $reqLogin = {
            url: url + 'ViewTerm/' + editLink,
     
            data: null,
            type: "Get",
            contentType: "application/json"
        };
        //  alert($reqLogin.url);
        // $('#ajax-loading').show();
        var ajax = new Ajax();
        ajax.send($reqLogin).done(function (response) {
           
            if (response.ResponseCode === 0) {
                $('#TermCode').val(response.model.TermCode);
                $('#ItbId').val(response.model.ItbId);
                $('#CreatedBy').text(response.model.CreatedBy);
                $('#CreatedDate').text(response.model.DateString);
                $('#Status').text(response.model.Status);
                $('#pnlAudit').css('display', 'block');
                // $('').val(response.ExamName);
                $('#btnSubmit').text('Update');
            };
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

    function addGridItem(model)
    {
       // alert(model.TermCode);
       // var cur
        table2.row.add([
       model.TermCode,
        model.CurrentTerm ? "Yes" : "No",
       model.Status,
        '<a id="edit" data-key="'+ model.ItbId + '" class="edit btn btn-xs btn-primary"><i class="fa fa-edit"></i> Edit</a>'+
                       '<form id="FormCurrent" class="inline" method="post">' +
                     
                            '<a id="current" data-key="'+ model.ItbId + '" class="current btn btn-xs btn-success"><i class="fa fa-info"></i> Set as Current</a> </form>'         
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
        d[0] = model.TermCode;
       // alert(d[0]);
      
        table2
     .row(rowIdx)
     .data(d)
     .draw();
    }
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

    $('#btnNew').click(function () {
        formClear('#FormTerm');
        $('#pnlAudit').css('display', 'none');
        $('#btnSubmit').text('Save');
    });
});