$(document).ready(function () {
    var url = BaseUrl() + 'Roles';
   // alert(BaseUrl());
    var ret = location.pathname.substring(location.pathname.lastIndexOf("/") + 1);
    // alert(ret);
    var roleid = 0 ;
    if (parseInt(ret))
    {
        roleid = ret;
        $('#RoleId').val(roleid);
        GetPriv(roleid);
    }
   
    
    //GetRoles(roleid);
    //bindParentMenu();
     
    var col;
    var validator = $("#formPriviledge").validate({
        rules: {
            Name: "required",
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
            

                urlTemp = url + '/SavePrivilege';
                postTemp = 'post';
               
           
            //  alert(urlTemp);
            var $reqLogin = {
                url: urlTemp,

                data: $('#formPriviledge').serialize(),
                type: postTemp,
                contentType: "application/x-www-form-urlencoded"
            };
           // alert($('#formPriviledge').serialize());
            loaderSpin(true);
            disableButton(true);
            var ajax = new Ajax();
            ajax.send($reqLogin).done(function (response) {
                loaderSpin(false);
                disableButton(false);
                if (response.RespCode === 0) {
                   // new Util().clearform('#formPriviledge');

                   
                        // alert('Record Created Successfully');
                        displayDialogNoty('Notification', response.RespMessage);
                       // GetPriv($('#RoleId').val());

                }
                else {
                    // alert(response.RespMessage)
                    displayDialogNoty('Notification', response.RespMessage, true);

                }


            }).fail(function (xhr, status, err) {
                loaderSpin(false);
                disableButton(false);
                displayDialogNoty('Notification', 'No network connectivity. Please check your network connections.', true);
                //$('#display-error').show();
                //$('#errorMessage').text("No network connectivity. Please check your network connections.");
                // errorMessage
                // showPopDialog("No network connectivity. Please check your network connections.", handlerError);
            });


        }
    });





   // $("#example1").on("click", "a.editor_edit", editDetailServer);


    $('a.editor_create').click(function () {
        // formClear('#FormTerm');
        new Util().clearform('#formPriviledge');

        validator.resetForm();
        $('#pnlAudit').css('display', 'none');
        $('#ItbId').val(0);
        $('#btnSave').val(1);
        $('#btnSave').html('<i class="fa fa-save"></i> Save');
        // $('#UserName').removeAttr('disabled', 'disabled');
        $('a.editor_reset').show();
        $('#myModal').modal({ backdrop: 'static', keyboard: false });
    });
  
    $("#formPriviledge #RoleId").change(function () {
        if ($("#RoleId").val() != "") {
            
            GetPriv($('#RoleId').val());
            // $("#example1 :checkbox[id *='CanView']").prop("checked", "checked");
            $("#CanViewAll,#CanAddAll,#CanEditAll").removeAttr("checked");
        }
        else {
            $("#example1 tbody").empty();
            $("#CanViewAll,#CanAddAll,#CanEditAll").removeAttr("checked");
        }
    });

    function GetPriv(roleid)
    {
       // alert(url + '/GetPrivilege')
        var options = {};
        options.url = url + '/GetPrivilege';
        options.type = "POST";
        options.data = JSON.stringify({ RoleId: roleid });
        options.dataType = "json";
        options.contentType = "application/json";
        options.success = function (response) {
            $("#example1 tbody").empty();
            var data = response.data;
            for (var i = 0; i < data.length; i++) {
                if (i == 0)
                {
                    //alert(data[i].RoleName);
                    $('#lblRoleName').html(data[i].RoleName);
                }
               // alert(data[i].CanDelete);
                var rw = '<tr>'
                         + '<td>' + (i + 1) + '</td>'
                         + '<td> <input type="hidden" name="RolePrivList[' + i + '].MenuId" id="MenuId_' + i + '" value="' + data[i].MenuId + '" />'
                          + '<input type="hidden" name="RolePrivList[' + i + '].RoleAssigId" id="RoleAssigId_' + i + '" value="' + data[i].RoleAssigId + '" />'
                         +       data[i].MenuName + '</td>'
                         + '<td style="text-align:center"> <input class="CanView" id="CanView_' + i + '" type="checkbox" name="RolePrivList[' + i + '].CanView"' + (data[i].CanView ? 'checked' : '') + ' value="' + data[i].CanView + '" /> </td>'
                         + '<td style="text-align:center"> <input class="CanAdd" id="CanAdd_' + i + '" type="checkbox" name="RolePrivList[' + i + '].CanAdd" ' + (data[i].CanAdd ? 'checked' : '') + ' value="' + data[i].CanAdd + '" /> </td>'
                         + '<td style="text-align:center"> <input class="CanEdit" id="CanEdit_' + i + '" type="checkbox" name="RolePrivList[' + i + '].CanEdit" ' + (data[i].CanEdit ? 'checked' : '') + ' value="' + data[i].CanEdit + '" /> </td>'
                        // + '<td style="text-align:center"> <input id="CanDelete_' + i + '" type="checkbox" name="RolePrivList[' + i + '].CanDelete" ' + (data[i].CanDelete ? 'checked' : '') + ' value="' + data[i].CanDelete + '" /> </td>'
                        // + '<td style="text-align:center"> <input class="CanAuth" id="CanAuthorize_' + i + '" type="checkbox" name="RolePrivList[' + i + '].CanAuthorize" ' + (data[i].CanAuthorize ? 'checked' : '') + ' value="' + data[i].CanAuthorize + '" /> </td>'
                         + '</tr>';
                $("#example1 tbody").append(rw);
            }
           
        };
        options.error = function () {
            //alert("Error retrieving states!");
            return null;
        };
        $.ajax(options);
    }

    $(document).on('click', '.CanView ,.CanAdd ,.CanEdit,.CanAuth', function () {
       // alert('lol');
        //if ($(this).
        if ($(this).is(":checked")) {
            $(this).val(true);
           // alert($(this).val());
        }
        else {
            $(this).val(false);
            //alert($(this).val());
        }
    });


    $("#CanViewAll").change(function () {
         //alert("here");
        // alert($("[id$=chkCrAll]").is(":checked"));
        if ($("#CanViewAll").is(":checked")) {
            //  alert("Handle");
            $("#example1 :checkbox[id *='CanView']").prop("checked", "checked");
            $("#example1 :checkbox[id *='CanView']").val(true);
        }
        else {
            // alert("Not Handle");
            // $("#grdSearchCr :checkbox[id *='chkCrSingle']").prop("checked", "checked");
            $("#example1 :checkbox[id *='CanView']").removeAttr("checked");
            $("#example1 :checkbox[id *='CanView']").val(false);
            // alert("Not Handle after");
        }
    });

    $("#CanAddAll").change(function () {
      
        if ($("#CanAddAll").is(":checked")) {
            //  alert("Handle");
            $("#example1 :checkbox[id *='CanAdd']").prop("checked", "checked");
            $("#example1 :checkbox[id *='CanAdd']").val(true);
        }
        else {
           
            $("#example1 :checkbox[id *='CanAdd']").removeAttr("checked");
            $("#example1 :checkbox[id *='CanAdd']").val(false);
        }
    });

    $("#CanEditAll").change(function () {
      
        if ($("#CanEditAll").is(":checked")) {
      
            $("#example1 :checkbox[id *='CanEdit']").prop("checked", "checked");
            $("#example1 :checkbox[id *='CanEdit']").val(true);
        }
        else {
         
            $("#example1 :checkbox[id *='CanEdit']").removeAttr("checked");
            $("#example1 :checkbox[id *='CanEdit']").val(false);
          
        }
    });
    $("#CanAuthAll").change(function () {

        if ($("#CanAuthAll").is(":checked")) {

            $("#example1 :checkbox[id *='CanAuth']").prop("checked", "checked");
            $("#example1 :checkbox[id *='CanAuth']").val(true);
        }
        else {

            $("#example1 :checkbox[id *='CanAuth']").removeAttr("checked");
            $("#example1 :checkbox[id *='CanAuth']").val(false);

        }
    });
    function bindParentMenu()
    {
        $("#ParentId").empty();
        $("#ParentId").append("<option value=''> Display All </option>");
     var $reqLogin = {
         url: url + '/ParentMenuList',

        data: null,
        type: "Get",
        contentType: "application/json"
    };

    var ajax = new Ajax();
    ajax.send($reqLogin).done(function (response) {
       // alert(response.data);
        var exist = false;
        if (response.data.length > 0) {
          
            for (var i = 0; i < response.data.length; i++) {
                $("#ParentId").append("<option value='" + response.data[i].ParentId + "'>" +
                response.data[i].ParentName + "</option>");
               
            }
          

        }
       // return response.data;
    }).fail(function (xhr, status, err) {
        return null;
       
    });
    }
});