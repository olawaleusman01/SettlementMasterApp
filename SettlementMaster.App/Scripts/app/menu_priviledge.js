$(document).ready(function () {
    var url = BaseUrl() + 'Roles';
   // alert(BaseUrl());
   // var ret = location.pathname.substring(location.pathname.lastIndexOf("/") + 1);
    // alert(ret);
    var roleid = 0 ;
    //if (parseInt(ret))
    //{
    //    roleid = ret;
    //    $('#RoleId').val(roleid);
        GetPriv();
   // }
   
    
    //GetRoles(roleid);
    //bindParentMenu();
     
    var col;
    var validator = $("#formMenuPriviledge").validate({
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
            

            urlTemp = url + '/SaveMenuPrivilege';
            postTemp = 'post';
               
           
            //  alert(urlTemp);
                var $reqLogin = {
                    url: urlTemp,

                    data: $('#formMenuPriviledge').serialize(),
                    type: postTemp,
                    contentType: "application/x-www-form-urlencoded"
                };
           // alert($('#formMenuPriviledge').serialize());
            loaderSpin(true);
            disableButton(true);
            var ajax = new Ajax();
            ajax.send($reqLogin).done(function (response) {
                loaderSpin(false);
                disableButton(false);
                if (response.RespCode === 0) {
                   // new Util().clearform('#formMenuPriviledge');

                   
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
        new Util().clearform('#formMenuPriviledge');

        validator.resetForm();
        $('#pnlAudit').css('display', 'none');
        $('#ItbId').val(0);
        $('#btnSave').val(1);
        $('#btnSave').html('<i class="fa fa-save"></i> Save');
        // $('#UserName').removeAttr('disabled', 'disabled');
        $('a.editor_reset').show();
        $('#myModal').modal({ backdrop: 'static', keyboard: false });
    });
  
    $("#formMenuPriviledge #RoleId").change(function () {
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

    function GetPriv()
    {
       // alert(url + '/GetPrivilege')
        var options = {};
        options.url = url + '/GetMenuPrivilege';
        options.type = "POST";
        //options.data = JSON.stringify({ RoleId: roleid });
        options.dataType = "json";
        options.contentType = "application/json";
        options.success = function (response) {
            $("#example1 tbody").empty();
            var data = response.data;
            for (var i = 0; i < data.length; i++) {
                //if (i == 0)
                //{
                //    //alert(data[i].RoleName);
                //    $('#lblRoleName').html(data[i].RoleName);
                //}
               // alert(data[i].CanDelete);
                var rw = '<tr>'
                         + '<td>' + (i + 1) + '</td>'
                         + '<td> <input type="hidden" name="MenuPrivList[' + i + '].MenuId" id="MenuId_' + i + '" value="' + data[i].MenuId + '" />'
                         // + '<input type="hidden" name="MenuPrivList[' + i + '].RoleAssigId" id="RoleAssigId_' + i + '" value="' + data[i].RoleAssigId + '" />'
                         +       data[i].MenuName + '</td>'
                         + '<td style="text-align:center"> <input class="CanPos" id="CanPos_' + i + '" type="radio" name="MenuPrivList[' + i + '].App"' + (data[i].App == 'P' ? 'checked' : '') + ' value="P" /> </td>'
                         + '<td style="text-align:center"> <input class="CanAtm" id="CanAtm_' + i + '" type="radio" name="MenuPrivList[' + i + '].App" ' + (data[i].App == 'A' ? 'checked' : '') + ' value="A" /> </td>'
                         + '<td style="text-align:center"> <input class="CanBoth" id="CanBoth_' + i + '" type="radio" name="MenuPrivList[' + i + '].App" ' + (data[i].App == 'G' ? 'checked' : '') + ' value="G" /> </td>'
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

    //$(document).on('click', '.CanPos ,.CanAtm ,.CanBoth', function () {
    //   // alert('lol');
    //    //if ($(this).
    //    if ($(this).is(":checked")) {
    //        $(this).val(true);
    //       // alert($(this).val());
    //    }
    //    else {
    //        $(this).val(false);
    //        //alert($(this).val());
    //    }
    //});


    $("#CanPosAll").change(function () {
         //alert("here");
        // alert($("[id$=chkCrAll]").is(":checked"));
        if ($("#CanPosAll").is(":checked")) {
            //  alert("Handle");
            $("#example1 :radio[id *='CanPos']").prop("checked", "checked");
           // $("#example1 :radio[id *='CanPos']").val(true);
            $("#CanAtmAll,#CanBothAll").removeAttr("checked");
        }
        else {
            // alert("Not Handle");
            // $("#grdSearchCr :checkbox[id *='chkCrSingle']").prop("checked", "checked");
            $("#example1 :radio[id *='CanPos']").removeAttr("checked");
           // $("#example1 :radio[id *='CanPos']").val(false);
            // alert("Not Handle after");
        }
    });

    $("#CanAtmAll").change(function () {
      
        if ($("#CanAtmAll").is(":checked")) {
            //  alert("Handle");
            $("#example1 :radio[id *='CanAtm']").prop("checked", "checked");
           // $("#example1 :radio[id *='CanAtm']").val(true);
            $("#CanPosAll,#CanBothAll").removeAttr("checked");
        }
        else {
           
            $("#example1 :radio[id *='CanAtm']").removeAttr("checked");
            //$("#example1 :radio[id *='CanAtm']").val(false);
        }
    });

    $("#CanBothAll").change(function () {
      
        if ($("#CanBothAll").is(":checked")) {
      
            $("#example1 :radio[id *='CanBoth']").prop("checked", "checked");
            //$("#example1 :radio[id *='CanBoth']").val(true);
            $("#CanAtmAll,#CanPosAll").removeAttr("checked");
        }
        else {
         
            $("#example1 :radio[id *='CanBoth']").removeAttr("checked");
           // $("#example1 :radio[id *='CanBoth']").val(false);
          
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