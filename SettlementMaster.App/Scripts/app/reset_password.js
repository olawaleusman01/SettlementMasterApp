$(document).ready(function () {
    /// GetRoles(1);

    var col;
    var menuId = $('#menuId').val();
    //alert(menuId);
    var url = BaseUrl() + 'ResetPassword/';
    var table2 = $('.datatable').DataTable({
        ajax: url + "UserList",
        pageLength:100,
        columns: [
             {
                data: null,
                className: "center_column",
                //defaultContent: '<a class="btn btn-primary btn-xs editor_edit" data-key="' + data.RoleId + '"><i class="fa fa-edit"></i></a>' +
                //    '  <a class="btn btn-warning btn-xs editor_remove" data-key="' + data.RoleId + '"><i class="fa fa-trash-o"></i></a>',
                render: function (data, type, row) {
                    // Combine the first and last names into a single table field

                    var html = '<input type="checkbox" name="chkSingle" id="chkSingle" value="'+ data.UserName + '" />';
                    return html;
                }
            },
            { data: "UserName" },
            { data: "FullName" },
            { data: "RoleName" },
            { data: "Email" },
            { data: "Status" },
        ]
    });

    $(document).on('change', '#chkSingle', function () {
     
        var cnt = $('#example1 input[id*="chkSingle"]:checkbox:checked').size();
       // alert(cnt);
        if (cnt > 0) {
            $('#btnReset').removeAttr('disabled');
        }
        else {
            $('#btnReset').prop('disabled',true);
        }
    })
    $(document).on('change','#example1 #chkAll', function () {
        if ($('#example1 #chkAll').is(':checked')) {
            $("#example1 :checkbox[name ='chkSingle']").prop("checked", true);
            $('#btnReset').prop('disabled', false);
        }
        else {
            $("#example1 :checkbox[name ='chkSingle']").prop("checked", false);
            $('#btnReset').prop('disabled', true);
        }
    });
    $(document).on('click','#example1 #btnReset',function (e) {
        e.preventDefault();
        // alert('i get clicked');
      
        var cnt = $('#example1 input[name*="chkSingle"]:checkbox:checked').size();
        // alert(cnt);
        if (cnt == 0) {
            displayDialogNoty('Notification', 'No Record is Selected from the Grid');
            return;
        }
        var lst = new Array();
        $('#example1 input[name*="chkSingle"]:checkbox:checked').each(function (i, e) {
            //alert($(this).val());
            lst.push($(this).val());
        });
        //console.log(JSON.stringify({ selected_value: lst }));
        
        loaderSpin2(true);
        $.ajax({
            type: "POST",
            url: url + 'ResetPassword',
            contentType: "application/json",
            data: JSON.stringify({ selected_value: lst }),// JSON.stringify(data),
            success: function (response) {
                loaderSpin2(false);
                if (response.RespCode == 0) {
                    table2.ajax.reload();
                    displayDialogNoty('Notification', response.RespMessage);
                }
                else {
                    //alert(response.RespMessage);
                    var msg = '<p>' + response.RespMessage + '</p>';
                    displayDialogNoty('Notification', msg);
                }
            },
            error: function (err) {
                //alert('Error Posting Record');
                console.log(err);
                loaderSpin2(false);
            }
        });
    });
  
});